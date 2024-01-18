using Home.Tickets.Domain.Entities;
using Home.Tickets.Infrastructure.Database.EntityMappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Infrastructure.Database {
    public class TicketContext : DbContext {

        public const string schema = "tickets";
        private IDbContextTransaction dbContextTransaction = null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken) {
            if (dbContextTransaction != null) {
                throw new Exception("Context already has open transaction");
            }

            dbContextTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken) {
            await dbContextTransaction?.CommitAsync(cancellationToken);
        }

        public async Task RollBackTransactionAsync(CancellationToken cancellationToken) {
            await dbContextTransaction?.RollbackAsync(cancellationToken);
        }

        public TicketContext(DbContextOptions<TicketContext> dbContextOptions) : base(dbContextOptions) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.HasDefaultSchema(schema);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            await base.SaveChangesAsync(cancellationToken);

            var events = this.ChangeTracker.
                Entries()
                .Where(x => x.Entity is EntityBase)
                .Select(x => x.Entity)
                .Cast<EntityBase>()
                .SelectMany(x => x.GetEvents())
                .Select(x => {
                    //this.eventFiller.Fill(x);

                    var outbox = new OutBox.Service.OutBox("tickets", x.GetType().AssemblyQualifiedName, System.Text.Json.JsonSerializer.Serialize(x, x.GetType()));

                    //this.outBoxPropertiesFiller.Fill(outbox);

                    return outbox;
                })
                .ToList();

            var dbSet = this.Set<OutBox.Service.OutBox>();

            await dbSet.AddRangeAsync(events, cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task MigrateAsync() {
            //await this.Database.EnsureDeletedAsync();
            await this.Database.MigrateAsync();
        }
    }
}

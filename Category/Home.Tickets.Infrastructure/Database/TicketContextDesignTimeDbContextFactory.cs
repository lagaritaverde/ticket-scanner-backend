using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Home.Tickets.Infrastructure.Database {
    internal class TicketContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TicketContext> {
        public TicketContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<TicketContext>();

            Console.WriteLine(string.Join(Environment.NewLine, args));

            optionsBuilder.UseNpgsql(args[0], x =>
            {
                x.MigrationsHistoryTable("__EFMigration", TicketContext.schema);
            });

            return new TicketContext(optionsBuilder.Options);
        }
    }
}

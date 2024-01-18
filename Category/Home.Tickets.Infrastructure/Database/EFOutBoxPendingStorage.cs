using Home.OutBox.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Infrastructure.Database {
    public class EFOutBoxPendingStorage : IOutBoxPendingStorage {
        private readonly DbSet<OutBox.Service.OutBox> dbSet;
        private readonly TicketContext ticketContext;

        public EFOutBoxPendingStorage(TicketContext ticketContext) {
            this.dbSet = ticketContext.Set<OutBox.Service.OutBox>();
            this.ticketContext = ticketContext;
        }

        public async Task<OutBox.Service.OutBox?> Get(CancellationToken cancellationToken = default) {
            return await dbSet.OrderBy(x => x.Id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task Delete(OutBox.Service.OutBox outBox, CancellationToken cancellationToken = default) {
            dbSet.Remove(outBox);

            await ticketContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<OutBox.Service.OutBox>> Get(int count, CancellationToken cancellationToken = default) {
            return await dbSet.OrderBy(x => x.Id).Take(count).ToArrayAsync(cancellationToken);
        }

        public async Task Delete(IEnumerable<OutBox.Service.OutBox> outBoxes, CancellationToken cancellationToken = default) {
            dbSet.RemoveRange(outBoxes);

            await ticketContext.SaveChangesAsync(cancellationToken);
        }
    }
}

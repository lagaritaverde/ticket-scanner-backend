using Home.Tickets.Domain.Entities;

namespace Home.Tickets.Domain.Specifications
{
    public class AllowedTicketSpecification(string id, string accountingGroup) : Specification<Ticket>
    {
        public override IQueryable<Ticket> Get(IQueryable<Ticket> query)
        {
            return query.Where(x => x.Id == id && x.AccountingGroup == accountingGroup);
        }
    }
}

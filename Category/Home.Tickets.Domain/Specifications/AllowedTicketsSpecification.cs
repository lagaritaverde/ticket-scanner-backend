using Home.Tickets.Domain.Entities;

namespace Home.Tickets.Domain.Specifications
{
    public class AllowedTicketsSpecification(string accountingGroup) : Specification<Ticket>
    {
        public override IQueryable<Ticket> Get(IQueryable<Ticket> query)
        {
            return query.Where(x => x.AccountingGroup == accountingGroup);
        }
    }
}

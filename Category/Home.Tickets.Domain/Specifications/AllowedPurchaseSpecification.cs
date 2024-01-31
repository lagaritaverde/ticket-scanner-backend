using Home.Tickets.Domain.Entities;

namespace Home.Tickets.Domain.Specifications
{
    public class AllowedPurchaseSpecification(string id, string accountingGroup) : Specification<Purchase>
    {
        public override IQueryable<Purchase> Get(IQueryable<Purchase> query)
        {
            return query.Where(x => x.Id == id && x.AccountingGroup == accountingGroup);
        }
    }
}

using Home.Tickets.Domain.Entities;

namespace Home.Tickets.Domain.Specifications
{
    public class AllowedPurchasesByGroupSpecification(string accountingGroup, string groupId) : Specification<Purchase>
    {
        public override IQueryable<Purchase> Get(IQueryable<Purchase> query)
        {
            return query.Where(x => x.AccountingGroup == accountingGroup && x.GroupId == groupId);
        }
    }
}

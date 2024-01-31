namespace Home.Tickets.Domain.Specifications
{
    public abstract class Specification<T>
    {

        public abstract IQueryable<T> Get(IQueryable<T> predicate);
    }
}

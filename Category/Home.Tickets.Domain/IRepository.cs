using Home.Tickets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Tickets.Domain {
    public interface IRepository<T> where T : class {

        Task<T?> Get(object id);
        Task Add(T entity);
        IQueryable<T> List();
        Task AddRange(IEnumerable<T> entities);
    }
}

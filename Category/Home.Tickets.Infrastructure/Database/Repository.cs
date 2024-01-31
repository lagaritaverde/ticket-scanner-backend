﻿using Home.Tickets.Domain;
using Home.Tickets.Domain.Entities;
using Home.Tickets.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Home.Tickets.Infrastructure.Database
{
    public class Repository<T> : IRepository<T> where T : class {
        private readonly DbSet<T> dbSet;

        public Repository(TicketContext ticketContext) {
            dbSet = ticketContext.Set<T>();
        }

        public async Task Add(T entity) {
            await dbSet.AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities) {
            await dbSet.AddRangeAsync(entities);
        }

        public async Task<T?> Get(object id) {
            return await dbSet.FindAsync(id);
        }

        public async Task<T?> Get(Specification<T> specification) {

            var query = specification.Get(dbSet);

            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<T> List() {
            return dbSet.AsQueryable();
        }

        public async Task<T[]> List(Specification<T> specification) {
            var query = specification.Get(dbSet);

            return await query.ToArrayAsync();
        }
    }
}

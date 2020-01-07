using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WmKazTest.Data.Interfaces;

namespace WmKazTest.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        protected ObservationDataContext DataContext { get; }

        protected Repository(ObservationDataContext context)
        {
            DataContext = context;
            _dbSet = DataContext.Set<T>();
        }

        public virtual async Task<T> Add(T item)
        {
            var created = await _dbSet.AddAsync(item);
            return created.Entity;
        }

        public IEnumerable<T> Get()
        {
            return _dbSet;
        }

        public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            query = query.Where(filter);

            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty.Trim()));

            if (orderBy != null)
            {
                return await orderBy.Invoke(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> Get<TKeyType>(TKeyType id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual void Update(T item)
        {
            DataContext.Entry(item).State = EntityState.Modified;
        }

        public virtual async Task Delete<TKeyType>(TKeyType id)
        {
            var entity = await _dbSet.FindAsync(id);
            _dbSet.Remove(entity);
        }

        public virtual void DeleteAll()
        {
            _dbSet.RemoveRange(_dbSet);
        }
    }
}
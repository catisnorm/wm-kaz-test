using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WmKazTest.Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T item);

        IEnumerable<T> Get();

        Task<IEnumerable<T>> Get(Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "");

        Task<T> Get<TKeyType>(TKeyType id);

        void Update(T item);

        Task Delete<TKeyType>(TKeyType id);
        
        void DeleteAll();
    }
}
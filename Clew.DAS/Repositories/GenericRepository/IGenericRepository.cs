using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Clew.DAL
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllGenericAsync
         (
             Expression<Func<T, bool>>? expression = null,
             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
             bool trackChanges = false
         );

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(string id);

        void Add(T entity);

        void Delete(T entity);
    }
}

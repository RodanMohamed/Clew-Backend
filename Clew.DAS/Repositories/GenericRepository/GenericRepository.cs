using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Clew.DAL {
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllGenericAsync
             (
                 Expression<Func<T, bool>>? expression = null,
                 Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                 bool trackChanges = false
             )
        {
            IQueryable<T> query = _context.Set<T>();

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            if (trackChanges == false)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _context.Set<T>()
                .FindAsync(id);
        }

        public void Add(T entity)//no need to be async because  it saves in local container(it has not save changes) to avoid extra heavy but make savechangesAsync in unitofwork
        {

            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }


    }
}


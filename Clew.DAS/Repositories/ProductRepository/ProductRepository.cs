using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId)
            => await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Product>> GetByMaterialAsync(string material)
            => await _context.Products
                .Where(p => p.Material.ToLower() == material.ToLower())
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
            => await _context.Products
                .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Product?> GetWithCategoryAsync(string id)
            => await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}

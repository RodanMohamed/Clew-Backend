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
        {
            var term = keyword?.Trim() ?? string.Empty;

            return await _context.Products
                .Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && EF.Functions.Like(p.Name, $"%{term}%")) ||
                    (!string.IsNullOrEmpty(p.Description) && EF.Functions.Like(p.Description, $"%{term}%")))
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetWithCategoryAsync(string id)
            => await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(string categoryId);
        Task<IEnumerable<Product>> GetByMaterialAsync(string material);
        Task<IEnumerable<Product>> SearchAsync(string keyword);
        Task<Product?> GetWithCategoryAsync(string id);

    }
}

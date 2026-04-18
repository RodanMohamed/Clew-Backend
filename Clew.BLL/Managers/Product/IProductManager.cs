using Clew.Common;
using Clew.DAL;
using CLew.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{ 
    public interface IProductManager
    {
        Task<GeneralResult<PagedResult<Product>>> GetProductsPaginationAsync(
            PaginationParameters paginationParameters,
            ProductFilterParameters productFilterParameters);
        Task<GeneralResult<IEnumerable<ProductReadDto>>> GetProductsAsync();

        Task<GeneralResult<ProductReadDto>> GetProductByIdAsync(string id);

        Task<GeneralResult<ProductEditDto>> GetProductEditByIdAsync(string id);

        Task<GeneralResult<IEnumerable<ProductReadDto>>> GetProductsByCategoryAsync(string categoryId);

        Task<GeneralResult<IEnumerable<ProductReadDto>>> SearchProductsAsync(string searchTerm);

        Task<GeneralResult<ProductReadDto>> CreateProductAsync(ProductCreateDto productCreateDto);

        Task<GeneralResult<ProductEditDto>> EditAsync(ProductEditDto productEditDto);

        Task<GeneralResult<bool>> DeleteAsync(string id);

        Task<GeneralResult<bool>> UpdateStockAsync(string id, int quantityChange);

        Task<GeneralResult<bool>> ToggleFavoriteAsync(string productId, string userId);
    }


}

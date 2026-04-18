using Clew.Common;
using CLew.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public interface ICategoryManager
    {
        Task<GeneralResult<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync();
        Task<GeneralResult<CategoryReadDto>> GetCategoryByIdAsync(string id);
        Task<GeneralResult<PagedResult<CategoryReadDto>>> GetCategoriesPaginationAsync(
            PaginationParameters paginationParameters,
            string? searchTerm = null);

        
        Task<GeneralResult<CategoryReadDto>> CreateCategoryAsync(CategoryCreateDto createDto);
        Task<GeneralResult<CategoryEditDto>> EditCategoryAsync(CategoryEditDto editDto);
        Task<GeneralResult<bool>> DeleteCategoryAsync(string id);
    }
}

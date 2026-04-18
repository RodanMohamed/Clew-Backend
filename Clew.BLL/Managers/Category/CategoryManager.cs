using AutoMapper;
using Clew.Common;
using Clew.DAL;
using CLew.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryManager> _logger;

        public CategoryManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CategoryManager> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Query Methods

        public async Task<GeneralResult<IEnumerable<CategoryReadDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var categoriesDto = new List<CategoryReadDto>();

                foreach (var category in categories)
                {
                    var products = await _unitOfWork.Products.GetByCategoryAsync(category.Id);
                    var categoryDto = _mapper.Map<CategoryReadDto>(category);
                    categoryDto.ProductCount = products.Count();
                    categoriesDto.Add(categoryDto);
                }

                return GeneralResult<IEnumerable<CategoryReadDto>>.SuccessResult(categoriesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCategoriesAsync");
                return GeneralResult<IEnumerable<CategoryReadDto>>.FailResult("An error occurred while retrieving categories");
            }
        }

        public async Task<GeneralResult<CategoryReadDto>> GetCategoryByIdAsync(string id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category is null)
                {
                    return GeneralResult<CategoryReadDto>.NotFound($"Category with ID {id} not found");
                }

                var products = await _unitOfWork.Products.GetByCategoryAsync(category.Id);
                var categoryDto = _mapper.Map<CategoryReadDto>(category);
                categoryDto.ProductCount = products.Count();

                return GeneralResult<CategoryReadDto>.SuccessResult(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCategoryByIdAsync for ID: {CategoryId}", id);
                return GeneralResult<CategoryReadDto>.FailResult("An error occurred while retrieving the category");
            }
        }

        public async Task<GeneralResult<PagedResult<CategoryReadDto>>> GetCategoriesPaginationAsync(
            PaginationParameters paginationParameters,
            string? searchTerm = null)
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var query = categories.AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                // Get total count before pagination
                var totalCount = query.Count();

                // Apply pagination
                var items = query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                // Map to DTO with product counts
                var itemsDto = new List<CategoryReadDto>();
                foreach (var category in items)
                {
                    var products = await _unitOfWork.Products.GetByCategoryAsync(category.Id);
                    var categoryDto = _mapper.Map<CategoryReadDto>(category);
                    categoryDto.ProductCount = products.Count();
                    itemsDto.Add(categoryDto);
                }

                var pagedResult = new PagedResult<CategoryReadDto>
                {
                    Items = itemsDto,
                    Metadata = new PaginationMetedata
                    {
                        CurrentPage = paginationParameters.PageNumber,
                        PageNumber = paginationParameters.PageNumber,
                        PageSize = paginationParameters.PageSize,
                        TotalCount = totalCount
                    }
                };

                return GeneralResult<PagedResult<CategoryReadDto>>.SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCategoriesPaginationAsync");
                return GeneralResult<PagedResult<CategoryReadDto>>.FailResult("An error occurred while retrieving categories");
            }
        }

        #endregion

        #region Command Methods

        public async Task<GeneralResult<CategoryReadDto>> CreateCategoryAsync(CategoryCreateDto createDto)
        {
            try
            {
                // Check if category with same name already exists
                var existingCategories = await _unitOfWork.Categories.GetAllAsync();
                if (existingCategories.Any(c => c.Name.Equals(createDto.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return GeneralResult<CategoryReadDto>.FailResult($"Category with name '{createDto.Name}' already exists");
                }

                // Map DTO to Entity
                var category = _mapper.Map<Category>(createDto);

                // Generate ID (ct1, ct2, ct3 pattern)
                var maxId = existingCategories
                    .Where(c => c.Id.StartsWith("ct"))
                    .Select(c => int.Parse(c.Id.Substring(2)))
                    .DefaultIfEmpty(0)
                    .Max();
                category.Id = $"ct{maxId + 1}";

                // Add to database
                _unitOfWork.Categories.Add(category);
                await _unitOfWork.SaveChangesAsync();

                var categoryReadDto = _mapper.Map<CategoryReadDto>(category);
                categoryReadDto.ProductCount = 0;

                return GeneralResult<CategoryReadDto>.SuccessResult(categoryReadDto, "Category created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCategoryAsync for category: {@CreateDto}", createDto);
                return GeneralResult<CategoryReadDto>.FailResult("An error occurred while creating the category");
            }
        }

        public async Task<GeneralResult<CategoryEditDto>> EditCategoryAsync(CategoryEditDto editDto)
        {
            try
            {
                var categoryInDb = await _unitOfWork.Categories.GetByIdAsync(editDto.Id);
                if (categoryInDb is null)
                {
                    return GeneralResult<CategoryEditDto>.NotFound($"Category with ID {editDto.Id} not found");
                }

                // Check if new name conflicts with another category
                var existingCategories = await _unitOfWork.Categories.GetAllAsync();
                if (existingCategories.Any(c => c.Name.Equals(editDto.Name, StringComparison.OrdinalIgnoreCase) && c.Id != editDto.Id))
                {
                    return GeneralResult<CategoryEditDto>.FailResult($"Category with name '{editDto.Name}' already exists");
                }

                // Update entity properties (EF Core tracks changes automatically)
                _mapper.Map(editDto, categoryInDb);

                // Save changes
                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<CategoryEditDto>.SuccessResult(editDto, "Category updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditCategoryAsync for category: {@EditDto}", editDto);
                return GeneralResult<CategoryEditDto>.FailResult("An error occurred while updating the category");
            }
        }

        public async Task<GeneralResult<bool>> DeleteCategoryAsync(string id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category is null)
                {
                    return GeneralResult<bool>.NotFound($"Category with ID {id} not found");
                }

                // Check if category has products
                var products = await _unitOfWork.Products.GetByCategoryAsync(id);
                if (products.Any())
                {
                    return GeneralResult<bool>.FailResult($"Cannot delete category '{category.Name}' because it has {products.Count()} products. Reassign or delete the products first.");
                }

                // Delete from database
                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<bool>.SuccessResult(true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCategoryAsync for ID: {CategoryId}", id);
                return GeneralResult<bool>.FailResult("An error occurred while deleting the category");
            }
        }

        #endregion
    }
}

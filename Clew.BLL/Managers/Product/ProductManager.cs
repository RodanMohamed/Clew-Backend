using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Clew.BLL;
using Clew.Common;
using Clew.DAL;
using CLew.Common;
using Microsoft.Extensions.Logging;

namespace Clew.BLL
{
    public class ProductManager : IProductManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductManager> _logger;

        public ProductManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductManager> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        #region Query Methods

        public async Task<GeneralResult<PagedResult<Product>>> GetProductsPaginationAsync(
            PaginationParameters paginationParameters,
            ProductFilterParameters productFilterParameters)
        {
            try
            {
                
                var products = await _unitOfWork.Products.GetAllAsync();
                var query = products.AsQueryable();

                
                query = ApplyFilters(query, productFilterParameters);

                
                var totalCount = query.Count();

                
                var items = query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                var pagedResult = new PagedResult<Product>
                {
                    Items = items,
                    Metadata = new PaginationMetedata
                    {
                        CurrentPage = paginationParameters.PageNumber,
                        PageNumber = paginationParameters.PageNumber,
                        PageSize = paginationParameters.PageSize,
                        TotalCount = totalCount
                    }
                };

                return GeneralResult<PagedResult<Product>>.SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductsPaginationAsync");
                return GeneralResult<PagedResult<Product>>.FailResult("An error occurred while retrieving products");
            }
        }

        public async Task<GeneralResult<IEnumerable<ProductReadDto>>> GetProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                var productsDto = _mapper.Map<IEnumerable<ProductReadDto>>(products);
                return GeneralResult<IEnumerable<ProductReadDto>>.SuccessResult(productsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductsAsync");
                return GeneralResult<IEnumerable<ProductReadDto>>.FailResult("An error occurred while retrieving products");
            }
        }

        public async Task<GeneralResult<ProductReadDto>> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetWithCategoryAsync(id);
                if (product is null)
                {
                    return GeneralResult<ProductReadDto>.NotFound($"Product with ID {id} not found");
                }

                var productReadDto = _mapper.Map<ProductReadDto>(product);
                return GeneralResult<ProductReadDto>.SuccessResult(productReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductByIdAsync for ID: {ProductId}", id);
                return GeneralResult<ProductReadDto>.FailResult("An error occurred while retrieving the product");
            }
        }

        public async Task<GeneralResult<ProductEditDto>> GetProductEditByIdAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetWithCategoryAsync(id);
                if (product is null)
                {
                    return GeneralResult<ProductEditDto>.NotFound($"Product with ID {id} not found");
                }

                var productEditDto = _mapper.Map<ProductEditDto>(product);
                return GeneralResult<ProductEditDto>.SuccessResult(productEditDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductEditByIdAsync for ID: {ProductId}", id);
                return GeneralResult<ProductEditDto>.FailResult("An error occurred while retrieving the product");
            }
        }

        public async Task<GeneralResult<IEnumerable<ProductReadDto>>> GetProductsByCategoryAsync(string categoryId)
        {
            try
            {
                var products = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
                var productsDto = _mapper.Map<IEnumerable<ProductReadDto>>(products);
                return GeneralResult<IEnumerable<ProductReadDto>>.SuccessResult(productsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProductsByCategoryAsync for CategoryId: {CategoryId}", categoryId);
                return GeneralResult<IEnumerable<ProductReadDto>>.FailResult("An error occurred while retrieving products by category");
            }
        }

        public async Task<GeneralResult<IEnumerable<ProductReadDto>>> SearchProductsAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetProductsAsync();
                }

                var products = await _unitOfWork.Products.SearchAsync(searchTerm);
                var productsDto = _mapper.Map<IEnumerable<ProductReadDto>>(products);
                return GeneralResult<IEnumerable<ProductReadDto>>.SuccessResult(productsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchProductsAsync for term: {SearchTerm}", searchTerm);
                return GeneralResult<IEnumerable<ProductReadDto>>.FailResult("An error occurred while searching products");
            }
        }

        #endregion

        #region Command Methods

        public async Task<GeneralResult<ProductReadDto>> CreateProductAsync(ProductCreateDto productCreateDto)
        {
            try
            {
                // Validate category exists
                var category = await _unitOfWork.Categories.GetByIdAsync(productCreateDto.CategoryId);
                if (category is null)
                {
                    return GeneralResult<ProductReadDto>.FailResult($"Category with ID {productCreateDto.CategoryId} not found");
                }

                // Map DTO to Entity
                var product = _mapper.Map<Product>(productCreateDto);

                // Generate ID and ProductCode
                product.Id = GenerateProductId();
                product.ProductCode = GenerateProductCode(category.Name);
                product.Status = product.Stock > 0 ? "In Stock" : "Out of Stock";

                // Add to database (void method - do NOT await)
                _unitOfWork.Products.Add(product);
                await _unitOfWork.SaveChangesAsync();

                // Get the created product with category
                var createdProduct = await _unitOfWork.Products.GetWithCategoryAsync(product.Id);
                var productReadDto = _mapper.Map<ProductReadDto>(createdProduct);

                return GeneralResult<ProductReadDto>.SuccessResult(productReadDto, "Product created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateProductAsync for product: {@ProductCreateDto}", productCreateDto);
                return GeneralResult<ProductReadDto>.FailResult("An error occurred while creating the product");
            }
        }

        public async Task<GeneralResult<ProductEditDto>> EditAsync(ProductEditDto productEditDto)
        {
            try
            {
                var productInDb = await _unitOfWork.Products.GetByIdAsync(productEditDto.Id);
                if (productInDb is null)
                {
                    return GeneralResult<ProductEditDto>.NotFound($"Product with ID {productEditDto.Id} not found");
                }

                // Validate category exists if changed
                if (productInDb.CategoryId != productEditDto.CategoryId)
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(productEditDto.CategoryId);
                    if (category is null)
                    {
                        return GeneralResult<ProductEditDto>.FailResult($"Category with ID {productEditDto.CategoryId} not found");
                    }
                }

                
                _mapper.Map(productEditDto, productInDb);

              
                productInDb.Status = productInDb.Stock > 0 ? "In Stock" : "Out of Stock";

                
                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<ProductEditDto>.SuccessResult(productEditDto, "Product updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EditAsync for product: {@ProductEditDto}", productEditDto);
                return GeneralResult<ProductEditDto>.FailResult("An error occurred while updating the product");
            }
        }

        public async Task<GeneralResult<bool>> DeleteAsync(string id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product is null)
                {
                    return GeneralResult<bool>.NotFound($"Product with ID {id} not found");
                }

                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<bool>.SuccessResult(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAsync for ID: {ProductId}", id);
                return GeneralResult<bool>.FailResult("An error occurred while deleting the product");
            }
        }

        public async Task<GeneralResult<bool>> UpdateStockAsync(string id, int quantityChange)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product is null)
                {
                    return GeneralResult<bool>.NotFound($"Product with ID {id} not found");
                }

                product.Stock += quantityChange;

                if (product.Stock < 0)
                    product.Stock = 0;

    
                product.Status = product.Stock > 0 ? "In Stock" : "Out of Stock";


                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<bool>.SuccessResult(true, "Stock updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStockAsync for ID: {ProductId}, Change: {QuantityChange}", id, quantityChange);
                return GeneralResult<bool>.FailResult("An error occurred while updating stock");
            }
        }

        public async Task<GeneralResult<bool>> ToggleFavoriteAsync(string productId, string userId)
        {
            try
            {
          
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product is null)
                {
                    return GeneralResult<bool>.NotFound($"Product with ID {productId} not found");
                }

              
                var allFavourites = await _unitOfWork.Favourites.GetAllAsync();
                var existingFavorite = allFavourites.FirstOrDefault(f => f.ProductId == productId && f.UserId == userId);

                if (existingFavorite is not null)
                {
                 
                    _unitOfWork.Favourites.Delete(existingFavorite);
                    await _unitOfWork.SaveChangesAsync();
                    return GeneralResult<bool>.SuccessResult(false, "Product removed from favorites");
                }
                else
                {
                   
                    var favorite = new Favourite
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        UserId = userId
                    };
                    _unitOfWork.Favourites.Add(favorite);
                    await _unitOfWork.SaveChangesAsync();
                    return GeneralResult<bool>.SuccessResult(true, "Product added to favorites");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ToggleFavoriteAsync for ProductId: {ProductId}, UserId: {UserId}", productId, userId);
                return GeneralResult<bool>.FailResult("An error occurred while toggling favorite");
            }
        }

        #endregion

        #region Private Helper Methods

        private IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductFilterParameters filters)
        {
            if (filters == null)
                return query;

            if (!string.IsNullOrWhiteSpace(filters.CategoryId))
            {
                query = query.Where(p => p.CategoryId == filters.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(filters.Name))
            {
                var nameTerm = filters.Name.Trim();
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.Contains(nameTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filters.Description))
            {
                var descriptionTerm = filters.Description.Trim();
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.Description) && p.Description.Contains(descriptionTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filters.Material))
            {
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.Material) && p.Material.Contains(filters.Material.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            }

            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            return query;
        }

        private static string GenerateProductId()
        {
            return $"p{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        private static string GenerateProductCode(string categoryName)
        {
            var prefix = categoryName switch
            {
                "Rings" => "RG",
                "Earrings" => "ER",
                "Necklaces" => "NK",
                "Bracelets" => "BR",
                "Watches" => "WT",
                _ => "PR"
            };
            return $"{prefix}-{new Random().Next(1, 999):D3}";
        }

        #endregion
    }
}
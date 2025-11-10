using ItemsCache.Core.Abstraction.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SampleApi.Models;

namespace SampleApi.Controllers
{
    /// <summary>
    /// Products API controller demonstrating cache usage
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IItemsCacheService<int, Product> _productItemsCache;
        private readonly IItemsCacheGroupedService<Product, string> _categoryGroupedCache;
        private readonly IItemsCacheGroupedService<Product, bool> _activeGroupedCache;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IItemsCacheService<int, Product> productItemsCache,
            IItemsCacheGroupedService<Product, string> categoryGroupedCache,
            IItemsCacheGroupedService<Product, bool> activeGroupedCache,
            ILogger<ProductsController> logger)
        {
            _productItemsCache = productItemsCache ?? throw new ArgumentNullException(nameof(productItemsCache));
            _categoryGroupedCache = categoryGroupedCache ?? throw new ArgumentNullException(nameof(categoryGroupedCache));
            _activeGroupedCache = activeGroupedCache ?? throw new ArgumentNullException(nameof(activeGroupedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all products from cache
        /// </summary>
        /// <returns>List of all products</returns>
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                var products = _productItemsCache.GetAll().ToDictionary();
                _logger.LogInformation("Retrieved {Count} products from cache", products.Count);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products from cache");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific product by ID from cache
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Product if found, 404 if not found</returns>
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            try
            {
                _productItemsCache.TryGetByKey(id, out var product);
                
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {Id} not found in cache", id);
                    return NotFound($"Product with ID {id} not found");
                }

                _logger.LogInformation("Retrieved product {Id} from cache", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {Id} from cache", id);
                return StatusCode(500, "Internal server error");
            }
        }



        /// <summary>
        /// Gets cache statistics
        /// </summary>
        /// <returns>Cache statistics</returns>
        [HttpGet("cache/stats")]
        public ActionResult<object> GetCacheStats()
        {
            try
            {
                var allProducts = _productItemsCache.GetAll().ToDictionary();
                var activeProducts = allProducts.Values.Count(p => p.IsActive);
                var categories = allProducts.Values.Select(p => p.Category).Distinct().Count();

                var stats = new
                {
                    TotalProducts = allProducts.Count,
                    ActiveProducts = activeProducts,
                    InactiveProducts = allProducts.Count - activeProducts,
                    Categories = categories,
                    LastRetrieved = DateTime.UtcNow
                };

                _logger.LogInformation("Cache stats requested: {Stats}", stats);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets products for a specific category
        /// </summary>
        /// <param name="category">Category name</param>
        /// <returns>List of products in the category</returns>
        [HttpGet("grouped/category/{category}")]
        public ActionResult<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            try
            {
                var products = _categoryGroupedCache.GetByGroupKey(category).ToList();
                _logger.LogInformation("Retrieved {Count} products for category {Category}", products.Count, category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category {Category}", category);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets products grouped by active status
        /// </summary>
        /// <returns>Enumerable of active status-product pairs</returns>
        [HttpGet("grouped/active")]
        public ActionResult<IEnumerable<Product>> GetProductsGroupedByActive()
        {
            try
            {
                var grouped = _activeGroupedCache.GetByGroupKey(true);
                _logger.LogInformation("Retrieved {GroupCount} active status groups from cache", grouped.Count());
                return Ok(grouped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products grouped by active status");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}



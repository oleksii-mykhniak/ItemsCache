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
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IItemsCacheService<int, Product> productItemsCache,
            ILogger<ProductsController> logger)
        {
            _productItemsCache = productItemsCache ?? throw new ArgumentNullException(nameof(productItemsCache));
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
    }
}



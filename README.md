# ItemsCache

[![CI](https://github.com/oleksii-mykhniak/ItemsCache/workflows/CI/badge.svg)](https://github.com/oleksii-mykhniak/ItemsCache/actions)
[![NuGet](https://img.shields.io/nuget/v/ItemsCache.svg)](https://www.nuget.org/packages/ItemsCache/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A high-performance, flexible caching library for ASP.NET Core applications that provides automatic data loading, background refresh, and retry logic with SOLID principles.

## Features

- 🚀 **High Performance**: Optimized for speed with minimal memory overhead
- 🔄 **Background Refresh**: Automatic cache refresh with configurable intervals
- 🛡️ **Retry Logic**: Built-in retry policies using Polly for resilient operations
- 🏗️ **SOLID Principles**: Clean architecture with proper abstractions
- 📦 **Modular Design**: Use only what you need with separate packages
- 🔧 **Easy Integration**: Simple setup with dependency injection
- 📊 **Observability**: Built-in logging and monitoring support

## Packages

| Package | Description | NuGet |
|---------|-------------|-------|
| **ItemsCache.All** | Complete package with all features | [![NuGet](https://img.shields.io/nuget/v/ItemsCache.All.svg)](https://www.nuget.org/packages/ItemsCache.All/) |
| **ItemsCache** | Core caching functionality | [![NuGet](https://img.shields.io/nuget/v/ItemsCache.svg)](https://www.nuget.org/packages/ItemsCache/) |
| **ItemsCache.Refresh** | Background refresh capabilities | [![NuGet](https://img.shields.io/nuget/v/ItemsCache.Refresh.svg)](https://www.nuget.org/packages/ItemsCache.Refresh/) |
| **ItemsCache.Refresh.Polling** | Polling-based refresh implementation | [![NuGet](https://img.shields.io/nuget/v/ItemsCache.Refresh.Polling.svg)](https://www.nuget.org/packages/ItemsCache.Refresh.Polling/) |
| **ItemsCache.RetryPolicy** | Retry policy implementations with Polly | [![NuGet](https://img.shields.io/nuget/v/ItemsCache.RetryPolicy.svg)](https://www.nuget.org/packages/ItemsCache.RetryPolicy/) |

## Quick Start

### Installation

For the complete experience, install the main package:

```bash
dotnet add package ItemsCache.All
```

Or install individual packages based on your needs:

```bash
dotnet add package ItemsCache
dotnet add package ItemsCache.Refresh.Polling
dotnet add package ItemsCache.RetryPolicy
```

### Basic Usage

1. **Register services** in your `Program.cs`:

```csharp
using ItemsCache.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add ItemsCache services
builder.Services.AddItemsCache()
    .AddRefreshPolling()
    .AddRetryPolicy();

var app = builder.Build();
```

2. **Create a data source**:

```csharp
public class ProductDataSource : IDataSource<Product>
{
    private readonly HttpClient _httpClient;
    
    public ProductDataSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Product> LoadAsync(string key)
    {
        var response = await _httpClient.GetFromJsonAsync<Product>($"/api/products/{key}");
        return response ?? throw new InvalidOperationException("Product not found");
    }
}
```

3. **Use the cache service**:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IItemsCacheService<Product> _cache;
    
    public ProductsController(IItemsCacheService<Product> cache)
    {
        _cache = cache;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(string id)
    {
        var product = await _cache.GetAsync(id);
        return Ok(product);
    }
}
```

## Advanced Configuration

### Background Refresh

Configure automatic cache refresh:

```csharp
builder.Services.AddItemsCache()
    .AddRefreshPolling(options =>
    {
        options.RefreshInterval = TimeSpan.FromMinutes(5);
        options.Enabled = true;
    });
```

### Retry Policies

Configure retry behavior:

```csharp
builder.Services.AddItemsCache()
    .AddRetryPolicy(options =>
    {
        options.MaxRetryAttempts = 3;
        options.DelayBetweenRetries = TimeSpan.FromSeconds(1);
        options.UseExponentialBackoff = true;
    });
```

### Custom Data Sources

Implement your own data source:

```csharp
public class DatabaseDataSource : IDataSource<User>
{
    private readonly AppDbContext _context;
    
    public DatabaseDataSource(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<User> LoadAsync(string key)
    {
        var user = await _context.Users.FindAsync(key);
        return user ?? throw new InvalidOperationException("User not found");
    }
}
```

### Grouped Cache

ItemsCache supports pre-computed grouped indexes that are automatically maintained in memory. This trades memory for CPU performance, providing O(1) lookups for grouped data.

**Register grouped cache services:**

```csharp
builder.Services.AddItemsCache<int, Product>();

// Group by category
builder.Services.AddItemsCacheGrouped<int, Product, string>(p => p.Category ?? "Uncategorized");

// Group by active status
builder.Services.AddItemsCacheGrouped<int, Product, bool>(p => p.IsActive);
```

**Use grouped cache in your controllers:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IItemsCacheGroupedService<int, Product, string> _categoryGroupedCache;
    
    public ProductsController(
        IItemsCacheGroupedService<int, Product, string> categoryGroupedCache)
    {
        _categoryGroupedCache = categoryGroupedCache;
    }
    
    [HttpGet("grouped/category")]
    public ActionResult<Dictionary<string, List<Product>>> GetProductsGroupedByCategory()
    {
        var grouped = _categoryGroupedCache.GetGrouped();
        return Ok(grouped);
    }
    
    [HttpGet("grouped/category/{category}")]
    public ActionResult<List<Product>> GetProductsByCategory(string category)
    {
        var products = _categoryGroupedCache.GetByGroupKey(category);
        return Ok(products);
    }
}
```

**Key Features:**
- **Automatic Updates**: Grouped indexes are automatically updated when cache items change
- **Memory Efficient**: Pre-computed indexes eliminate CPU overhead on retrieval
- **Multiple Groupings**: Register multiple grouped services for different grouping strategies
- **Thread Safe**: All operations are thread-safe

## Architecture

ItemsCache follows SOLID principles with a clean, modular architecture:

```
ItemsCache.Core.Abstraction
├── IDataSource<T>
├── IItemsCacheService<T>
└── IItemsCacheServiceWithModifications<T>

ItemsCache.Core
├── ItemsCacheService<T>
├── ItemsCacheLoader<T>
└── CacheInitHostedService

ItemsCache.Refresh.Abstraction
├── IRefreshItemCacheHandler<T>
└── IRefreshItemCacheHandlerFactory<T>

ItemsCache.Refresh.Core
├── RefreshItemCacheHandler<T>
└── RefreshItemCacheHandlerFactory<T>

ItemsCache.Refresh.Polling
├── PollingRefreshService<T>
└── PollingRefreshHostedService<T>

ItemsCache.RetryPolicy.Abstraction
├── IRetryPolicy<T>
└── RetryPolicyOptions<T>

ItemsCache.RetryPolicy.Polly
└── PollyRetryPolicy<T>
```

## Performance Considerations

- **Memory Efficient**: Uses weak references and proper disposal patterns
- **Thread Safe**: All operations are thread-safe and optimized for concurrent access
- **Lazy Loading**: Data is loaded only when needed
- **Background Processing**: Refresh operations don't block main thread
- **Configurable Limits**: Set memory and performance limits as needed

## Contributing

We welcome contributions! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

### Development Setup

1. Clone the repository
2. Install .NET 9.0 SDK
3. Run `dotnet restore`
4. Run `dotnet build`
5. Run `dotnet test`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📖 [Documentation](https://github.com/oleksii-mykhniak/ItemsCache/wiki)
- 🐛 [Issue Tracker](https://github.com/oleksii-mykhniak/ItemsCache/issues)
- 💬 [Discussions](https://github.com/oleksii-mykhniak/ItemsCache/discussions)

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for a list of changes and version history.

---

Made with ❤️ by the ItemsCache Contributors
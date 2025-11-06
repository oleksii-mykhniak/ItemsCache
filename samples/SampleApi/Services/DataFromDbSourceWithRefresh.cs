using ItemsCache.Refresh.Abstraction.Models;
using ItemsCache.Refresh.Polling.Abstraction.Interfaces;
using ItemsCache.Refresh.Polling.Abstraction.Models;
using Microsoft.EntityFrameworkCore;
using SampleApi.Data;
using SampleApi.Models;

namespace SampleApi.Services;

public class DataFromDbSourceWithRefresh : IDataSourceWithRefresh<int, Product, RefreshContext>
{
    private readonly AppDbContext _dbContext;

    public DataFromDbSourceWithRefresh(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CacheItemRefreshResult<int, Product, RefreshContext>> GetUpdatedItemsAsync(RefreshContext? lastRefreshContext, CancellationToken cancellationToken = default)
    {
        var lastRefreshTime = lastRefreshContext?.LastRefresh ?? DateTime.MinValue;

        // set before querying to avoid missing updates during the query execution
        var updatedTime = DateTime.UtcNow;

        var updatedItems = await _dbContext.Products
            .Where(p => p.UpdatedAt > lastRefreshTime || (p.DeletedAt != null && p.DeletedAt > lastRefreshTime))
            .ToListAsync(cancellationToken);

        var newRefreshContext = new RefreshContext
        {
            LastRefresh = updatedTime
        };

        var cacheItems = updatedItems
            .Select(item => new RefreshCacheItem<int, Product>(item.Id, item, item.DeletedAt != null ? RefreshCacheItemStatus.Deleted : RefreshCacheItemStatus.Updated))
            .ToList();

        return new CacheItemRefreshResult<int, Product, RefreshContext>(cacheItems, newRefreshContext);
    }
}
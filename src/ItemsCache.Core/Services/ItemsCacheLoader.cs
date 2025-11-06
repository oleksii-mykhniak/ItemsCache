using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Core.Services;

internal sealed class ItemsCacheLoader<TKey, TCacheItem> : IItemsCacheLoader
    where TKey : notnull
    where TCacheItem : class
{
    private readonly IDataSource<TKey, TCacheItem> _dataSource;
    private readonly IItemsCacheServiceWithModifications<TKey, TCacheItem> _itemsCacheService;
    private readonly ILogger<ItemsCacheLoader<TKey, TCacheItem>> _logger;

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    
    public ItemsCacheLoader(IDataSource<TKey, TCacheItem> dataSource, IItemsCacheServiceWithModifications<TKey, TCacheItem> itemsCacheService, ILogger<ItemsCacheLoader<TKey, TCacheItem>> logger)
    {
        _dataSource = dataSource;
        _itemsCacheService = itemsCacheService;
        _logger = logger;
    }

    public async Task<bool> LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);

            _logger.LogInformation("ItemsCache {CacheItemType} Loading initial cache", typeof(TCacheItem).Name);

            var allItems = await _dataSource.LoadAllAsync(cancellationToken);
            var allItemsDictionary = allItems.ToDictionary(item => item.Key, item => item.Value);
            _itemsCacheService.TryRefresh(allItemsDictionary);

            _logger.LogInformation("ItemsCache {CacheItemType} Successfully loaded {ItemCount} items into cache", typeof(TCacheItem).Name, allItemsDictionary.Count);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ItemsCache {CacheItemType} failed to load cache", typeof(TCacheItem).Name);
            return false;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}
using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Core.Services;

internal sealed class ItemsCacheLoader<TCacheItem, TKey> : IItemsCacheLoader
    where TCacheItem : class
    where TKey : notnull
{
    private readonly IDataSource<TCacheItem, TKey> _dataSource;
    private readonly IItemsCacheServiceWithModifications<TCacheItem, TKey> _itemsCacheService;
    private readonly ILogger<ItemsCacheLoader<TCacheItem, TKey>> _logger;

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    
    public ItemsCacheLoader(IDataSource<TCacheItem, TKey> dataSource, IItemsCacheServiceWithModifications<TCacheItem, TKey> itemsCacheService, ILogger<ItemsCacheLoader<TCacheItem, TKey>> logger)
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
            var allItemsDictionary = allItems.ToDictionary();
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
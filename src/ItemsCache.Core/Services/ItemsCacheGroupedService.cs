using ItemsCache.Core.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Core.Services;

internal sealed class ItemsCacheGroupedService<TKey, TCacheItem, TGroupKey> 
    : IItemsCacheGroupedService<TCacheItem, TGroupKey>, ICacheUpdateObserver<TKey, TCacheItem>
    where TKey : notnull
    where TGroupKey : notnull
    where TCacheItem : class
{
    private readonly Func<TCacheItem, TGroupKey> _keySelector;
    private readonly ILogger<ItemsCacheGroupedService<TKey, TCacheItem, TGroupKey>> _logger;
    private readonly object _lockObject = new();
    
    //TODO: use concurrent collections and lock only for atomic writes
    
    // Group -> Key -> Item mapping
    private readonly Dictionary<TGroupKey, Dictionary<TKey, TCacheItem>> _groupedIndex = new();
    
    // Key -> Group mapping (for fast lookup)
    private readonly Dictionary<TKey, TGroupKey> _keyToGroupIndex = new();

    public ItemsCacheGroupedService(
        Func<TCacheItem, TGroupKey> keySelector,
        ILogger<ItemsCacheGroupedService<TKey, TCacheItem, TGroupKey>> logger)
    {
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<TCacheItem> GetByGroupKey(TGroupKey groupKey)
    {
        if (groupKey == null)
            throw new ArgumentNullException(nameof(groupKey));

        lock (_lockObject)
        {
            if (_groupedIndex.TryGetValue(groupKey, out var groupDict))
            {
                // Return a copy to prevent external modification
                return groupDict.Values.ToList();
            }
            
            return [];
        }
    }

    public bool HasGroup(TGroupKey groupKey)
    {
        if (groupKey == null)
            throw new ArgumentNullException(nameof(groupKey));

        lock (_lockObject)
        {
            return _groupedIndex.ContainsKey(groupKey);
        }
    }

    public int GetGroupCount()
    {
        lock (_lockObject)
        {
            return _groupedIndex.Count;
        }
    }

    public void OnCacheRefreshed(IEnumerable<KeyValuePair<TKey, TCacheItem>> items)
    {
        lock (_lockObject)
        {
            _logger.LogInformation(
                "ItemsCacheGroupedService {CacheItemType} Rebuilding grouped index from {ItemCount} items",
                typeof(TCacheItem).Name,
                items.Count());

            RebuildIndexes(items);
            
            _logger.LogInformation(
                "ItemsCacheGroupedService {CacheItemType} Successfully rebuilt grouped index with {GroupCount} groups",
                typeof(TCacheItem).Name,
                _groupedIndex.Count);
        }
    }

    public void OnItemUpdated(TKey key, TCacheItem item)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        lock (_lockObject)
        {
            // Remove item from old groups
            RemoveItemFromGroups(key);

            // Add item to new group
            var newGroupKey = _keySelector(item);
            AddItemToGroup(key, item, newGroupKey);
        }
    }

    public void OnItemDeleted(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        lock (_lockObject)
        {
            RemoveItemFromGroups(key);
        }
    }

    private void RebuildIndexes(IEnumerable<KeyValuePair<TKey, TCacheItem>> items)
    {
        _groupedIndex.Clear();
        _keyToGroupIndex.Clear();

        foreach (var (key, item) in items)
        {
            var groupKey = _keySelector(item);
            AddItemToGroup(key, item, groupKey);
        }
    }

    private void RemoveItemFromGroups(TKey key)
    {
        // Find which group this key belongs to (O(1) lookup)
        if (!_keyToGroupIndex.TryGetValue(key, out var groupKey))
            return;

        // Remove key from the group
        if (_groupedIndex.TryGetValue(groupKey, out var groupDict))
        {
            groupDict.Remove(key);
            TryRemoveEmptyGroup(groupKey, groupDict);
        }

        // Remove from reverse mapping
        _keyToGroupIndex.Remove(key);
    }

    private void AddItemToGroup(TKey key, TCacheItem item, TGroupKey groupKey)
    {
        if (!_groupedIndex.TryGetValue(groupKey, out var groupDict))
        {
            groupDict = new Dictionary<TKey, TCacheItem>();
            _groupedIndex[groupKey] = groupDict;
        }

        groupDict[key] = item;
        _keyToGroupIndex[key] = groupKey;
    }

    private void TryRemoveEmptyGroup(TGroupKey groupKey, Dictionary<TKey, TCacheItem> groupDict)
    {
        if (groupDict.Count == 0)
        {
            _groupedIndex.Remove(groupKey);
        }
    }
}


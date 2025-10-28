using System.Collections.Concurrent;
using ItemsCache.Core.Abstraction.Exceptions;
using ItemsCache.Core.Abstraction.Interfaces;

namespace ItemsCache.Core.Services
{
    internal sealed class ItemsCacheService<TCacheItem, TKey> : IItemsCacheServiceWithModifications<TCacheItem, TKey>
        where TCacheItem : class
        where TKey : notnull
    {
        private volatile bool _isInitialized;
        private ConcurrentDictionary<TKey, TCacheItem> _cache = new();

        public int Count => _cache.Count;

        public bool TryGetByKey(TKey key, out TCacheItem? item)
        {
            if (!_isInitialized)
                throw new CacheItemException("Cache is not initialized.");

            return _cache.TryGetValue(key, out item);
        }

        public IEnumerable<KeyValuePair<TKey, TCacheItem>> GetAll()
        {
            if (!_isInitialized)
                throw new CacheItemException("Cache is not initialized.");

            return _cache;
        }

        public bool TryRefresh(IEnumerable<KeyValuePair<TKey, TCacheItem>> items)
        {
            _cache = new ConcurrentDictionary<TKey, TCacheItem>(items);
            _isInitialized = true;
            return true;
        }

        public bool TrySet(TKey key, TCacheItem item)
        {
            var itemFromCache = _cache.AddOrUpdate(key, item, (_, _) => item);
            return itemFromCache.Equals(item);
        }

        public bool TryDelete(TKey key)
        {
            return _cache.TryRemove(key, out _);
        }
    }
}
using System.Collections.Concurrent;
using ItemsCache.Core.Abstraction.Exceptions;
using ItemsCache.Core.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Core.Services
{
    internal sealed class ItemsCacheService<TKey, TCacheItem> : IItemsCacheServiceWithModifications<TKey, TCacheItem>
        where TKey : notnull
        where TCacheItem : class
    {
        private volatile bool _isInitialized;
        private ConcurrentDictionary<TKey, TCacheItem> _cache = new();

        private readonly IEnumerable<ICacheUpdateObserver<TKey, TCacheItem>> _observers;
        private readonly ILogger<ItemsCacheService<TKey, TCacheItem>> _logger;

        public ItemsCacheService(
            IEnumerable<ICacheUpdateObserver<TKey, TCacheItem>> observers,
            ILogger<ItemsCacheService<TKey, TCacheItem>> logger)
        {
            _observers = observers;
            _logger = logger;
        }

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

            NotifyObserversRefreshed(items);

            return true;
        }

        public bool TrySet(TKey key, TCacheItem item)
        {
            var itemFromCache = _cache.AddOrUpdate(key, item, (_, _) => item);
            var result = itemFromCache.Equals(item);

            if (result)
            {
                NotifyObserversItemUpdated(key, item);
            }

            return result;
        }

        public bool TryDelete(TKey key)
        {
            var result = _cache.TryRemove(key, out _);

            if (result)
            {
                NotifyObserversItemDeleted(key);
            }

            return result;
        }

        private void NotifyObserversRefreshed(IEnumerable<KeyValuePair<TKey, TCacheItem>> items)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnCacheRefreshed(items);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error notifying observer of cache refresh");
                }
            }
        }

        private void NotifyObserversItemUpdated(TKey key, TCacheItem item)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnItemUpdated(key, item);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error notifying observer of item update");
                }
            }
        }

        private void NotifyObserversItemDeleted(TKey key)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnItemDeleted(key);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error notifying observer of item deletion");
                }
            }
        }
    }
}
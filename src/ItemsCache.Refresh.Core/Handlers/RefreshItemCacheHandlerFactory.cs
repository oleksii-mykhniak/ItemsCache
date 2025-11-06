using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class RefreshItemCacheHandlerFactory<TKey, TCacheItem> : IRefreshItemCacheHandlerFactory<TKey, TCacheItem>
        where TKey : notnull
    {
        private readonly Dictionary<RefreshCacheItemStatus, IRefreshItemCacheHandler<TKey, TCacheItem>> _handlers;

        public RefreshItemCacheHandlerFactory(IEnumerable<IRefreshItemCacheHandler<TKey, TCacheItem>> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.Status, h => h);
        }

        public IRefreshItemCacheHandler<TKey, TCacheItem> Create(RefreshCacheItemStatus status)
        {
            return _handlers.TryGetValue(status, out var handler) ? handler : throw new InvalidOperationException($"No handler found for status {status}");
        }
    }
}

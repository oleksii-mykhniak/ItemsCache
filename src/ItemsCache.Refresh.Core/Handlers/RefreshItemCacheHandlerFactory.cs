using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Abstraction.Models;

namespace ItemsCache.Refresh.Core.Handlers
{
    internal sealed class RefreshItemCacheHandlerFactory<TCacheItem, TKey> : IRefreshItemCacheHandlerFactory<TCacheItem, TKey>
        where TKey : notnull
    {
        private readonly Dictionary<RefreshCacheItemStatus, IRefreshItemCacheHandler<TCacheItem, TKey>> _handlers;

        public RefreshItemCacheHandlerFactory(IEnumerable<IRefreshItemCacheHandler<TCacheItem, TKey>> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.Status, h => h);
        }

        public IRefreshItemCacheHandler<TCacheItem, TKey> Create(RefreshCacheItemStatus status)
        {
            return _handlers.TryGetValue(status, out var handler) ? handler : throw new InvalidOperationException($"No handler found for status {status}");
        }
    }
}

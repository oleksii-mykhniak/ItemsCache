using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Polling.Abstraction;
using ItemsCache.Refresh.Polling.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Polling.Services
{
    internal sealed class ItemsCachePollingRefresher<TCacheItem, TKey, TRefreshContext> : IItemsCachePollingRefresher
        where TCacheItem : class
        where TKey : notnull
    {
        private readonly IDataSourceWithRefresh<TCacheItem, TKey, TRefreshContext> _dataSource;
        private readonly IRefreshItemCacheHandlerFactory<TCacheItem, TKey> _refreshItemCacheHandlerFactory;
        private readonly ILogger<ItemsCachePollingRefresher<TCacheItem, TKey, TRefreshContext>> _logger;

        private TRefreshContext? _lastRefreshContext;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public ItemsCachePollingRefresher(IDataSourceWithRefresh<TCacheItem, TKey, TRefreshContext> dataSource,
            IRefreshItemCacheHandlerFactory<TCacheItem, TKey> refreshItemCacheHandlerFactory,
            ILogger<ItemsCachePollingRefresher<TCacheItem, TKey, TRefreshContext>> logger)
        {
            _dataSource = dataSource;
            _refreshItemCacheHandlerFactory = refreshItemCacheHandlerFactory;
            _logger = logger;
        }
        
        public async Task<bool> RefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);

                var refreshResult = await _dataSource.GetUpdatedItemsAsync(_lastRefreshContext, cancellationToken);
                _lastRefreshContext = refreshResult.NewRefreshContext;

                foreach (var refreshCacheItem in refreshResult.Items)
                {
                    var handler = _refreshItemCacheHandlerFactory.Create(refreshCacheItem.Status);
                    await handler.HandleAsync(refreshCacheItem.Key, refreshCacheItem.Item, cancellationToken);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during auto-refresh");
                return false;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}

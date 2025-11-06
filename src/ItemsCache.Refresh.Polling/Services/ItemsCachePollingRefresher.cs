using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Polling.Abstraction;
using ItemsCache.Refresh.Polling.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Polling.Services
{
    internal sealed class ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext> : IItemsCachePollingRefresher
        where TKey : notnull
        where TCacheItem : class
    {
        private readonly IDataSourceWithRefresh<TKey, TCacheItem, TRefreshContext> _dataSource;
        private readonly IRefreshItemCacheHandlerFactory<TKey, TCacheItem> _refreshItemCacheHandlerFactory;
        private readonly ILogger<ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext>> _logger;

        private TRefreshContext? _lastRefreshContext;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public ItemsCachePollingRefresher(IDataSourceWithRefresh<TKey, TCacheItem, TRefreshContext> dataSource,
            IRefreshItemCacheHandlerFactory<TKey, TCacheItem> refreshItemCacheHandlerFactory,
            ILogger<ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext>> logger)
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

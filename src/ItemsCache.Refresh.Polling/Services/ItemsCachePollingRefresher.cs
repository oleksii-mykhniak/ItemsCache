using ItemsCache.Refresh.Abstraction.Interfaces;
using ItemsCache.Refresh.Polling.Abstraction;
using ItemsCache.Refresh.Polling.Abstraction.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ItemsCache.Refresh.Polling.Services
{
    internal sealed class ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext> : IItemsCachePollingRefresher
        where TKey : notnull
        where TCacheItem : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext>> _logger;

        private TRefreshContext? _lastRefreshContext;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public ItemsCachePollingRefresher(IServiceProvider serviceProvider,
            ILogger<ItemsCachePollingRefresher<TKey, TCacheItem, TRefreshContext>> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<bool> RefreshAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _semaphoreSlim.WaitAsync(cancellationToken);

                using var scope = _serviceProvider.CreateScope();
                var refreshItemCacheHandlerFactory = scope.ServiceProvider.GetRequiredService<IRefreshItemCacheHandlerFactory<TKey, TCacheItem>>();
                var dataSource = scope.ServiceProvider.GetRequiredService<IDataSourceWithRefresh<TKey, TCacheItem, TRefreshContext>>();
                var refreshResult = await dataSource.GetUpdatedItemsAsync(_lastRefreshContext, cancellationToken);
                _lastRefreshContext = refreshResult.NewRefreshContext;

                foreach (var refreshCacheItem in refreshResult.Items)
                {
                    var handler = refreshItemCacheHandlerFactory.Create(refreshCacheItem.Status);
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
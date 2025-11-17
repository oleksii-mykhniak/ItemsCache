using ItemsCache.Refresh.Polling.Abstraction;
using ItemsCache.Refresh.Polling.Abstraction.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ItemsCache.Refresh.Polling.Services
{
    internal sealed class ItemsCachePollingRefreshBackgroundService : BackgroundService
    {
        private readonly IEnumerable<IItemsCachePollingRefresher> _cachePollingRefreshers;
        private readonly IOptionsMonitor<ItemsCacheOptions> _optionsMonitor;
        private readonly ILogger<ItemsCachePollingRefreshBackgroundService> _logger;

        public ItemsCachePollingRefreshBackgroundService(IEnumerable<IItemsCachePollingRefresher> cachePollingRefreshers,
            IOptionsMonitor<ItemsCacheOptions> optionsMonitor,
            ILogger<ItemsCachePollingRefreshBackgroundService> logger)
        {
            _cachePollingRefreshers = cachePollingRefreshers;
            _optionsMonitor = optionsMonitor;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = _optionsMonitor.CurrentValue.RefreshInterval;

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Ignore cancellation exception
                }

                if (stoppingToken.IsCancellationRequested)
                    break;

                _logger.LogInformation("Items cache polling refresh started. Next refresh");
                var refreshTasks = _cachePollingRefreshers.Select(r => r.RefreshAsync(stoppingToken));
                await Task.WhenAll(refreshTasks);

                _logger.LogInformation("Items cache polling refresh completed. Next refresh");
            }
        }
    }
}
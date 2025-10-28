namespace ItemsCache.Refresh.Polling.Abstraction
{
    public interface IItemsCachePollingRefresher
    {
        Task<bool> RefreshAsync(CancellationToken cancellationToken);
    }
}

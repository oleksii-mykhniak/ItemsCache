namespace ItemsCache.Core.Interfaces
{
    internal interface IItemsCacheInitService
    {
        Task<bool> TryInitCacheAsync(CancellationToken ct);
    }
}

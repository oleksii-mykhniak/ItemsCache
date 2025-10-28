namespace ItemsCache.Core.Interfaces
{
    internal interface IItemsCacheLoader
    {
        Task<bool> LoadAsync(CancellationToken cancellationToken);
    }
}

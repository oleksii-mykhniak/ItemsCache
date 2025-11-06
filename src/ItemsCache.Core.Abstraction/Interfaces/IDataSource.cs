namespace ItemsCache.Core.Abstraction.Interfaces
{
    public interface IDataSource<TKey, TCacheItem>
        where TKey : notnull
        where TCacheItem : class
    {
        Task<IEnumerable<KeyValuePair<TKey, TCacheItem>>> LoadAllAsync(CancellationToken cancellationToken = default);
        
    }
}
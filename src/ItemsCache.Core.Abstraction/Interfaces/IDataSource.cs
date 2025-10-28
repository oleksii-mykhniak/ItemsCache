namespace ItemsCache.Core.Abstraction.Interfaces
{
    public interface IDataSource<TCacheItem, TKey>
        where TCacheItem : class
        where TKey : notnull
    {
        Task<IEnumerable<KeyValuePair<TKey, TCacheItem>>> LoadAllAsync(CancellationToken cancellationToken = default);
        
    }
}
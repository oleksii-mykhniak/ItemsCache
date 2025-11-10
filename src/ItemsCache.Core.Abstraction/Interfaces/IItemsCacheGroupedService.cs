namespace ItemsCache.Core.Abstraction.Interfaces;

public interface IItemsCacheGroupedService<TCacheItem, TGroupKey> where TGroupKey : notnull
{
    IEnumerable<TCacheItem> GetByGroupKey(TGroupKey groupKey);
    
    bool HasGroup(TGroupKey groupKey);
    
    int GetGroupCount();
}



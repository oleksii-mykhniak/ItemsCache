namespace ItemsCache.Refresh.Abstraction.Models;

public class RefreshCacheItem<TKey, TCacheItem>
    where TKey : notnull
    where TCacheItem : class
{
    public TKey Key { get; }

    public TCacheItem Item { get; }

    public RefreshCacheItemStatus Status { get; }

    public RefreshCacheItem(TKey key, TCacheItem item, RefreshCacheItemStatus status)
    {
        Key = key;
        Item = item;
        Status = status;
    }
}

using ItemsCache.Core.Abstraction.Interfaces;

namespace ItemsCache.Core.Abstraction.Extensions;

public static class ItemsCacheServiceExtensions
{
    public static TCacheItem GetOrDefault<TKey, TCacheItem>(this IItemsCacheService<TKey, TCacheItem> itemsCacheService, TKey key)
        where TKey : notnull
    {
        return itemsCacheService.TryGetByKey(key, out var item) ? item! : default!;
    }
}
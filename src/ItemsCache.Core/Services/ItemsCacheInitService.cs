using ItemsCache.Core.Interfaces;

namespace ItemsCache.Core.Services;

internal sealed class ItemsCacheInitService : IItemsCacheInitService
{
    private readonly IEnumerable<IItemsCacheLoader> _itemsCacheInitializers;

    public ItemsCacheInitService(IEnumerable<IItemsCacheLoader> itemsCacheInitializers)
    {
        _itemsCacheInitializers = itemsCacheInitializers;
    }

    public async Task<bool> TryInitCacheAsync(CancellationToken ct)
    {
        var result = await Task.WhenAll(RunInvalidationTasks(ct));
        return result.All(x => x);
    }

    private IEnumerable<Task<bool>> RunInvalidationTasks(CancellationToken ct)
    {
        return _itemsCacheInitializers.Select(loader => loader.LoadAsync(ct));
    }
}
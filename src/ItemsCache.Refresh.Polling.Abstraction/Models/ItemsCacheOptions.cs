namespace ItemsCache.Refresh.Polling.Abstraction.Models;

public class ItemsCacheOptions
{
    public TimeSpan RefreshInterval { get; set; } = TimeSpan.FromSeconds(30);
}
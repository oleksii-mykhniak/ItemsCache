using ItemsCache.Core.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using SampleApi.Data;
using SampleApi.Models;

namespace SampleApi.Services;

public class DataFromDbSource : IDataSource<Product, int>
{
    private readonly AppDbContext _dbContext;

    public DataFromDbSource(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<KeyValuePair<int, Product>>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _dbContext.Set<Product>().ToListAsync(cancellationToken);
        return products.ToDictionary(p => p.Id);
    }
}
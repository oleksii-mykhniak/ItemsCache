using ItemsCache.Core.Abstraction.Interfaces;
using ItemsCache.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using SampleApi.Data;
using SampleApi.Models;
using ItemsCache.Refresh.Polling.Abstraction.Interfaces;
using ItemsCache.Refresh.Polling.Extensions;
using SampleApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

builder.Services.AddItemsCache<Product, int>();

// without retry policy
builder.Services.AddScoped<IDataSource<Product, int>, DataFromDbSource>();

// with retry policy
// builder.Services.AddScoped<IDataSource<Product, int>>(sp =>
// {
//     var logger = sp.GetRequiredService<ILogger<RetryDataSourceDecorator<Product, int>>>();
//
//     return new RetryDataSourceDecorator<Product, int>(
//         new DataFromDbSource(sp.GetRequiredService<AppDbContext>()),
//         new RetryPolicyWithPolly(new RetryOptions { }, logger, "DataFromDbSource"),
//         logger);
// });

//with polling refresh
builder.Services.AddPollingRefreshItemCacheHandler<Product, int, RefreshContext>(builder.Configuration);

// Data source without retry policy
builder.Services.AddScoped<IDataSourceWithRefresh<Product, int, RefreshContext>, DataFromDbSourceWithRefresh>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Seed data if empty
    if (!context.Products.Any())
    {
        var products = new List<Product>
        {
            new Product { Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Category = "Electronics", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Category = "Electronics", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m, Category = "Electronics", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Monitor", Description = "4K monitor", Price = 299.99m, Category = "Electronics", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Desk", Description = "Standing desk", Price = 199.99m, Category = "Furniture", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Chair", Description = "Ergonomic chair", Price = 149.99m, Category = "Furniture", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Book", Description = "Programming book", Price = 19.99m, Category = "Books", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Pen", Description = "Blue ink pen", Price = 2.99m, Category = "Office Supplies", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Notebook", Description = "Spiral notebook", Price = 4.99m, Category = "Office Supplies", UpdatedAt = DateTime.UtcNow },
            new Product { Name = "Coffee Mug", Description = "Ceramic coffee mug", Price = 8.99m, Category = "Kitchen", UpdatedAt = DateTime.UtcNow }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}

app.Run();
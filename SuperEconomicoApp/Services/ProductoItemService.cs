using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using SuperEconomicoApp.Model;
using System.Linq;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace SuperEconomicoApp.Services
{
    public class ProductoItemService
    { 

        FirebaseClient client;
    public ProductoItemService()
    {
        client = new FirebaseClient("https://supereconomicoapp-default-rtdb.firebaseio.com/");
    }

    public async Task<List<ProductoItem>> GetProductoItemsAsync()
    {
        var products = (await client.Child("ProductoItems")
             .OnceAsync<ProductoItem>())
             .Select(f => new ProductoItem
             {
                 CategoryID = f.Object.CategoryID,
                 Description = f.Object.Description,
                 HomeSelected = f.Object.HomeSelected,
                 ImageUrl = f.Object.ImageUrl,
                 Name = f.Object.Name,
                 Price = f.Object.Price,
                 ProductID = f.Object.ProductID,
                 Rating = f.Object.Rating,
                 RatingDetail = f.Object.RatingDetail
             }).ToList();
        return products;
    }

    public async Task<ObservableCollection<ProductoItem>> GetProductoItemsByCategoryAsync(int categoryID)
    {
        var productoItemsByCategory = new ObservableCollection<ProductoItem>();
        var items = (await GetProductoItemsAsync()).Where(p => p.CategoryID == categoryID).ToList();
        foreach (var item in items)
        {
                productoItemsByCategory.Add(item);
        }
        return productoItemsByCategory;
    }

    public async Task<ObservableCollection<ProductoItem>> GetLatestProductoItemsAsync()
    {
        var latestProductoItems = new ObservableCollection<ProductoItem>();
        var items = (await GetProductoItemsAsync()).OrderByDescending(f => f.ProductID).Take(8);
        foreach (var item in items)
        {
                latestProductoItems.Add(item);
        }
        return latestProductoItems;
    }

    public async Task<ObservableCollection<ProductoItem>> GetProductoItemsByQueryAsync(string searchText)
    {
        var productoItemsByQuery = new ObservableCollection<ProductoItem>();
        var items = (await GetProductoItemsAsync()).Where(p => p.Name.Contains(searchText)).ToList();
        foreach (var item in items)
        {
                productoItemsByQuery.Add(item);
        }
        return productoItemsByQuery;
    }
}
}

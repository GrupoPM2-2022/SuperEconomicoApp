using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperEconomicoApp.Model;
using System.Linq;
using System.Collections.ObjectModel;

namespace SuperEconomicoApp.Services
{
    public class ProductoItemService
    {

        public Task<List<ProductoItem>> GetProductoItemsAsync()
        {
            //var products = (await client.Child("ProductoItems")
            //     .OnceAsync<ProductoItem>())
            //     .Select(f => new ProductoItem
            //     {
            //         Status = f.Object.Status,
            //         Description = f.Object.Description,
            //         Discount = f.Object.Discount,
            //         Image = f.Object.Image,
            //         Name = f.Object.Name,
            //         Price = f.Object.Price,
            //         Product_Id = f.Object.Product_Id,
            //         Code = f.Object.Code,
            //         Stock = f.Object.Stock
            //     }).ToList();
            //return products;

            return null;
        }

        public Task<ObservableCollection<ProductoItem>> GetProductoItemsByCategoryAsync(int categoryID)
        {
            //var productoItemsByCategory = new ObservableCollection<ProductoItem>();
            //var items = (await GetProductoItemsAsync()).Where(p => p.Status == categoryID).ToList();
            //foreach (var item in items)
            //{
            //        productoItemsByCategory.Add(item);
            //}
            //return productoItemsByCategory;
            return null;
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

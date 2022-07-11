using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperEconomicoApp.Model;
using System.Linq;

namespace SuperEconomicoApp.Services
{
    public class CategoryDataService
    {
        

        public Task<List<Category>> GetCategoriesAsync()
        {
            //var categories = (await client.Child("Categories")
            //    .OnceAsync<Category>())
            //    .Select(c => new Category
            //    {
            //        CategoryID = c.Object.CategoryID,
            //        CategoryName = c.Object.CategoryName,
            //        CategoryPoster = c.Object.CategoryPoster,
            //        ImageUrl = c.Object.ImageUrl
            //    }).ToList();
            return null;
        }
    }
}


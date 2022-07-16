using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.Helpers
{
    public class AddCategoryData
    {
        public List<Category> Categories { get; set; }

        public AddCategoryData()
        {
        }

        public Task AddCategoriesAsync()
        {
            return null;
        }
    }
}

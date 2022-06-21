using SuperEconomicoApp.Model;
using System;
using System.Collections.ObjectModel;
using System.Text;
using SuperEconomicoApp.Services;

namespace SuperEconomicoApp.ViewsModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private Category _SelectedCategory;
        public Category SelectedCategory
        {
            set
            {
                _SelectedCategory = value;
                OnPropertyChanged();
            }

            get
            {
                return _SelectedCategory;
            }

    }
        public ObservableCollection<ProductoItem> ProductoItemsByCategory { get; set; }
        private int _TotalProductoItems;
        public int TotalProductoItems
        {
            set
            {
                _TotalProductoItems = value;
                OnPropertyChanged();
            }

            get
            {
                return _TotalProductoItems;
            }
        }
        public CategoryViewModel(Category category)
        {
            SelectedCategory = category;
            ProductoItemsByCategory = new ObservableCollection<ProductoItem>();
            GetProductoItems(category.CategoryID);
        }
        private async void GetProductoItems(int categoryID)
        {
            var data = await new ProductoItemService().GetProductoItemsByCategoryAsync(categoryID);
            ProductoItemsByCategory.Clear();
            foreach (var item in data)
            {
                ProductoItemsByCategory.Add(item);
            }
            TotalProductoItems = ProductoItemsByCategory.Count;
        }

    }
}

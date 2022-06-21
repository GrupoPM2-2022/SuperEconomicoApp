using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SuperEconomicoApp.ViewsModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public ObservableCollection<ProductoItem> ProductoItemsByQuery { get; set; }

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

        public SearchResultsViewModel(string searchText)
        {
            ProductoItemsByQuery = new ObservableCollection<ProductoItem>();
            GetProductoItemsByQuery(searchText);
        }

        private async void GetProductoItemsByQuery(string searchText)
        {
            var data = await new ProductoItemService().GetProductoItemsByQueryAsync(searchText);
            ProductoItemsByQuery.Clear();
            foreach (var item in data)
            {
                ProductoItemsByQuery.Add(item);
            }
            TotalProductoItems = ProductoItemsByQuery.Count;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperEconomicoApp.Api;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private string _UserName;
        public string UserName
        {
            set
            {
                _UserName = value;
                OnPropertyChanged();
            }

            get
            {
                return _UserName;
            }
        }

        private int _UserCartItemsCount;
        public int UserCartItemsCount
        {
            set
            {
                _UserCartItemsCount = value;
                OnPropertyChanged();
            }

            get
            {
                return _UserCartItemsCount;
            }
        }

        private string _SearchText;
        public string SearchText
        {
            set
            {
                _SearchText = value;
                OnPropertyChanged();
            }

            get
            {
                return _SearchText;
            }
        }

        public ObservableCollection<Category> Categories { get; set; }
        private ObservableCollection<ProductoItem> _ListItemsProducts;

        public Command ViewCartCommand { get; set; }
        public Command LogoutCommand { get; set; }
        public Command OrdersHistoryCommand { get; set; }
        public Command SearchViewCommand { get; set; }

        public ObservableCollection<ProductoItem> ListItemsProducts {
            get { return _ListItemsProducts; }
            set
            {
                _ListItemsProducts = value;
                OnPropertyChanged("ListItemsProducts");
            }
        }

        public ProductsViewModel()
        {
            if (Settings.ExistUser)
            {
                UserName = Settings.UserName;
            }
            else
            {
                UserName = "Usuario";
            }

            UserCartItemsCount = new CartItemService().GetUserCartCount();
            ListItemsProducts = new ObservableCollection<ProductoItem>();
            Categories = new ObservableCollection<Category>();

            ViewCartCommand = new Command(async () => await ViewCartAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
            OrdersHistoryCommand = new Command(async () => await OrderHistoryAsync());
            SearchViewCommand = new Command(async () => await SearchViewAsync());

            GetCategories();
            GetAllProducts();
        }

        private async Task SearchViewAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(
            new SearchResultsView(SearchText));
        }

        private async Task OrderHistoryAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new OrdersHistoryView());
        }

        private async Task ViewCartAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CartView());
        }

        private async Task LogoutAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new LogoutView());
        }

        private async void GetCategories()
        {
            var data = await new CategoryDataService().GetCategoriesAsync();
            Categories.Clear();
            foreach (var item in data)
            {
                Categories.Add(item);
            }
        }

        private async void GetAllProducts()
        {
            //ListItemsProducts.Clear();
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(ApiMethods.URL_PRODUCTS);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                ListItemsProducts = JsonConvert.DeserializeObject<ObservableCollection<ProductoItem>>(content);
            }

        }

    }
}


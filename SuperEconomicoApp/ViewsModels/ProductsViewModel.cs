using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using SuperEconomicoApp.Api;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Ubication;
using SuperEconomicoApp.Views.Reusable;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private string _UserName;
        private string _DepartamentPreview;
        private string _Coordinates;
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
        public string DepartamentPreview
        {
            set
            {
                _DepartamentPreview = value;
                OnPropertyChanged();
            }

            get
            {
                return _DepartamentPreview;
            }
        }
        public string Coordinates
        {
            set
            {
                _Coordinates = value;
                OnPropertyChanged();
            }

            get
            {
                return _Coordinates;
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
        private List<Department> _ListDepartment;

        public ObservableCollection<ProductoItem> ListItemsProducts
        {
            get { return _ListItemsProducts; }
            set
            {
                _ListItemsProducts = value;
                OnPropertyChanged("ListItemsProducts");
            }
        }
        public List<Department> ListDepartment
        {
            get { return _ListDepartment; }
            set
            {
                _ListDepartment = value;
                OnPropertyChanged("ListDepartment");
            }
        }

        public Command ViewCartCommand { get; set; }
        public Command LogoutCommand { get; set; }
        public Command OrdersHistoryCommand { get; set; }
        public Command SearchViewCommand { get; set; }
        public Command SelectDeparmentCommand { get; set; }
        public Command ConfirmDepartmentCommand { get; set; }
        public Command ViewDirectionCommand { get; set; }

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
            SelectDeparmentCommand = new Command<Department>( (param) => SelectDeparment(param));
            ConfirmDepartmentCommand = new Command(ConfirmDepartment);
            ViewDirectionCommand = new Command(async () => await ViewDirection());

            //GetCategories();
            ConfigurationDepartment();
            GetAllProducts();
        }

        private async Task ViewDirection()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ListDirectionView());
        }

        private void ConfirmDepartment()
        {
            Settings.Department = DepartamentPreview;
            Settings.Coordinates = Coordinates;
            PopupNavigation.Instance.PopAsync();
        }

        private void SelectDeparment(Department department)
        {
            Coordinates = department.Latitude.ToString() + "," + department.Longitude.ToString();
            DepartamentPreview = department.Name;
        }

        private void ConfigurationDepartment()
        {
            if (!Settings.ExistDepartment)
            {
                var popup = new SelectDepartment();
                popup.BindingContext = this;
                LoadListDepartment();
                PopupNavigation.Instance.PushAsync(popup);
            }
        }

        private void LoadListDepartment()
        {
            ListDepartment = new List<Department>()
            {
                new Department()
                {
                    Id = 1,
                    Name = "San Pedro Sula",
                    Latitude = 15.533129694846346,
                    Longitude = -88.03389851150737
                },
                new Department()
                {
                    Id = 2,
                    Name = "Tegucigalpa",
                    Latitude = 14.078002052781091,
                    Longitude = -87.19175533457481
                }
            };
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


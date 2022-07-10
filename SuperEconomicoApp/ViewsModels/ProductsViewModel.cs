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
        private GoogleDistanceMatrix googleDistanceMatrix;
        private GoogleServiceApi googleServiceApi;
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
        public Command EditUserCommand { get; set; }
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
            googleDistanceMatrix = new GoogleDistanceMatrix();
            googleServiceApi = new GoogleServiceApi();

            ViewCartCommand = new Command(async () => await ViewCartAsync());
            EditUserCommand = new Command(async () => await EditUserAsync());
            OrdersHistoryCommand = new Command(async () => await OrderHistoryAsync());
            SearchViewCommand = new Command(async () => await SearchViewAsync());
            SelectDeparmentCommand = new Command<Department>((param) => SelectDeparment(param));
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

        private async void ConfirmDepartment()
        {
            if (string.IsNullOrEmpty(DepartamentPreview))
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes seleccionar tu departamento actual.", "Ok");
                return;
            }
            var statusCheck = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (statusCheck == PermissionStatus.Granted)
            {
                IsValidDistance();
            }
            else {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes conceder permisos de localizacion a la aplicacion.", "Ok");
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
        }

        private async void IsValidDistance()
        {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                string coordinatesUser = location.Latitude.ToString() + "," + location.Longitude.ToString();
                googleDistanceMatrix = await googleServiceApi.CalculateDistanceTwoCoordinates(Coordinates, coordinatesUser);

                int meters = googleDistanceMatrix.rows[0].elements[0].distance.value;

                double kilometers = meters / 1000;
                if (kilometers > Constants.VALID_KILOMETERS)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual", "Ok");
                }
                else
                {
                    Settings.Department = DepartamentPreview;
                    Settings.Coordinates = Coordinates;
                    await PopupNavigation.Instance.PopAsync();
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "No podemos obtener tu localizacion actual.", "Ok");
            }
        }

        private void SelectDeparment(Department department)
        {
            Coordinates = department.Latitude.ToString() + "," + department.Longitude.ToString();
            DepartamentPreview = department.Name;
        }

        private async void ConfigurationDepartment()
        {
            if (!Settings.ExistDepartment)
            {
                var popup = new SelectDepartment();
                popup.BindingContext = this;
                LoadListDepartment();
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                await PopupNavigation.Instance.PushAsync(popup);
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
                    Latitude = 15.494080,
                    Longitude = -88.033686
                },
                new Department()
                {
                    Id = 2,
                    Name = "Tegucigalpa",
                    Latitude = 14.088415,
                    Longitude = -87.184149
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
            await Application.Current.MainPage.Navigation.PushModalAsync(new TabbedOrdersView());
            
        }

        private async Task ViewCartAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CartView());
        }

        private async Task EditUserAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AccountUserView());
        }

        //private async Task LogoutAsync()
        //{
        //    await Application.Current.MainPage.Navigation.PushModalAsync(new LogoutView());
        //}

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


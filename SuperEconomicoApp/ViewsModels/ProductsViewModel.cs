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
using System.Linq;
using System.Windows.Input;

namespace SuperEconomicoApp.ViewsModels
{
    public class ProductsViewModel : BaseViewModel
    {
        #region Variables
        private ObservableCollection<ProductoItem> _ListItemsProducts;
        private ObservableCollection<Category> _ListCategories;
        private List<Department> _ListDepartment;
        private string _UserName;
        private string _DepartamentPreview;
        private string _Coordinates;
        private string _SearchText;
        private int _UserCartItemsCount;
        private bool _IsVisibleEmptyMessage = false;
        private bool _IsVisibleProducts = true;

        private GoogleDistanceMatrix googleDistanceMatrix;
        private GoogleServiceApi googleServiceApi;
        CollectionView collectionView;
        #endregion

        #region Objetos
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

        public bool IsVisibleProducts
        {
            set
            {
                _IsVisibleProducts = value;
                OnPropertyChanged();
            }

            get
            {
                return _IsVisibleProducts;
            }
        }
        
        public bool IsVisibleEmptyMessage
        {
            set
            {
                _IsVisibleEmptyMessage = value;
                OnPropertyChanged();
            }

            get
            {
                return _IsVisibleEmptyMessage;
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

        public ObservableCollection<ProductoItem> ListItemsProducts
        {
            get { return _ListItemsProducts; }
            set
            {
                _ListItemsProducts = value;
                OnPropertyChanged("ListItemsProducts");
            }
        }

        public ObservableCollection<Category> ListCategories
        {
            get { return _ListCategories; }
            set
            {
                _ListCategories = value;
                OnPropertyChanged("ListCategories");
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

        #endregion

        public ProductsViewModel(CollectionView collectionViewReceived)
        {
            UserCartItemsCount = new CartItemService().GetUserCartCount();
            ListItemsProducts = new ObservableCollection<ProductoItem>();
            ListCategories = new ObservableCollection<Category>();
            googleDistanceMatrix = new GoogleDistanceMatrix();
            googleServiceApi = new GoogleServiceApi();
            collectionView = collectionViewReceived;

            GetCategories();
            GetAllProducts();
            ConfigurationDepartment();
        }

        #region Procesos
        private void SearchProduct()
        {
            var searchResult = ListItemsProducts.Where(item => item.Name.ToUpper().Contains(SearchText.ToUpper()));
            collectionView.ItemsSource = searchResult;
        }

        private void SelectCategory(Category category)
        {
            if (category.Name.Equals("Todos"))
            {
                collectionView.ItemsSource = ListItemsProducts;
            }
            else
            {
                var searchbyCategory = ListItemsProducts.Where(item => item.Category_Id.ToString().Contains(category.CategoryID.ToString()));

                if (searchbyCategory.Count() == 0)
                {
                    IsVisibleEmptyMessage = true;
                    IsVisibleProducts = false;
                } else
                {
                    IsVisibleEmptyMessage = false;
                    IsVisibleProducts = true;
                    collectionView.ItemsSource = searchbyCategory;
                }

            }

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
            else
            {
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

        private async void GetCategories()
        {
            var categories = await CategoryService.GetAllCategories();
            if (categories != null)
            {
                ListCategories = categories;
            }
        }

        private async void GetAllProducts()
        {
            var listProducts = await ProductoService.GetAllProducts();
            if (listProducts != null)
            {
                ListItemsProducts = listProducts;
                collectionView.ItemsSource = ListItemsProducts;
            }

        }
        #endregion

        #region Comandos
        public ICommand ViewCartCommand => new Command(async () => await ViewCartAsync());
        public ICommand EditUserCommand => new Command(async () => await EditUserAsync());
        public ICommand OrdersHistoryCommand => new Command(async () => await OrderHistoryAsync());
        public ICommand SelectDeparmentCommand => new Command<Department>((param) => SelectDeparment(param));
        public ICommand ConfirmDepartmentCommand => new Command(ConfirmDepartment);
        public ICommand ViewDirectionCommand => new Command(async () => await ViewDirection());
        public ICommand SearchProductCommand => new Command(SearchProduct);
        public ICommand SelectCategoryCommand => new Command<Category>((Category) => SelectCategory(Category));

        #endregion

    }
}


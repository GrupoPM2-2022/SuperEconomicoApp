using Rg.Plugins.Popup.Services;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using SuperEconomicoApp.Views.Ubication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class ConfirmOrderViewModel : BaseViewModel
    {
        #region Variables
        public ObservableCollection<UserCartItem> _ListProductsOrdered;
        public ObservableCollection<Direction> _ListDirection;
        public List<OrderDetails> ListOrders { get; set; }
        public Order SelectedOrder { get; set; }
        public CartItemService CartItemService { get; set; }
        private UserCartItem _SelectedProductoItem;
        public DirectionService DirectionServiceObject { get; set; }
        GoogleServiceApi googleServiceApi;
        GoogleDistanceMatrix googleDistanceMatrix;

        private double _Total;
        private string _TotalPrincipal;
        private string _UbicationPreview;
        private int _TotalQuantity;
        #endregion

        #region OBJETOS
        public double Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                OnPropertyChanged();
            }
        }
        
        public string TotalPrincipal
        {
            get { return _TotalPrincipal; }
            set
            {
                _TotalPrincipal = value;
                OnPropertyChanged();
            }
        }

        public string UbicationPreview
        {
            get { return _UbicationPreview; }
            set
            {
                _UbicationPreview = value;
                OnPropertyChanged();
            }
        }

        public UserCartItem SelectedProductoItem
        {
            get { return _SelectedProductoItem; }
            set
            {
                _SelectedProductoItem = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<UserCartItem> ListProductsOrdered
        {
            get { return _ListProductsOrdered; }
            set
            {
                _ListProductsOrdered = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Direction> ListDirection
        {
            get { return _ListDirection; }
            set
            {
                _ListDirection = value;
                OnPropertyChanged();
            }
        }

        public int TotalQuantity
        {
            set
            {
                _TotalQuantity = value;
                if (_TotalQuantity <= 0)
                    _TotalQuantity = 1;
                if (SelectedProductoItem != null)
                {
                    if (_TotalQuantity > SelectedProductoItem.Stock)
                    {
                        Application.Current.MainPage.DisplayAlert("Advertencia", "Este producto alcanzo su limite en stock", "Ok");
                        _TotalQuantity -= 1;
                    }
                }
                OnPropertyChanged();
            }
            get
            {
                return _TotalQuantity;
            }
        }
        #endregion

        public ConfirmOrderViewModel(ObservableCollection<UserCartItem> listOrderDetails, Order order)
        {
            ListProductsOrdered = listOrderDetails;
            SelectedOrder = order;
            CartItemService = new CartItemService();
            DirectionServiceObject = new DirectionService();
            ListDirection = new ObservableCollection<Direction>();
            googleServiceApi = new GoogleServiceApi();
            googleDistanceMatrix = new GoogleDistanceMatrix();

            LoadConfiguration();
        }

        #region Procesos
        private async Task AddDirection()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddDirectionView("Guardar", null));
        }

        private async Task AddDirectionCurrent()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                CreateDirection();
            }
            else
            {
                var request = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (request == PermissionStatus.Granted)
                {
                    CreateDirection();
                }
            }
        }

        public async Task<bool> IsValidCoverageRange() {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                string coordinatesUser = location.Latitude.ToString() + "," + location.Longitude.ToString();
                googleDistanceMatrix = await googleServiceApi.CalculateDistanceTwoCoordinates(Settings.Coordinates, coordinatesUser);

                int meters = googleDistanceMatrix.rows[0].elements[0].distance.value;
                
                double kilometers = meters / 1000;
                if (kilometers <= Constants.VALID_KILOMETERS)
                {
                    UbicationPreview = googleDistanceMatrix.destination_addresses[0];
                    SelectedOrder.client_location = location.Latitude.ToString() + "," + location.Longitude.ToString();
                    return true;
                }
                
                return false;
            }

            return false;
        }

        public async void CreateDirection()
        {
            //string[] coordinates = SelectedOrder.client_location.Split(',');
            bool isValid = await IsValidCoverageRange();
            if (!isValid)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual, trabajaremos en eso pronto", "Ok");
                return;
            }

            //Direction direction = new Direction
            //{
            //    description = UbicationPreview,
            //    latitude = coordinates[0],
            //    longitude = coordinates[1],
            //    id_user = Convert.ToInt32(Settings.IdUser)
            //};

            //bool response = await DirectionServiceObject.CreateDirection(direction);
            //if (response)
            //{
            //    await Application.Current.MainPage.DisplayAlert("Confirmación", "Dirección agregada correctamente", "Ok");
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener su dirección actual", "Ok");
            //}
        }

        private void IncreaseQuantity(UserCartItem userCartItem)
        {
            var popup = new IncreaseQuantity();
            popup.BindingContext = this;
            SelectedProductoItem = userCartItem;
            TotalQuantity = SelectedProductoItem.Quantity;
            PopupNavigation.Instance.PushAsync(popup);
        }

        private async Task DeleteProduct(UserCartItem userCartItem)
        {
            bool response = await Application.Current.MainPage.DisplayAlert("Advertencia", "¿Está seguro de remover el producto " + userCartItem.ProductName + "?", "Si", "No");
            if (response)
            {
                ListProductsOrdered.Remove(userCartItem);
                CartItemService.RemoveProductById(userCartItem);
                UpdateProductCart();
            }
        }

        private void SelectLocation(Direction direction)
        {
            SelectedOrder.client_location = direction.latitude.ToString() + "," + direction.longitude.ToString();
            UbicationPreview = direction.description;
        }

        private async void SaveOrder()
        {
            try
            {
                if (ListProductsOrdered.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "No hay productos para procesar su orden.", "Ok");
                    return;
                }
                if (ListDirection.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "No tienes ubicaciones registradas, agrega una ubicación.", "Ok");
                    return;
                }

                if (string.IsNullOrEmpty(SelectedOrder.client_location))
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes seleccionar la ubicación.", "Ok");
                    return;
                }

                FillOrderList();
                var response = await new OrderService().CreateOrder(SelectedOrder);
                if (response)
                {
                    await Application.Current.MainPage.DisplayAlert("Confirmacion", "Pedido realizado exitosamente.", "Ok");
                    await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al insertar su pedido.", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void FillOrderList()
        {
            ListOrders = new List<OrderDetails>();
            foreach (var item in ListProductsOrdered)
            {
                OrderDetails orderDetail = new OrderDetails()
                {
                    product_id = item.ProductId,
                    quantity = item.Quantity,
                    price = item.Price,
                    discount = 0,
                    name_product = item.ProductName
                };

                ListOrders.Add(orderDetail);
            }

            SelectedOrder.orders_detail = ListOrders;
            SelectedOrder.sucursal = Settings.Coordinates;
            SelectedOrder.order_date = DateTime.Now;
            SelectedOrder.deliver_date = DateTime.Now;
        }

        private void LoadConfiguration()
        {
            TotalPrincipal = SelectedOrder.total.ToString("F", CultureInfo.InvariantCulture);
            //Total = Math.Round(, 2);
            GetAllDirections();
        }

        public async void GetAllDirections()
        {
            ListDirection = await DirectionServiceObject.GetDirectionByUser();
        }

        private async Task DeleteOrder()
        {
            var response = await Application.Current.MainPage.DisplayAlert("Aviso", "¿Está seguro de cancelar su orden?", "Si", "No");
            if (response)
            {
                CartItemService.RemoveItemsFromCart();
                await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());
            }
        }

        private void AddToCart()
        {
            try
            {
                CartItemService.AddProductTocart(SelectedProductoItem, TotalQuantity);
                UpdateProductCart();
                TotalQuantity = 1;
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void UpdateProductCart()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var items = cn.Table<CartItem>().ToList();
            ListProductsOrdered.Clear();
            Total = 0;
            foreach (var item in items)
            {
                ListProductsOrdered.Add(new UserCartItem()
                {
                    CartItemId = item.CartItemId,
                    ImageProduct = item.ImageProduct,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Description = item.Description,
                    Price = Math.Round(item.Price, 2),
                    Quantity = item.Quantity,
                    Cost = item.Price * item.Quantity,
                    Stock = item.Stock
                });
                Total += (item.Price * item.Quantity);
            }

            //Total = Math.Round(Total, 2);
            TotalPrincipal = Total.ToString("F", CultureInfo.InvariantCulture);
        }


        private void DecrementOrder()
        {
            TotalQuantity--;
        }

        private void IncrementOrder()
        {
            TotalQuantity++;
        }

        #endregion

        #region Comandos
        public ICommand DeleteOrderCommand => new Command(async () => await DeleteOrder());
        public ICommand SaveOrderCommand => new Command(SaveOrder);
        public ICommand SelectLocationCommand => new Command<Direction>((Direction) => SelectLocation(Direction));
        public ICommand DeleteProductCommand => new Command<UserCartItem>(async (UserCartItem) => await DeleteProduct(UserCartItem));
        public ICommand IncreaseQuantityCommand => new Command<UserCartItem>((UserCartItem) => IncreaseQuantity(UserCartItem));
        public ICommand IncrementOrderCommand => new Command(() => IncrementOrder());
        public ICommand DecrementOrderCommand => new Command(() => DecrementOrder());
        public ICommand AddToCartCommand => new Command(() => AddToCart());
        public ICommand AddDirectionCommand => new Command(async () => await AddDirection());
        public ICommand AddDirectionCurrentCommand => new Command(async () => await AddDirectionCurrent());

        #endregion
    }
}

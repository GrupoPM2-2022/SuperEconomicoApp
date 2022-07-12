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
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class ConfirmOrderViewModel : BaseViewModel
    {
        public ObservableCollection<UserCartItem> _ListProductsOrdered;
        public Order SelectedOrder { get; set; }
        public ObservableCollection<Direction> _ListDirection;
        public List<OrderDetails> ListOrders { get; set; }
        public CartItemService CartItemService { get; set; }
        private UserCartItem _SelectedProductoItem;
        public DirectionService DirectionServiceObject { get; set; }

        private double _Total;
        private string _UbicationPreview;
        private int _TotalQuantity;

        public Command DeleteOrderCommand { get; set; }
        public Command SaveOrderCommand { get; set; }
        public Command SelectLocationCommand { get; set; }
        public Command DeleteProductCommand { get; set; }
        public Command IncreaseQuantityCommand { get; set; }
        public Command IncrementOrderCommand { get; set; }
        public Command DecrementOrderCommand { get; set; }
        public Command AddToCartCommand { get; set; }
        public Command AddDirectionCommand { get; set; }

        public double Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
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


        public ConfirmOrderViewModel(ObservableCollection<UserCartItem> listOrderDetails, Order order)
        {
            ListProductsOrdered = listOrderDetails;
            SelectedOrder = order;
            CartItemService = new CartItemService();
            DirectionServiceObject = new DirectionService();
            ListDirection = new ObservableCollection<Direction>();
            LoadConfiguration();

            // COMANDOS
            DeleteOrderCommand = new Command(async () => await DeleteOrder());
            SaveOrderCommand = new Command(SaveOrder);
            SelectLocationCommand = new Command<Direction>((Direction) => SelectLocation(Direction));
            DeleteProductCommand = new Command<UserCartItem>(async (UserCartItem) => await DeleteProduct(UserCartItem));
            IncreaseQuantityCommand = new Command<UserCartItem>((UserCartItem) => IncreaseQuantity(UserCartItem));

            IncrementOrderCommand = new Command(() => IncrementOrder());
            DecrementOrderCommand = new Command(() => DecrementOrder());
            AddToCartCommand = new Command(() => AddToCart());
            AddDirectionCommand = new Command(async () => await AddDirection());
        }

        private async Task AddDirection()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new AddDirectionView("Guardar", null));
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
        }

        private void LoadConfiguration()
        {
            Total = Math.Round(SelectedOrder.total, 2);
            GetAllDirections();
        }

        public async void GetAllDirections() {
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

            Total = Math.Round(Total, 2);
        }


        private void DecrementOrder()
        {
            TotalQuantity--;
        }

        private void IncrementOrder()
        {
            TotalQuantity++;
        }
    }
}

using Rg.Plugins.Popup.Services;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class CartViewModel : BaseViewModel
    {
        private UserCartItem _SelectedProductoItem;
        private double _TotalCost;
        private int _TotalQuantity;

        private ObservableCollection<UserCartItem> _CartItems;
        CartItemService cartItemService;

        public ObservableCollection<UserCartItem> CartItems
        {
            get { return _CartItems; }
            set
            {
                _CartItems = value;
                OnPropertyChanged("CartItems");
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
        
        public double TotalCost
        {
            set
            {
                _TotalCost = value;
                OnPropertyChanged();
            }

            get
            {
                return _TotalCost;
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

        public Command PlaceOrdersCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public Command IncreaseQuantityCommand { get; set; }
        public Command DeleteProductCommand { get; set; }
        public Command IncrementOrderCommand { get; set; }
        public Command DecrementOrderCommand { get; set; }
        public Command AddToCartCommand { get; set; }
        public Command DeleteAllProductCommand { get; set; }

        public CartViewModel()
        {
            CartItems = new ObservableCollection<UserCartItem>();
            cartItemService = new CartItemService();
            TotalQuantity = 1;
            LoadItems();
            PlaceOrdersCommand = new Command(async () => await PlaceOrdersAsync());
            IncreaseQuantityCommand = new Command<UserCartItem>((UserCartItem) => IncreaseQuantity(UserCartItem));
            DeleteProductCommand = new Command<UserCartItem>(async (UserCartItem) => await DeleteProductCartView(UserCartItem));

            IncrementOrderCommand = new Command(() => IncrementOrder());
            DecrementOrderCommand = new Command(() => DecrementOrder());
            AddToCartCommand = new Command(() => AddToCart());
            DeleteAllProductCommand = new Command(async () => await RemoveItemsFromCart());
        }

        private void AddToCart()
        {
            try
            {
                cartItemService.AddProductTocart(SelectedProductoItem, TotalQuantity);       
                LoadItems();
                TotalQuantity = 1;
                PopupNavigation.Instance.PopAsync();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void IncreaseQuantity(UserCartItem userCartItem)
        {
            var popup = new IncreaseQuantity();
            popup.BindingContext = this;
            SelectedProductoItem = userCartItem;
            TotalQuantity = SelectedProductoItem.Quantity;
            PopupNavigation.Instance.PushAsync(popup);
        }

        private async Task DeleteProductCartView(UserCartItem item)
        {
            var response = await Application.Current.MainPage.DisplayAlert("Advertencia", "¿Está seguro de remover el producto " + item.ProductName + "?", "Si", "No");
            if (response)
            {
                CartItems.Remove(item);
                cartItemService.RemoveProductById(item);
                LoadItems();
            }  
        }

        private async Task PlaceOrdersAsync()
        {
            if (CartItems.Count != 0)
            {
                Order order = new Order()
                {
                    client_user_id = Convert.ToInt32(Settings.IdUser),
                    delivery_user_id = 0,
                    order_date = DateTime.Now.ToString("dd-MM-yyyy"),
                    deliver_date = DateTime.Now.ToString("dd-MM-yyyy"),
                    score = "0",
                    comment = "",
                    total = Convert.ToDouble(TotalCost),
                    full_discount = 0.0,
                    client_location = "",
                    payment_type = "",
                    status = "ACTIVO"
                };
                await Application.Current.MainPage.Navigation.PushModalAsync(new PaymentMethodView(CartItems, order));
            } else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "No hay productos para procesar su pedido.", "Ok");
            }
        }

        private async Task RemoveItemsFromCart()
        {
            bool confirmation = await Application.Current.MainPage.DisplayAlert("Advertencia", "¿Está seguro de cancelar toda su orden?", "Si", "No");

            if (confirmation)
            {
                var cis = new CartItemService();
                cis.RemoveItemsFromCart();
                CartItems.Clear();
            }
        }

        private void LoadItems()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var items = cn.Table<CartItem>().ToList();
            CartItems.Clear();
            TotalCost = 0;
            foreach (var item in items)
            {
                CartItems.Add(new UserCartItem()
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
                TotalCost += (item.Price * item.Quantity);
            }
            TotalCost = Math.Round(TotalCost, 2);
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

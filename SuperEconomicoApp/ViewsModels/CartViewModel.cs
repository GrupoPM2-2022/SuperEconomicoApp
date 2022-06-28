using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class CartViewModel : BaseViewModel
    {
        public ObservableCollection<UserCartItem> CartItems { get; set; }
        CartItemService cartItemService;

        private double _TotalCost;
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

        public Command PlaceOrdersCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public CartViewModel()
        {
            CartItems = new ObservableCollection<UserCartItem>();
            cartItemService = new CartItemService();
            LoadItems();
            PlaceOrdersCommand = new Command(async () => await PlaceOrdersAsync());
            DeleteCommand = new Command<UserCartItem>((UserCartItem) => DeleteProductCartView(UserCartItem));
        }

        private void DeleteProductCartView(UserCartItem item)
        {
            CartItems.Remove(item);
            cartItemService.RemoveProductById(item);
        }

        private async Task PlaceOrdersAsync()
        {
            Order order = new Order()
            {
                client_user_id = 55,
                delivery_user_id = 56,
                order_date = DateTime.Now.ToString("dd-MM-yyyy"),
                deliver_date = DateTime.Now.ToString("dd-MM-yyyy"),
                score = "5",
                comment = "",
                total = Convert.ToDouble(TotalCost),
                full_discount = 0.0,
                client_location = "",
                payment_type = "",
                status = "GESTIONANDO"
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.PaymentMethodView(CartItems, order));
        }

        private void RemoveItemsFromCart()
        {
            var cis = new CartItemService();
            cis.RemoveItemsFromCart();
        }

        private void LoadItems()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var items = cn.Table<CartItem>().ToList();
            CartItems.Clear();
            foreach (var item in items)
            {
                CartItems.Add(new UserCartItem()
                {
                    CartItemId = item.CartItemId,
                    ImageProduct = item.ImageProduct,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Cost = item.Price * item.Quantity
                });
                TotalCost += (item.Price * item.Quantity);
            }
        }
    }
}

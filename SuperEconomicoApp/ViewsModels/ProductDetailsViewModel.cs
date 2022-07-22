using SuperEconomicoApp.Model;
using SuperEconomicoApp.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;


namespace SuperEconomicoApp.ViewsModels
{
    public class ProductDetailsViewModel : BaseViewModel
    {
        private ProductoItem _SelectedProductoItem;
        public ProductoItem SelectedProductoItem
        {
            set
            {
                _SelectedProductoItem = value;
                OnPropertyChanged();
            }

            get
            {
                return _SelectedProductoItem;
            }
        }

        private int _TotalQuantity;
        public int TotalQuantity
        {
            set
            {
                _TotalQuantity = value;
                if (_TotalQuantity <= 0)
                    _TotalQuantity = 1;
                if (_TotalQuantity > SelectedProductoItem.Stock)
                {
                    Application.Current.MainPage.DisplayAlert("Advertencia", "Este producto alcanzo su limite en stock", "Ok");
                    _TotalQuantity -= 1;
                }
                OnPropertyChanged();
            }

            get
            {
                return _TotalQuantity;
            }
        }

        public Command IncrementOrderCommand { get; set; }
        public Command DecrementOrderCommand { get; set; }
        public Command AddToCartCommand { get; set; }
        public Command ViewCartCommand { get; set; }
        public Command HomeCommand { get; set; }
        public Command CloseCommand { get; set; }

        public ProductDetailsViewModel(ProductoItem productoItem)
        {
            SelectedProductoItem = productoItem;
            TotalQuantity = 1;

            IncrementOrderCommand = new Command(() => IncrementOrder());
            DecrementOrderCommand = new Command(() => DecrementOrder());
            AddToCartCommand = new Command(() => AddToCart());
            ViewCartCommand = new Command(async () => await ViewCartAsync());
            HomeCommand = new Command(async () => await GotoHomeAsync());
            CloseCommand = new Command(async () => await ClosePage());
        }

        private async Task ClosePage()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async Task GotoHomeAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());
        }

        private async Task ViewCartAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new CartView());
        }

        private void AddToCart()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            try
            {
                CartItem ci = new CartItem()
                {
                    ProductId = SelectedProductoItem.Product_Id,
                    ProductName = SelectedProductoItem.Name,
                    Price = Math.Round(SelectedProductoItem.Price, 2),
                    Quantity = TotalQuantity,
                    ImageProduct = SelectedProductoItem.Image,
                    Stock = SelectedProductoItem.Stock,
                    Description = SelectedProductoItem.Description
                };
                var item = cn.Table<CartItem>().ToList()
                    .FirstOrDefault(c => c.ProductId == SelectedProductoItem.Product_Id);
                if (item == null)
                    cn.Insert(ci);
                else
                {
                    item.Quantity += TotalQuantity;
                    cn.Update(item);
                }
                cn.Commit();
                cn.Close();
                Application.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                cn.Close();
            }
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

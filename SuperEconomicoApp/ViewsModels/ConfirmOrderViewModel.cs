using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
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
        public ObservableCollection<UserCartItem> ListProductsOrdered { get; set; }
        public Order SelectedOrder { get; set; }
        public List<Direction> ListDirection { get; set; }
        public List<OrderDetails> ListOrders { get; set; }

        public Command DeleteOrderCommand { get; set; }
        public Command SaveOrderCommand { get; set; }
        public Command SelectLocationCommand { get; set; }
        public Command DeleteProductCommand { get; set; }

        CartItemService cartItemService;
        private double _Total;
        private string _Comment;

        public double Total
        {
            get { return _Total; }
            set { _Total = value; }
        }

        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                OnPropertyChanged();
            }
        }


        public ConfirmOrderViewModel(ObservableCollection<UserCartItem> listOrderDetails, Order order)
        {
            ListProductsOrdered = listOrderDetails;
            SelectedOrder = order;
            cartItemService = new CartItemService();
            LoadConfiguration();

            // COMANDOS
            DeleteOrderCommand = new Command(async () => await DeleteOrder());
            SaveOrderCommand = new Command(SaveOrder);
            SelectLocationCommand = new Command<Direction>((Direction) => SelectLocation(Direction));
            DeleteProductCommand = new Command<UserCartItem>(async (UserCartItem) => await DeleteProduct(UserCartItem));
        }

        private async Task DeleteProduct(UserCartItem userCartItem)
        {
            bool response = await Application.Current.MainPage.DisplayAlert("Advertencia", "¿Está seguro de remover el producto " + userCartItem.ProductName + "?", "Si", "No");
            if (response)
            {
                ListProductsOrdered.Remove(userCartItem);
                cartItemService.RemoveProductById(userCartItem);
            }
        }

        private void SelectLocation(Direction direction)
        {
            SelectedOrder.client_location = direction.Latitude.ToString() + "," + direction.Longitude.ToString();
        }

        private async void SaveOrder()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedOrder.client_location))
                {
                    await Application.Current.MainPage.DisplayAlert("Title", "Debes seleccionar la ubicación.", "Ok");
                    return;
                }

                FillOrderList();
                var response = new OrderService().CreateOrder(SelectedOrder);
                if (!response.IsCompleted)
                {
                    await Application.Current.MainPage.DisplayAlert("Confirmacion", "Pedido realizado exitosamente.", "Ok");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al insertar su pedido.", "Ok");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR_INSERT_BD ->: " + ex.Message);
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
                    discount = 0
                };

                ListOrders.Add(orderDetail);
            }
            SelectedOrder.orders_detail = ListOrders;
            SelectedOrder.comment = Comment;
        }

        private void LoadConfiguration()
        {
            Total = SelectedOrder.total;
            ListDirection = new List<Direction>()
            {
               new Direction()
                {
                    Id = 1,
                    Description = "Tegucigalpa, Col. Matamoros",
                    Latitude = "14.55465468",
                    Longitude = "-87.65484984",
                    IdUser = 40
                },
                new Direction()
                {
                    Id = 1,
                    Description = "Tegucigalpa, Col. La Aleman",
                    Latitude = "14.55465468",
                    Longitude = "-87.65484984",
                    IdUser = 40
                },
                new Direction()
                {
                    Id = 1,
                    Description = "Tegucigalpa, Col. La Aleman",
                    Latitude = "14.55465468",
                    Longitude = "-87.65484984",
                    IdUser = 40
                },
            };
        }

        private async Task DeleteOrder()
        {
            var response = await Application.Current.MainPage.DisplayAlert("Aviso", "¿Está seguro de cancelar su orden?", "Si", "No");
            if (response)
            {
                cartItemService.RemoveItemsFromCart();
                await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());
            }

        }

    }
}

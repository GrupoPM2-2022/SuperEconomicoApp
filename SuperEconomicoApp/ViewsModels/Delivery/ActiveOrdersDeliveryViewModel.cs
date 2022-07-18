using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels.Delivery
{



    public class ActiveOrdersDeliveryViewModel : BaseViewModel
    {
        #region VARIABLES
        private OrderService orderService;
        OrdersDelivery ordersDelivery;
        private List<ContentOrderDelivery> _ListOrders;
        private bool _ExistOrders;
        private bool _NotExistOrders;
        #endregion

        #region CONSTRUCTOR
        public ActiveOrdersDeliveryViewModel()
        {
            ordersDelivery = new OrdersDelivery();
            orderService = new OrderService();

            LoadConfiguration();
        }

        private async void LoadConfiguration()
        {
            ordersDelivery = await orderService.GetOrdersDeliveryByMethod("getDeliveryOrderActive");

            if (ordersDelivery == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener el historico de ordenes", "Ok");
                return;
            }

            if (ordersDelivery.orders.Count == 0)
            {
                NotExistOrders = true;
                ExistOrders = false;
            }
            else
            {
                NotExistOrders = false;
                ExistOrders = true;

                ListOrders = GetOrdersActiveByUser((List<ContentOrderDelivery>)ordersDelivery.orders);
            }

        }

        #endregion

        #region OBJETOS
        public List<ContentOrderDelivery> ListOrders
        {
            get { return _ListOrders; }
            set
            {
                _ListOrders = value;
                OnPropertyChanged();
            }
        }

        public bool NotExistOrders
        {
            get { return _NotExistOrders; }
            set
            {
                _NotExistOrders = value;
                OnPropertyChanged();
            }
        }

        public bool ExistOrders
        {
            get { return _ExistOrders; }
            set
            {
                _ExistOrders = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region PROCESOS

        private List<ContentOrderDelivery> GetOrdersActiveByUser(List<ContentOrderDelivery> orders)
        {
            foreach (var item in orders)
            {
                if (!item.PaymentType.Equals("Efectivo"))
                {
                    item.PaymentType = "Pago Online";
                }
                if (item.Status.Equals("ACTIVO"))
                {
                    item.TextButton = "PROCEDER A ENTREGAR";
                    item.IsVisibleStatus = false;

                }
                else if (item.Status.Equals("ENTREGA"))
                {
                    item.TextButton = "CERRAR ORDEN";
                    item.IsVisibleStatus = true;
                }
            }
            return orders;
        }

        private void ProceedWithDelivery(ContentOrderDelivery order)
        {
            if (order.Status.Equals("ACTIVO"))
            {
                // CAMBIAR STADO ORDEN A ENTREGA
                order.Status = "ENTREGA";
                UpdateStatusOrder(order);
            }
            else if (order.Status.Equals("ENTREGA"))
            {
                // CAMBIAR STADO ORDEN A CERRADO
                order.Status = "CERRADO";
                order.DeliveryDate = DateTime.Now;
                UpdateStatusOrder(order);
            }

        }

        private async void UpdateStatusOrder(ContentOrderDelivery order)
        {
            order = ExcludeUnnecessaryFields(order);
            bool response = await orderService.UpdateOrderDelivery(order);
            if (response)
            {
                //UpdateStatusFirebase();
                string messageSuccess = order.Status.Equals("CERRADO") ? "Orden Cerrada Correctamente." : "Orden cambio a entregando de forma correcta";
                await Application.Current.MainPage.DisplayAlert("Confirmacion", messageSuccess, "Ok");
                LoadConfiguration();
            }
            else
            {
                string messageError = order.Status.Equals("CERRADO") ? "Se produjo un error al cerrar la orden" : "Se produjo un error al proceder a entregar la orden";
                await Application.Current.MainPage.DisplayAlert("Advertencia", messageError, "Ok");
            }

        }

        private void UpdateStatusFirebase()
        {
            throw new NotImplementedException();
        }

        private ContentOrderDelivery ExcludeUnnecessaryFields(ContentOrderDelivery order)
        {
            order.ClientName = null;
            order.ClientLastname = null;
            order.PhoneClient = null;
            order.ClientImage = null;
            order.OrdersDetail = null;
            return order;

        }
        #endregion

        #region COMANDOS
        public ICommand ProceedWithDeliveryCommand => new Command<ContentOrderDelivery>((param) => ProceedWithDelivery(param));
        public ICommand CerrarCommand => new Command(async () => await Cerrar());

        private async Task Cerrar()
        {
            var cis = new CartItemService();
            cis.RemoveItemsFromCart();
            Settings.ClearAllData();
            await Application.Current.MainPage.Navigation.PushModalAsync(new LoginView());
        }

        #endregion
    }
}

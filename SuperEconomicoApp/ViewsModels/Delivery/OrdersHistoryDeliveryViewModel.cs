using Acr.UserDialogs;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views.Delivery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels.Delivery
{
    public class OrdersHistoryDeliveryViewModel : BaseViewModel
    {
        #region VARIABLES
        private OrderService orderService;
        OrdersDelivery ordersDelivery;
        private List<ContentOrderDelivery> _ListOrders;
        private bool _ExistOrders;
        private bool _NotExistOrders;
        private string _ImageScore;
        private string _NameUser;
        private string _Comment;

        #endregion

        #region CONSTRUCTOR
        public OrdersHistoryDeliveryViewModel()
        {
            ordersDelivery = new OrdersDelivery();
            orderService = new OrderService();
            //LoadConfiguration();
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

        public string ImageScore
        {
            get { return _ImageScore; }
            set
            {
                _ImageScore = value;
                OnPropertyChanged();
            }
        }
        public string NameUser
        {
            get { return _NameUser; }
            set
            {
                _NameUser = value;
                OnPropertyChanged();
            }
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

        #endregion

        #region PROCESOS
        public async void LoadConfiguration()
        {
            UserDialogs.Instance.ShowLoading("Cargando");
            ordersDelivery = await orderService.GetOrdersDeliveryByMethod("getDeliveryOrderForId");
            if (ordersDelivery == null)
            {
                UserDialogs.Instance.HideLoading();
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

                ListOrders = (List<ContentOrderDelivery>)ordersDelivery.orders;
            }
            UserDialogs.Instance.HideLoading();

        }

        private async Task ShowOrderDetail(ContentOrderDelivery order)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new OrdersDetailHistoryByDeliveryView(order));
        }

        #endregion

        #region COMANDOS
        public ICommand OrderDetailCommand => new Command<ContentOrderDelivery>(async (Order) => await ShowOrderDetail(Order));

        #endregion
    }
}
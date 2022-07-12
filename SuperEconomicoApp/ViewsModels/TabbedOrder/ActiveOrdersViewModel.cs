using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels.TabbedOrder
{
    public class ActiveOrdersViewModel : BaseViewModel
    {
        #region VARIABLES
        private OrderService orderService;
        OrdersByUser ordersByUser;
        private List<Order> _ListOrders;
        private bool _ExistOrders;
        private bool _NotExistOrders;
        #endregion

        #region CONSTRUCTOR
        public ActiveOrdersViewModel()
        {
            ordersByUser = new OrdersByUser();
            orderService = new OrderService();

            LoadConfiguration();
        }

        private async void LoadConfiguration()
        {
            ordersByUser = await orderService.GetOrdersUserByMethod("getUserOrderActive");

            if (ordersByUser == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener las ordenes activas", "Ok");
                return;
            }

            if (ordersByUser.orders.Count == 0)
            {
                NotExistOrders = true;
                ExistOrders = false;
            } else
            {
                NotExistOrders = false;
                ExistOrders = true;

                ListOrders = (List<Order>) ordersByUser.orders;
                ListOrders.Sort((x, y) => DateTime.Compare(DateTime.Now, y.order_date));
            }

        }
        #endregion

        #region OBJETOS
        public List<Order> ListOrders
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
        private async Task ShowOrderDetail(Order order)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ShowOrderDetailView(order));
        }
        #endregion

        #region COMANDOS
        public ICommand OrderDetailCommand => new Command<Order>(async (Order) => await ShowOrderDetail(Order));

        #endregion
    }
}

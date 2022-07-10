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
        private string _Texto;
        private OrderService orderService;
        OrdersActiveByUser ordersActiveByUsers;
        private List<Order> _ListOrders;
        #endregion

        #region CONSTRUCTOR
        public ActiveOrdersViewModel()
        {
            ordersActiveByUsers = new OrdersActiveByUser();
            orderService = new OrderService();

            LoadConfiguration();
        }

        private async void LoadConfiguration()
        {
            ordersActiveByUsers = await orderService.GetActiveOrdersByUser();
            if (ordersActiveByUsers == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener las ordenes activas", "Ok");
                return;
            }

            ListOrders = (List<Order>) ordersActiveByUsers.orders;
        }
        #endregion

        #region OBJETOS
        public string Texto
        {
            get { return _Texto; }
            set
            {
                _Texto = value;
                OnPropertyChanged();
            }
        }


        public List<Order> ListOrders
        {
            get { return _ListOrders; }
            set
            {
                _ListOrders = value;
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

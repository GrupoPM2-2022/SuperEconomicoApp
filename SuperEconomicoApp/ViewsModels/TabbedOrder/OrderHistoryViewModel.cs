using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SuperEconomicoApp.Helpers;

namespace SuperEconomicoApp.ViewsModels.TabbedOrder
{
    public class OrderHistoryViewModel : BaseViewModel
    {
        #region VARIABLES
        private OrderService orderService;
        OrdersByUser ordersActiveByUsers;
        private List<Order> _ListOrders;
        private bool _ExistOrders;
        private bool _NotExistOrders;
        private string _ImageScore;
        private string _NameUser;
        private string _Comment;

        #endregion

        #region CONSTRUCTOR
        public OrderHistoryViewModel()
        {
            ordersActiveByUsers = new OrdersByUser();
            orderService = new OrderService();

            LoadConfiguration();
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
            ordersActiveByUsers = await orderService.GetOrdersUserByMethod("getUserOrderForId");
            if (ordersActiveByUsers == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener las ordenes activas", "Ok");
                return;
            }

            if (ordersActiveByUsers.orders.Count == 0)
            {
                NotExistOrders = true;
                ExistOrders = false;
            }
            else
            {
                NotExistOrders = false;
                ExistOrders = true;

                GetAllOrdersActiveByUser();
            }

        }
        private void GetAllOrdersActiveByUser()
        {
            ListOrders = GetOrdersByUser((List<Order>)ordersActiveByUsers.orders);
            //ListOrders.Sort((x, y) => DateTime.Compare(DateTime.Now, Convert.ToDateTime(y.order_date)));
        }

        private List<Order> GetOrdersByUser(List<Order> orders)
        {
            foreach (var item in orders)
            {
                if (string.IsNullOrEmpty(item.score) || item.score.Equals("0"))
                {
                    item.NameButton = "CALIFICAR";
                }
                else
                {
                    item.NameButton = "VER OPINION";
                }
            }

            return orders;
        }

        private async Task ShowOrderDetail(Order order)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ShowOrderHistoryView(order));
        }

        private async Task ShowComment(Order order)
        {
            if (string.IsNullOrEmpty(order.score) || order.score.Equals("0"))
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new AddCommentView(order));
            }
            else
            {
                ImageScore = SelectImageRating(order.score);
                Comment = order.comment;
                NameUser = Settings.UserName;

                var popup = new PreviewOpinion();
                popup.BindingContext = this;
                await PopupNavigation.Instance.PushAsync(popup);
            }
        }

        private string SelectImageRating(string score)
        {
            if (score.Equals("1"))
            {
                return "una_estrellas.png";
            }
            else if (score.Equals("2"))
            {
                return "dos_estrellas.png";
            }
            else if (score.Equals("3"))
            {
                return "tres_estrellas.png";
            }
            else if (score.Equals("4"))
            {
                return "cuatro_estrellas.png";
            }
            else if (score.Equals("5"))
            {
                return "cinco_estrellas.png";
            }
            else
            {
                return "Empty";
            }
        }

        #endregion

        #region COMANDOS
        public ICommand OrderDetailCommand => new Command<Order>(async (Order) => await ShowOrderDetail(Order));
        public ICommand ShowCommentCommand => new Command<Order>(async (Order) => await ShowComment(Order));

        #endregion
    }
}

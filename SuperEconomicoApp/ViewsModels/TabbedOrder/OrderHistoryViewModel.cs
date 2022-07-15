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
        private bool _IsVisiblePreview;
        private bool _IsVisibleSaveScore;
        private string _ImageScore;
        private string _NameUser;
        private string _Comment;
        private string _Star5 = "estrella_sin_relleno";
        private string _Star4 = "estrella_sin_relleno";
        private string _Star3 = "estrella_sin_relleno";
        private string _Star2 = "estrella_sin_relleno";
        private string _Star1 = "estrella_sin_relleno";
        private string Score;
        private string OrderId;
        private int DeliveryId;
        private int ClientUserId;
        private double Total = 0.00;
        private DateTime DateOrder;
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

        public bool IsVisiblePreview
        {
            get { return _IsVisiblePreview; }
            set
            {
                _IsVisiblePreview = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisibleSaveScore
        {
            get { return _IsVisibleSaveScore; }
            set
            {
                _IsVisibleSaveScore = value;
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

        public string Star1
        {
            get { return _Star1; }
            set
            {
                _Star1 = value;
                OnPropertyChanged();
            }
        }

        public string Star2
        {
            get { return _Star2; }
            set
            {
                _Star2 = value;
                OnPropertyChanged();
            }
        }

        public string Star3
        {
            get { return _Star3; }
            set
            {
                _Star3 = value;
                OnPropertyChanged();
            }
        }

        public string Star4
        {
            get { return _Star4; }
            set
            {
                _Star4 = value;
                OnPropertyChanged();
            }
        }

        public string Star5
        {
            get { return _Star5; }
            set
            {
                _Star5 = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region PROCESOS
        private async void LoadConfiguration()
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
                CleanStars();
                OrderId = order.order_id;
                DeliveryId = order.delivery_user_id;
                ClientUserId = order.client_user_id;
                Total = order.total;
                DateOrder = order.order_date;

                IsVisiblePreview = false;
                IsVisibleSaveScore = true;
            }
            else
            {
                ImageScore = SelectImageRating(order.score);

                IsVisiblePreview = true;
                IsVisibleSaveScore = false;
            }

            Comment = order.comment;
            NameUser = Settings.UserName;

            var popup = new PreviewOpinion();
            popup.BindingContext = this;
            await PopupNavigation.Instance.PushAsync(popup);
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

        private void SelectStar(string star)
        {
            Score = star;
            
            if (star.Equals("1"))
            {
                Star1 = "estrella_con_relleno";
            }
            else if (star.Equals("2"))
            {
                Star1 = "estrella_con_relleno";
                Star2 = "estrella_con_relleno";
            }
            else if (star.Equals("3"))
            {
                Star1 = "estrella_con_relleno";
                Star2 = "estrella_con_relleno";
                Star3 = "estrella_con_relleno";
            }
            else if (star.Equals("4"))
            {
                Star1 = "estrella_con_relleno";
                Star2 = "estrella_con_relleno";
                Star3 = "estrella_con_relleno";
                Star4 = "estrella_con_relleno";
            }
            else if (star.Equals("5"))
            {
                Star1 = "estrella_con_relleno";
                Star2 = "estrella_con_relleno";
                Star3 = "estrella_con_relleno";
                Star4 = "estrella_con_relleno";
                Star5 = "estrella_con_relleno";
            }
        }

        private async Task SaveScore()
        {
            if (Score.Equals(""))
            {
                await Application.Current.MainPage.DisplayAlert("Ok", "Selecciona la calificación para poder guardarla", "Ok");
                return;
            }

            Order order = new Order
            {
                comment = Comment,
                score = Score,
                order_id = OrderId,
                client_user_id = ClientUserId,
                delivery_user_id = DeliveryId,
                total = Total,
                order_date = DateOrder
            };

            //bool response = await orderService.UpdateOrder(order);
            //if (response)
            //{
            //    await Application.Current.MainPage.DisplayAlert("Confirmación", "Calificación guardada correctamente.", "Ok");
            //    CleanAfterUpdate();
            //    await PopupNavigation.Instance.PopAsync();
            //}
            //else
            //{
            //    await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al guardar su calificación.", "Ok");
            //}

        }

        private void CleanStars()
        {
            Star1 = "estrella_sin_relleno";
            Star2 = "estrella_sin_relleno";
            Star3 = "estrella_sin_relleno";
            Star4 = "estrella_sin_relleno";
            Star5 = "estrella_sin_relleno";
        }

        private void CleanAfterUpdate()
        {
            CleanStars();
            Comment = "";
            Score = "";
            OrderId = "";
            DeliveryId = 0;
            ClientUserId = 0;
            ListOrders.Clear();
            GetAllOrdersActiveByUser();
        }
        #endregion

        #region COMANDOS
        public ICommand OrderDetailCommand => new Command<Order>(async (Order) => await ShowOrderDetail(Order));
        public ICommand ShowCommentCommand => new Command<Order>(async (Order) => await ShowComment(Order));
        public ICommand SelectStarCommand => new Command<string>((args) => SelectStar(args));
        public ICommand SaveScoreCommand => new Command(async () => await SaveScore());

        #endregion
    }
}

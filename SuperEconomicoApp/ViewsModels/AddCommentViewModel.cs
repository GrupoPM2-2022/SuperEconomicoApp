using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class AddCommentViewModel : BaseViewModel
    {

        #region Variables
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
        private string _Comment;
        OrderService orderService;
        #endregion

        #region Contructor
        public AddCommentViewModel(Order order)
        {
            OrderId = order.order_id;
            DeliveryId = order.delivery_user_id;
            ClientUserId = order.client_user_id;
            Total = order.total;
            DateOrder = order.order_date;

            orderService = new OrderService();
        }
        #endregion

        #region Objetos
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

        #region Procesos
        private void SelectStar(string star)
        {
            Score = star;
            CleanStars();

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

        private void CleanStars()
        {
            Star1 = "estrella_sin_relleno";
            Star2 = "estrella_sin_relleno";
            Star3 = "estrella_sin_relleno";
            Star4 = "estrella_sin_relleno";
            Star5 = "estrella_sin_relleno";
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
                order_id = OrderId,
                delivery_user_id = DeliveryId,
                client_user_id = ClientUserId,
                total = Total,
                order_date = DateOrder,
                score = Score,
                comment = Comment
            };
          

            bool response = await orderService.UpdateOrder(order);
            if (response)
            {
                await Application.Current.MainPage.DisplayAlert("Confirmación", "Calificación guardada correctamente.", "Ok");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al guardar su calificación.", "Ok");
            }

        }
        #endregion

        #region Constructor
        public ICommand SelectStarCommand => new Command<string>((args) => SelectStar(args));
        public ICommand SaveScoreCommand => new Command(async () => await SaveScore());
        #endregion

    }
}

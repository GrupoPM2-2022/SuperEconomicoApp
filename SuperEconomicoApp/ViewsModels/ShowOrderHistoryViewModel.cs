using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Plugin.CloudFirestore;
using SuperEconomicoApp.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SuperEconomicoApp.ViewsModels
{
    public class ShowOrderHistoryViewModel : BaseViewModel
    {
        private List<OrderDetails> _ListOrderDetails;
        private string _Total;
        private string _NameDelivery;
        private string _NumberPhoneDelivery;
        private byte[] _ImageDelivery;
        private string _DateOrder;
        private string _DateDelivery;
        private string _ImageScore;
        private string _Comment;
        private string _NameUser;
        private string _TypePayment;
        private bool _IsVisibleScore;
        private bool _IsVisibleText;

        private Map map;
        private Order order;
        private string[] coordinatesSup;
        private string[] coordinatesUser;

        public ShowOrderHistoryViewModel(Map mapParam, Order orderParam)
        {
            map = mapParam;
            order = orderParam;
            LoadConfiguration();
        }

        #region OBJETOS
        public List<OrderDetails> ListOrdersDetail
        {
            get { return _ListOrderDetails; }
            set
            {
                _ListOrderDetails = value;
                OnPropertyChanged();
            }
        }

        public byte[] ImageDelivery
        {
            get { return _ImageDelivery; }
            set
            {
                _ImageDelivery = value;
                OnPropertyChanged();
            }
        }

        public string Total
        {
            get { return _Total; }
            set
            {
                _Total = value;
                OnPropertyChanged();
            }
        }

        public string NameDelivery
        {
            get { return _NameDelivery; }
            set
            {
                _NameDelivery = value;
                OnPropertyChanged();
            }
        }

        public string NumberPhoneDelivery
        {
            get { return _NumberPhoneDelivery; }
            set
            {
                _NumberPhoneDelivery = value;
                OnPropertyChanged();
            }
        }
        public string DateOrder
        {
            get { return _DateOrder; }
            set
            {
                _DateOrder = value;
                OnPropertyChanged();
            }
        }
        public string DateDelivery
        {
            get { return _DateDelivery; }
            set
            {
                _DateDelivery = value;
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
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                OnPropertyChanged();
            }
        }
        public string TypePayment
        {
            get { return _TypePayment; }
            set
            {
                _TypePayment = value;
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

        public bool IsVisibleScore
        {
            get { return _IsVisibleScore; }
            set
            {
                _IsVisibleScore = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisibleText
        {
            get { return _IsVisibleText; }
            set
            {
                _IsVisibleText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region PROCESOS
        private async void LoadConfiguration()
        {
            if (order == null)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Se produjo un error al mostrar el detalle del pedido", "Ok");
                return;
            }

            ConfigurationMap();

            ListOrdersDetail = order.orders_detail;
            ImageDelivery = order.delivery_image;
            Total = order.total.ToString("F", CultureInfo.InvariantCulture);

            NameDelivery = order.delivery_name + " " + order.delivery_lastname;
            NumberPhoneDelivery = order.phone_delivery;
            DateOrder = order.order_date;
            DateDelivery = order.deliver_date;
            Comment = order.comment;
            NameUser = Settings.UserName;

            if (order.payment_type.Equals("Efectivo"))
            {
                TypePayment = "Pago en efectivo";
            }
            else
            {
                TypePayment = "Pago online";
            }

            if (!string.IsNullOrEmpty(order.score))
            {
                string imageScore = SelectImageRating();
                if (!imageScore.Equals("Empty"))
                {
                    IsVisibleText = false;
                    IsVisibleScore = true;
                    ImageScore = imageScore;
                }
            }
            else
            {
                IsVisibleText = true;
                IsVisibleScore = false;
            }

        }

        private string SelectImageRating()
        {
            if (order.score.Equals("1"))
            {
                return "una_estrellas.png";
            }
            else if (order.score.Equals("2"))
            {
                return "dos_estrellas.png";
            }
            else if (order.score.Equals("3"))
            {
                return "tres_estrellas.png";
            }
            else if (order.score.Equals("4"))
            {
                return "cuatro_estrellas.png";
            }
            else if (order.score.Equals("5"))
            {
                return "cinco_estrellas.png";
            }
            else
            {
                return "Empty";
            }
        }

        private void ConfigurationMap()
        {
            coordinatesSup = order.sucursal.Split(',');
            coordinatesUser = order.client_location.Split(',');

            TraceRoute();

            Position currentPosition = new Position(Convert.ToDouble(coordinatesSup[0]), Convert.ToDouble(coordinatesSup[1]));

            Pin pinSupermarket = new Pin
            {
                Label = "El Economico",
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.FromBundle("supermercado.png"),
                Position = currentPosition,
            };

            Pin pinDestination = new Pin
            {
                Label = "Destino",
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.FromBundle("destination.png"),
                Position = new Position(Convert.ToDouble(coordinatesUser[0]), Convert.ToDouble(coordinatesUser[1])),
            };

            map.Pins.Add(pinSupermarket);
            map.Pins.Add(pinDestination);
        }

        private async void TraceRoute()
        {
            var pathContent = await LoadRoute();
            map.Polylines.Clear();

            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.Purple;
            polyline.StrokeWidth = 3;

            foreach (var item in pathContent)
            {
                polyline.Positions.Add(item);
            }

            map.Polylines.Add(polyline);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(polyline.Positions[0].Latitude, polyline.Positions[0].Longitude), Xamarin.Forms.GoogleMaps.Distance.FromMeters(700)));
        }


        private async Task<List<Position>> LoadRoute()
        {
            try
            {
                var googleDirection = await ApiServices.ServiceClientInstance.GetDirections(coordinatesSup[0], coordinatesSup[1], coordinatesUser[0], coordinatesUser[1]);
                if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
                {
                    var positions = (Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points)));
                    return positions;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Agrega tu método de pago dentro de la consola de Google Maps", "Ok");
                    return null;
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Se produjo un error al trazar la ruta", "Ok");
            }

            return null;
        }

        public void ProcesoSimple()
        {

        }
        #endregion

        #region COMANDOS
        public ICommand ProcesoSimpcommand => new Command(ProcesoSimple);
        #endregion

    }
}

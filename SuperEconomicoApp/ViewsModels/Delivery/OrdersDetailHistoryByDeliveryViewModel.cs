using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace SuperEconomicoApp.ViewsModels.Delivery
{
    public class OrdersDetailHistoryByDeliveryViewModel : BaseViewModel
    {
        private List<OrderDetails> _ListOrderDetails;
        private string _Total;
        private string _NameClient;
        private string _NumberPhoneClient;
        private byte[] _ImageClient;
        private string _DateOrder;
        private string _DateDelivery;
        private string _ImageScore;
        private string _Comment;
        private string _NameUser;
        private string _TypePayment;

        private Map map;
        private ContentOrderDelivery order;
        private string[] coordinatesSup;
        private string[] coordinatesUser;

        public OrdersDetailHistoryByDeliveryViewModel(Map mapParam, ContentOrderDelivery orderParam)
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

        public byte[] ImageClient
        {
            get { return _ImageClient; }
            set
            {
                _ImageClient = value;
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

        public string NameClient
        {
            get { return _NameClient; }
            set
            {
                _NameClient = value;
                OnPropertyChanged();
            }
        }

        public string NumberPhoneClient
        {
            get { return _NumberPhoneClient; }
            set
            {
                _NumberPhoneClient = value;
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

            ListOrdersDetail = order.OrdersDetail;
            ImageClient = order.ClientImage;
            Total = order.Total;

            NameClient = order.ClientName + " " + order.ClientLastname;
            NumberPhoneClient = order.PhoneClient;
            DateOrder = order.OrderDate.ToString();
            DateDelivery = order.DeliveryDate.ToString();

            if (order.PaymentType.Equals("Efectivo"))
            {
                TypePayment = "Pago en efectivo";
            }
            else
            {
                TypePayment = "Pago Online";
            }

        }

        private void ConfigurationMap()
        {
            coordinatesSup = order.Sucursal.Split(',');
            coordinatesUser = order.ClientLocation.Split(',');

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
        #endregion
    }
}
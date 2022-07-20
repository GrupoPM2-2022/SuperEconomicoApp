using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SuperEconomicoApp.Model;
using Xamarin.Forms.GoogleMaps;
using System.Threading.Tasks;
using SuperEconomicoApp.Services;
using System.Linq;
using System.Windows.Input;
using System.Globalization;
using SuperEconomicoApp.Helpers;
using Xamarin.Essentials;

namespace SuperEconomicoApp.ViewsModels.Delivery
{
    public class ActiveOrdersDetailDeliveryViewModel : BaseViewModel
    {
        #region VARIABLES
        private List<OrderDetails> _ListOrderDetails;
        private string _Total;
        private string _NameDelivery;
        private string _NumberPhoneDelivery;
        private bool _IsPhoneEnabled;
        private Xamarin.Forms.GoogleMaps.Map map;
        private ContentOrderDelivery order;
        private byte[] _ImageDelivery;
        private Pin pinDelivery;
        private string[] coordinatesUser;

        #endregion

        #region CONSTRUCTOR
        public ActiveOrdersDetailDeliveryViewModel(ContentOrderDelivery orderParam, Xamarin.Forms.GoogleMaps.Map mapParam)
        {
            map = mapParam;
            order = orderParam;
            pinDelivery = new Pin();
            LoadConfiguration();
        }

        public ActiveOrdersDetailDeliveryViewModel(Xamarin.Forms.GoogleMaps.Map mapParam)
        {
            map = mapParam;
        }

        #endregion

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

        public bool IsPhoneEnabled
        {
            get { return _IsPhoneEnabled; }
            set
            {
                _IsPhoneEnabled = value;
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

            ListOrdersDetail = order.OrdersDetail;
            ImageDelivery = order.ClientImage;
            Total = order.Total;

            if (!order.ClientName.Equals("no asignado"))
            {
                NameDelivery = order.ClientName + " " + order.ClientLastname;
                NumberPhoneDelivery = order.PhoneClient;
                IsPhoneEnabled = true;
            }
            else
            {
                NameDelivery = "No asignado momentáneamente";
                NumberPhoneDelivery = "+504 --";
                IsPhoneEnabled = false;
            }

            ConfigurationMap();

        }

        private async void ConfigurationMap()
        {
            var coordinatesSup = order.Sucursal.Split(',');
            coordinatesUser = order.ClientLocation.Split(',');
            var location = await Geolocation.GetLocationAsync();
            string[] currentLocation = string.Concat(location.Latitude.ToString(), ",",location.Longitude.ToString()).Split(',');

            TraceRoute(currentLocation, coordinatesUser);

            

            Pin pinSupermarket = new Pin
            {
                Label = "El Economico",
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.FromBundle("supermercado.png"),
                Position = new Position(Convert.ToDouble(coordinatesSup[0]), Convert.ToDouble(coordinatesSup[1])),
            };

            Pin pinDestination = new Pin
            {
                Label = "Destino",
                Type = PinType.Place,
                Icon = BitmapDescriptorFactory.FromBundle("destination.png"),
                Position = new Position(Convert.ToDouble(coordinatesUser[0]), Convert.ToDouble(coordinatesUser[1]))
            };

            pinDelivery.Label = "Repartidor";
            pinDelivery.Type = PinType.Place;
            pinDelivery.Icon = BitmapDescriptorFactory.FromBundle("pin_delivery.png");

            map.Pins.Add(pinSupermarket);
            map.Pins.Add(pinDestination);
            map.Pins.Add(pinDelivery);

            //map.MoveToRegion(MapSpan.FromCenterAndRadius(destintyPosition, Xamarin.Forms.GoogleMaps.Distance.FromMeters(400)));
            Settings.CurrentPage = "Mapa";

            //GetTrackingRealTime();
        }

        public async void TraceRoute(string[] coordinatesOrigin, string[] coordinatesDestiny)
        {
            var pathContent = await LoadRoute(coordinatesOrigin, coordinatesDestiny);
            map.Polylines.Clear();

            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.Purple;
            polyline.StrokeWidth = 3;

            foreach (var item in pathContent)
            {
                polyline.Positions.Add(item);
            }

            map.Polylines.Add(polyline);

            var position = new Position(polyline.Positions[0].Latitude, polyline.Positions[0].Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Xamarin.Forms.GoogleMaps.Distance.FromMeters(400)));
            pinDelivery.Position = position;
            Console.WriteLine("RECALCULANDO RUTA");
        }


        private async Task<List<Position>> LoadRoute(string[] coordinatesOrigin, string[] coordinatesDestiny)
        {
            try
            {
                var googleDirection = await ApiServices.ServiceClientInstance.GetDirections(coordinatesOrigin[0], coordinatesOrigin[1], coordinatesDestiny[0], coordinatesDestiny[1]);
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

        private async void CallDelivery()
        {
            try
            {
                Xamarin.Essentials.PhoneDialer.Open(NumberPhoneDelivery);
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al llamar al repartidor", "Ok");
            }
        }
        #endregion

        #region COMANDOS
        public ICommand CallDeliveryCommand => new Command(CallDelivery);
        #endregion
    }
}
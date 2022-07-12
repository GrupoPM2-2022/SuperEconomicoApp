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
    public class ShowOrderDetailViewModel : BaseViewModel
    {
        #region VARIABLES
        private List<OrderDetails> _ListOrderDetails;
        private string _Total;
        private string _NameDelivery;
        private string _NumberPhoneDelivery;
        private bool _IsPhoneEnabled;
        private Map map;
        private Order order;
        private byte[] _ImageDelivery;
        private Pin pinDelivery;
        private string[] coordinatesUser;

        #endregion

        #region CONSTRUCTOR
        public ShowOrderDetailViewModel(Order orderParam, Map mapParam)
        {
            map = mapParam;
            order = orderParam;
            pinDelivery = new Pin();
            LoadConfiguration();
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

            ListOrdersDetail = order.orders_detail;
            ImageDelivery = order.delivery_image;
            Total = order.total.ToString("F", CultureInfo.InvariantCulture);

            if (!order.delivery_name.Equals("no asignado"))
            {
                NameDelivery = order.delivery_name + " " + order.delivery_lastname;
                NumberPhoneDelivery = order.phone_delivery;
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

        private void ConfigurationMap()
        {
            var coordinatesSup = order.sucursal.Split(',');
            coordinatesUser = order.client_location.Split(',');

            TraceRoute(coordinatesSup, 900);

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

            pinDelivery.Label = "Repartidor";
            pinDelivery.Type = PinType.Place;
            pinDelivery.Icon = BitmapDescriptorFactory.FromBundle("pin_delivery.png");
            pinDelivery.Position = currentPosition;
            pinDelivery.IsVisible = false;

            map.Pins.Add(pinSupermarket);
            map.Pins.Add(pinDestination);
            map.Pins.Add(pinDelivery);

            GetTrackingRealTime();
        }

        private void GetTrackingRealTime()
        {
            string idUserDelivery = order.delivery_user_id.ToString();
            CrossCloudFirestore.Current
               .Instance
               .Collection("Ubicacion2")
               .AddSnapshotListener((snapshot, error) =>
               {
                   if (snapshot != null)
                   {
                       foreach (var documentChange in snapshot.DocumentChanges)
                       {
                           if (documentChange.Type == DocumentChangeType.Modified)
                           {
                               if (!pinDelivery.IsVisible)
                               {
                                   pinDelivery.IsVisible = true;
                               }

                               if (documentChange.Document.Id.Equals(idUserDelivery))
                               {
                                   Dictionary<string, object> element = (Dictionary<string, object>) documentChange.Document.Data;
                                   string[] ubication = { };

                                   foreach (KeyValuePair<string, object> pair in element)
                                   {
                                       if (pair.Key.Equals("ubicacion"))
                                       {
                                           ubication = pair.Value.ToString().Split(',');
                                       }
                                   }

                                   TraceRoute(ubication, 350);
                                   pinDelivery.Position = new Position(Convert.ToDouble(ubication[0]), Convert.ToDouble(ubication[1]));
                               }

                           }
                       }
                   }
               });
        }

        private async void TraceRoute(string[] coordinatesOrigin, double meters) {
            var pathContent = await LoadRoute(coordinatesOrigin);
            map.Polylines.Clear();

            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            polyline.StrokeColor = Color.Purple;
            polyline.StrokeWidth = 3;

            foreach (var item in pathContent)
            {
                polyline.Positions.Add(item);
            }

            map.Polylines.Add(polyline);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(polyline.Positions[0].Latitude, polyline.Positions[0].Longitude), Xamarin.Forms.GoogleMaps.Distance.FromMeters(meters)));
        }


        private async Task<List<Position>> LoadRoute(string[] coordinatesOrigin) {
            try
            {
                var googleDirection = await ApiServices.ServiceClientInstance.GetDirections(coordinatesOrigin[0], coordinatesOrigin[1], coordinatesUser[0], coordinatesUser[1]);
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
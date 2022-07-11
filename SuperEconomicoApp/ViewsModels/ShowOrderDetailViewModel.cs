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
            var coordinatesUser = order.client_location.Split(',');
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

            map.Pins.Add(pinSupermarket);
            map.Pins.Add(pinDestination);
            map.Pins.Add(pinDelivery);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(currentPosition, Xamarin.Forms.GoogleMaps.Distance.FromKilometers(1.5)));

            GetTrackingRealTime();
        }

        private void GetTrackingRealTime()
        {
            string idUserDelivery = order.delivery_user_id.ToString();
            //if (!idUserDelivery.Equals(300))
            //{
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

                               if (documentChange.Document.Id.Equals(idUserDelivery))
                               {
                                   Dictionary<string, object> city = (Dictionary<string, object>)documentChange.Document.Data;
                                   double latitude = 0, longitude = 0;

                                   foreach (KeyValuePair<string, object> pair in city)
                                   {
                                       if (pair.Key.Equals("ubicacion"))
                                       {
                                           string[] ubication = pair.Value.ToString().Split(',');
                                           latitude = Convert.ToDouble(ubication[0]);
                                           longitude = Convert.ToDouble(ubication[1]);
                                       }
                                   }

                                   pinDelivery.Position = new Position(latitude, longitude);
                                   map.MoveToRegion(MapSpan.FromCenterAndRadius(pinDelivery.Position, Xamarin.Forms.GoogleMaps.Distance.FromMeters(800)));

                               }

                           }
                       }
                   }
               });
            //}
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
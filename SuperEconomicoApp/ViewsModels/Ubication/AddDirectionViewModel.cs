using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace SuperEconomicoApp.ViewsModels.Ubication
{
    public class AddDirectionViewModel : BaseViewModel
    {
        #region VARIABLES
        private static double VALID_KILOMETERS = 30;
        string _Direction;
        Pin pinSupermarket = new Pin();
        Pin pinUser = new Pin();
        Map map;
        private double latitudeSupermarket;
        private double longitudeSupermarket;
        private double latitudeUser;
        private double longitudeUser;
        private string coordinatesSupermarket;
        private string coordinatesUser;
        GoogleServiceApi googleServiceApi;
        GoogleDistanceMatrix googleDistanceMatrix;

        #endregion

        #region CONSTRUCTOR
        public AddDirectionViewModel(Map mapReference)
        {
            map = mapReference;
            googleServiceApi = new GoogleServiceApi();
            googleDistanceMatrix = new GoogleDistanceMatrix();
            LoadConfiguration();
            map.PinDragEnd += Map_PinDragEnd;
        }

        #endregion

        #region OBJETOS
        public string Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }
        #endregion

        #region PROCESOS
        private async void Map_PinDragEnd(object sender, PinDragEventArgs e)
        {
            latitudeUser = e.Pin.Position.Latitude;
            longitudeUser = e.Pin.Position.Longitude;
            var position = new Position(latitudeUser, longitudeUser);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Xamarin.Forms.GoogleMaps.Distance.FromMeters(700)));
            coordinatesUser = latitudeUser.ToString() + "," + longitudeUser.ToString();
            googleDistanceMatrix = await googleServiceApi.CalculateDistanceTwoCoordinates(coordinatesSupermarket, coordinatesUser);

            int meters = googleDistanceMatrix.rows[0].elements[0].distance.value;

            double kilometers = meters / 1000;
            if (kilometers > VALID_KILOMETERS)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual", "Ok");
            }
        }

        private void LoadConfiguration()
        {
            ApplyMapTheme();
            var coordinates = Settings.Coordinates.Split(',');
            latitudeSupermarket = Convert.ToDouble(coordinates[0]);
            longitudeSupermarket = Convert.ToDouble(coordinates[1]);
            coordinatesSupermarket = coordinates[0] + "," + coordinates[1];
            var positionSupermarket = new Position(latitudeSupermarket, longitudeSupermarket);

            //pinSupermarket.Label = "El Economico";
            //pinSupermarket.Type = PinType.Place;
            //pinSupermarket.Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("supermercado.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "supermercado.png", WidthRequest = 64, HeightRequest = 64 });
            //pinSupermarket.Position = positionSupermarket;
            //pinSupermarket.IsDraggable = false;

            pinUser.Label = "Mi Ubicación";
            pinUser.Type = PinType.Place;
            pinUser.Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("pin_user.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "pin_user.png", WidthRequest = 64, HeightRequest = 64 });
            pinUser.Position = positionSupermarket;
            pinUser.IsDraggable = true;

            map.Pins.Add(pinUser);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(positionSupermarket, Xamarin.Forms.GoogleMaps.Distance.FromMeters(700)));
        }

        private void ApplyMapTheme()
        {
            var assambly = typeof(MainPage).GetTypeInfo().Assembly;
            var stream = assambly.GetManifestResourceStream($"SuperEconomicoApp.MapResources.MapTheme.json");
            string themeFile;
            using (var reader = new System.IO.StreamReader(stream))
            {
                themeFile = reader.ReadToEnd();
                map.MapStyle = MapStyle.FromJson(themeFile); //Leer hasta el final y aplicar el estilo
            }
        }

        public async Task ConfirmUbication()
        {
            await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual", "Ok");
        }
        #endregion

        #region COMANDOS
        public ICommand ConfirmUbicationCommand => new Command(async () => await ConfirmUbication());
        #endregion
    }
}

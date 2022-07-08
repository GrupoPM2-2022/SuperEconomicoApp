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
        string _Description;
        Pin pinUser = new Pin();
        Map map;
        private double latitudeSupermarket;
        private double longitudeSupermarket;
        private string latitudeUser;
        private string longitudeUser;
        private string coordinatesSupermarket;
        private string coordinatesUser;
        GoogleServiceApi googleServiceApi;
        GoogleDistanceMatrix googleDistanceMatrix;
        DirectionService directionService;
        Direction directionReference;

        private bool _IsEnabledButton = false;
        private string _TextButton;


        #endregion

        #region CONSTRUCTOR
        public AddDirectionViewModel(Map mapReference, string action, Direction direction)
        {
            TextButton = action;
            if (action.Equals("Editar"))
            {
                directionReference = direction;
                Description = direction.description;
            }
            map = mapReference;
            googleServiceApi = new GoogleServiceApi();
            googleDistanceMatrix = new GoogleDistanceMatrix();
            directionService = new DirectionService();
            LoadConfiguration();
            map.PinDragEnd += Map_PinDragEnd;
        }

        #endregion

        #region OBJETOS
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                OnPropertyChanged();
            }
        }
        public string TextButton
        {
            get { return _TextButton; }
            set
            {
                _TextButton = value;
                OnPropertyChanged();
            }
        }
        public bool IsEnabledButton
        {
            get { return _IsEnabledButton; }
            set
            {
                _IsEnabledButton = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region PROCESOS
        private async void Map_PinDragEnd(object sender, PinDragEventArgs e)
        {
            latitudeUser = e.Pin.Position.Latitude.ToString();
            longitudeUser = e.Pin.Position.Longitude.ToString();
            var position = new Position(e.Pin.Position.Latitude, e.Pin.Position.Longitude);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Xamarin.Forms.GoogleMaps.Distance.FromMeters(400)));
            coordinatesUser = latitudeUser + "," + longitudeUser;
            googleDistanceMatrix = await googleServiceApi.CalculateDistanceTwoCoordinates(coordinatesSupermarket, coordinatesUser);

            int meters = googleDistanceMatrix.rows[0].elements[0].distance.value;

            double kilometers = meters / 1000;
            if (kilometers > VALID_KILOMETERS)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual", "Ok");
                IsEnabledButton = false;
            }
            else
            {
                if (TextButton.Equals("Editar") && Description.Equals(directionReference.description))
                {
                    Description = "";
                }
                IsEnabledButton = true;
            }
        }

        private void LoadConfiguration()
        {
            //ApplyMapTheme();
            var coordinates = Settings.Coordinates.Split(',');
            coordinatesSupermarket = coordinates[0] + "," + coordinates[1];

            if (TextButton.Equals("Guardar"))
            {
                latitudeSupermarket = Convert.ToDouble(coordinates[0]);
                longitudeSupermarket = Convert.ToDouble(coordinates[1]);
            }
            else
            {
                latitudeSupermarket = Convert.ToDouble(directionReference.latitude);
                longitudeSupermarket = Convert.ToDouble(directionReference.longitude);
            }

            var positionMap = new Position(latitudeSupermarket, longitudeSupermarket);

            pinUser.Label = "Mi Ubicación";
            pinUser.Type = PinType.Place;
            pinUser.Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("pin_user.png") : BitmapDescriptorFactory.FromView(new Image() { Source = "pin_user.png", WidthRequest = 64, HeightRequest = 64 });
            pinUser.Position = positionMap;
            pinUser.IsDraggable = true;

            map.Pins.Add(pinUser);
            map.MoveToRegion(MapSpan.FromCenterAndRadius(positionMap, Xamarin.Forms.GoogleMaps.Distance.FromMeters(700)));
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
            if (string.IsNullOrEmpty(Description))
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Ingresa una descripción de tu dirección.", "Ok");
                return;
            }

            Direction direction = new Direction()
            {
                id = 0,
                description = Description,
                id_user = Convert.ToInt32(Settings.IdUser),
                latitude = latitudeUser,
                longitude = longitudeUser
            };

            if (TextButton.Equals("Guardar"))
            {
                CreateDirection(direction);
            } else
            {
                direction.id = directionReference.id;
                UpdateDirection(direction);
            } 

        }

        private async void UpdateDirection(Direction direction)
        {
            bool response = await directionService.UpdateDirection(direction);
            if (response)
            {
                await Application.Current.MainPage.DisplayAlert("Confirmación", "Dirección Actualizada Correctamente", "Ok");
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al actualizar su dirección", "Ok");
            }
        }

        public async void CreateDirection(Direction direction) {
            bool response = await directionService.CreateDirection(direction);
            if (response)
            {
                await Application.Current.MainPage.DisplayAlert("Confirmación", "Dirección Agregada Correctamente", "Ok");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al agregar su dirección", "Ok");
            }
        }

        #endregion

        #region COMANDOS
        public ICommand ConfirmUbicationCommand => new Command(async () => await ConfirmUbication());
        #endregion
    }
}

using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class AccountUserViewModels : BaseViewModel
    {
     
        private List<Department> _ListDepartment;
        private UserService userService;
        private User user;
        private GoogleDistanceMatrix googleDistanceMatrix;
        private GoogleServiceApi googleServiceApi;
        private string _Name;
        private string _Lastname;
        private string _DepartamentPreview;
        private string _Email;
        private byte[] _Image;
        private bool _ImageDefault;
        private bool _ImageBD;
        private bool _IsEnabledUpdate = false;
        private string departmentCoordinates;

        #region Objetos
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        public string Lastname
        {
            get { return _Lastname; }
            set
            {
                _Lastname = value;
                OnPropertyChanged();
            }
        }

        public string DepartamentPreview
        {
            get { return _DepartamentPreview; }
            set
            {
                _DepartamentPreview = value;
                OnPropertyChanged();
            }
        }


        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                OnPropertyChanged();
            }
        }
        public byte[] Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                OnPropertyChanged();
            }
        }

        public bool IsImageDefault
        {
            get { return _ImageDefault; }
            set
            {
                _ImageDefault = value;
                OnPropertyChanged();
            }
        }

        public bool IsImageBD
        {
            get { return _ImageBD; }
            set
            {
                _ImageBD = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabledUpdate
        {
            get { return _IsEnabledUpdate; }
            set
            {
                _IsEnabledUpdate = value;
                OnPropertyChanged();
            }
        }

        public List<Department> ListDepartment
        {
            get { return _ListDepartment; }
            set
            {
                _ListDepartment = value;
                OnPropertyChanged("ListDepartment");
            }
        }
        #endregion

        public AccountUserViewModels()
        {
            userService = new UserService();
            googleDistanceMatrix = new GoogleDistanceMatrix();
            googleServiceApi = new GoogleServiceApi();

            GetUserById();
            GetAllDepartments();
        }


        #region PROCESOS
        public async void GetUserById()
        {

            if (await userService.GetUserById() == null)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al obtener la informacion del usuario", "Ok");
            }
            else
            {
                user = await userService.GetUserById();
                Name = user.name;
                Lastname = user.lastname;
                Email = user.email;

                if (user.image == null || user.image.Length == 0)
                {
                    IsImageDefault = true;
                    IsImageBD = false;
                }
                else
                {
                    IsImageDefault = false;
                    IsImageBD = true;
                    Image = user.image;
                }
            }

        } 

        private void GetAllDepartments()
        {
            DepartamentPreview = Settings.Department;
            ListDepartment = new List<Department>()
            {
                new Department()
                {
                    Id = 1,
                    Name = "San Pedro Sula",
                    Latitude = 15.494080,
                    Longitude = -88.033686
                },
                new Department()
                {
                    Id = 2,
                    Name = "Tegucigalpa",
                    Latitude = 14.088415,
                    Longitude = -87.184149
                }
            };
        }

        private async Task LogoutAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.LogoutView());
        }

        private async Task EditUserAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.EditUserViews(user));
        }

        private void SelectDeparment(Department department)
        {
            DepartamentPreview = department.Name;
            if (DepartamentPreview.Equals(Settings.Department))
            {
                IsEnabledUpdate = false;
                return;
            }
            IsEnabledUpdate = true;
            departmentCoordinates = department.Latitude + " " + department.Longitude; 
        }

        private async Task UpdateDepartment()
        {
            if (DepartamentPreview.Equals(""))
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes seleccionar un departamento para actualizarlo", "Ok");
                return;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                IsValidDistance();
            }
            else
            {
                var request = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (request == PermissionStatus.Granted)
                {
                    IsValidDistance();
                }
            }

        }

        private async void IsValidDistance()
        {
            var location = await Geolocation.GetLocationAsync();
            if (location != null)
            {
                string coordinatesUser = location.Latitude.ToString() + "," + location.Longitude.ToString();
                googleDistanceMatrix = await googleServiceApi.CalculateDistanceTwoCoordinates(departmentCoordinates, coordinatesUser);

                int meters = googleDistanceMatrix.rows[0].elements[0].distance.value;

                double kilometers = meters / 1000;
                if (kilometers > Constants.VALID_KILOMETERS)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Nuestra cobertura no alcanza hasta tu ubicación actual", "Ok");
                    return;
                }
                else {
                    Settings.Department = DepartamentPreview;
                    Settings.Coordinates = departmentCoordinates;
                    IsEnabledUpdate = false;
                    await Application.Current.MainPage.DisplayAlert("Confirmacion", "Departamento actualizado correctamente "+ Settings.Coordinates+" "+ Settings.Department, "Ok");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "No podemos obtener tu localizacion actual.", "Ok");
            }
        }

        #endregion

        #region COMANDOS
        public ICommand EditUserCommand => new Command(async () => await EditUserAsync());
        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());
        public ICommand SelectDeparmentCommand => new Command<Department>((departmentReceived) => SelectDeparment(departmentReceived));
        public ICommand UpdateDepartmentCommand => new Command(async () => await UpdateDepartment());

        #endregion


    }
}

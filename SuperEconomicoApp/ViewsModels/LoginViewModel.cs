using Plugin.CloudFirestore;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Delivery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class LoginViewModel : BaseViewModel
    {
        const string ER_EMAIL = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        private string _Email;
        public string Email
        {
            set
            {
                this._Email = value;
                OnPropertyChanged();
            }
            get
            {
                return this._Email;
            }
        }

        private string _Password;
        public string Password
        {
            set
            {
                this._Password = value;
                OnPropertyChanged();
            }
            get
            {
                return this._Password;
            }
        }


        private bool _IsBusy;
        public bool IsBusy
        {
            set
            {
                this._IsBusy = value;
                OnPropertyChanged();
            }
            get
            {
                return this._IsBusy;
            }
        }

        private bool _Result;
        public bool Result
        {
            set
            {
                this._Result = value;
                OnPropertyChanged();
            }
            get
            {
                return this._Result;
            }
        }



        private bool _Disable;
        public bool Disable
        {
            set
            {
                _Disable = value;
                OnPropertyChanged();
            }

            get
            {
                return _Disable;
            }
        }

        public Command LoginCommand { get; set; }
        public Command RegisterCommand { get; set; }
        UserService userService;

        public LoginViewModel()
        {
            Disable = false;
            userService = new UserService();

            LoginCommand = new Command(async () => await LoginCommandAsync());
            RegisterCommand = new Command(async () => await RegisterCommandAsync());
        }

        private async Task RegisterCommandAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.AddUser());
        }

        private async Task LoginCommandAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;
                if (!Util.CheckConnectionInternet())
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "No tienes conexion a internet.", "OK");
                    return;
                }
                if (!Regex.IsMatch(Email, ER_EMAIL))
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "El correo electrónico no es valido.", "OK");
                    return;
                }

                User user = new User();
                user = await userService.GetUserByEmail(Email);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Email y/o Password incorrectos.", "OK");
                    return;
                }

                if (Email.Equals(user.email) && Password.Equals(user.password))
                {
                    bool resp = await UpdateTokenFirebase(user);
                    if (!resp)
                    {
                        await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error inesperado (Token)", "OK");
                        return;
                    }

                    if (user.typeuser.Equals("repartidor"))
                    {
                        bool response = await CheckExistUserFirebase(user);
                        if (response)
                            await Application.Current.MainPage.Navigation.PushModalAsync(new TabbedDeliveryView());
                    }
                    else
                    {
                        Settings.UserName = user.name + " " + user.lastname;
                        Settings.IdUser = user.id.ToString();
                        Settings.TypeUser = user.typeuser;
                        await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());
                    }

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Email y/o Password incorrectos.", "OK");
                }

            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> UpdateTokenFirebase(User user)
        {
            var value = Preferences.Get("TokenFirebase", "No existe");
            user.cod_firebase = value;
            bool response = await userService.UpdateUser(user);
            return response;
        }

        private async Task<bool> CheckExistUserFirebase(User user)
        {
            string idUser = user.id.ToString();
            var document = await CrossCloudFirestore.Current
                                        .Instance
                                        .Collection("Ubication")
                                        .Document(idUser)
                                        .GetAsync();

            if (!document.Exists)
            {
                var statusCheck = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (statusCheck == PermissionStatus.Granted)
                {
                    if (!await UserService.SaveUserFirebase(idUser))
                    {
                        await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error con tu cuenta, contactate con soporte.", "Ok");
                        return false;
                    }
                    else
                    {
                        Settings.UserName = user.name + " " + user.lastname;
                        Settings.IdUser = idUser;
                        Settings.TypeUser = user.typeuser;
                        return true;
                    }

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Debes conceder permisos de localizacion a la aplicacion.", "Ok");
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    return false;
                }

            }
            else
            {
                Model.Ubication ubication = document.ToObject<Model.Ubication>();
                Settings.UserName = user.name + " " + user.lastname;
                Settings.IdUser = idUser;
                Settings.TypeUser = user.typeuser;
                Settings.StatusDelivery = ubication.status;
                return true;
            }
        }


    }
}
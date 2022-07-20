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
        #region Variables
        const string ER_EMAIL = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        private string _Email;
        private string _Password;
        private bool _IsBusy;
        private bool _Result;
        private bool _Disable;
        User user;
        UserService userService;
        #endregion

        #region Objetos
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
        #endregion

        #region Comandos
        public Command LoginCommand { get; set; }
        public Command RegisterCommand { get; set; }
        #endregion

        public LoginViewModel()
        {
            Disable = false;
            userService = new UserService();

            LoginCommand = new Command(async () => await LoginCommandAsync());
            RegisterCommand = new Command(async () => await RegisterCommandAsync());
            user = new User();
        }

        #region PROCESOS
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

                user = await userService.GetUserByEmail(Email);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Email y/o Password incorrectos.", "OK");
                    return;
                }

                if (Email.Equals(user.email) && Password.Equals(user.password))
                {
                    var value = DependencyService.Get<Model.IFileService>().GetTextFile();
                    if (!value.Equals(user.cod_firebase) || string.IsNullOrEmpty(user.cod_firebase))
                    {
                        bool resp = await UpdateTokenFirebase(user, value);
                        if (!resp)
                        {
                            await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error inesperado (Token)", "OK");
                            return;
                        }
                    }

                    if (user.typeuser.Equals("repartidor"))
                    {
                        bool response = await CheckExistUserFirebase(user);
                        if (response)
                            Application.Current.MainPage = new TabbedDeliveryView();
                            //await Application.Current.MainPage.Navigation.PushModalAsync(new TabbedDeliveryView());
                    }
                    else
                    {
                        Settings.UserName = user.name + " " + user.lastname;
                        Settings.IdUser = user.id.ToString();
                        Settings.TypeUser = user.typeuser;
                        Application.Current.MainPage = new ProductsView();
                        //await Application.Current.MainPage.Navigation.PushModalAsync(new ProductsView());

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

        private async Task<bool> UpdateTokenFirebase(User user, string value)
        {
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

        #endregion

    }
}
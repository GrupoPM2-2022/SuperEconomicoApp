using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Services;
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

        public LoginViewModel()
        {
            Disable = false;

            LoginCommand = new Command(async () => await LoginCommandAsync());
            RegisterCommand = new Command(async () => await RegisterCommandAsync());

            var value = Preferences.Get("TokenFirebase", "No existe");
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
                if (!Regex.IsMatch(Email, ER_EMAIL))
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "El correo electrónico no es valido.", "OK");
                    return;
                }

                var userService = new UserService();
                User user = new User();
                user = await userService.GetUserByEmail(Email);
                if (user == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Email y/o Password incorrectos.", "OK");
                    return;
                }

                if (Email.Equals(user.email) && Password.Equals(user.password))
                {
                    Settings.UserName = user.name + " " + user.lastname;
                    Settings.IdUser = user.id.ToString();
                    await Application.Current.MainPage.Navigation.PushModalAsync(new Views.ProductsView());
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
    }
}
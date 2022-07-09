using SuperEconomicoApp.Model;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class UserAccountModels : BaseViewModel
    {
        public Command EditUserCommand { get; set; }
        public Command LogoutCommand { get; set; }
        private UserService userService;
        private User user;
        private string _Name;
        private string _Lastname;
        private string _Email;
        private byte[] _Image;
        private bool _ImageDefault;
        private bool _ImageBD;

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

        public UserAccountModels()
        {
            userService = new UserService();
            EditUserCommand = new Command(async () => await EditUserAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
            GetUserById();
        }


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

        private async Task LogoutAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.LogoutView());
        }

        private async Task EditUserAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.EditUserViews(user));
        }


    }
}

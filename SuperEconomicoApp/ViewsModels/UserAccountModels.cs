using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class UserAccountModels
    {
        public ObservableCollection<UserAccount> Users { get; set; }
        public Command EditUserCommand { get; set; }
        public Command LogoutCommand { get; set; }




        public UserAccountModels()
        {
            EditUserCommand = new Command(async () => await EditUserAsync());
            LogoutCommand = new Command(async () => await LogoutAsync());
            //Users = new ObservableCollection<UserAccount>
            //{
            //    new UserAccount
            //    {
            //        Nombre ="Levin Mauricio",
            //        Apellido="Lainez Aguirre",
            //        foto="person2",
            //        Correo="sile.lainez@gmail.com"
            //    }
            //};



        }

        private async Task LogoutAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.LogoutView());
        }

        private async Task EditUserAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.EditUserViews());
        }


    }
}

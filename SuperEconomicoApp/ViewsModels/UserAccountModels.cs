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

       

     
        public UserAccountModels()
        {
            EditUserCommand = new Command(async () => await EditUserAsync());
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

        private async Task EditUserAsync()
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.EditUserViews());
        }


    }
}

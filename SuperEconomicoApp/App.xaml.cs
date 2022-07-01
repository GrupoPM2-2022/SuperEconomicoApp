using SuperEconomicoApp.Helpers;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using SuperEconomicoApp.Views.Ubication;

namespace SuperEconomicoApp
{
    public partial class App : Application
    {
        public App()
        {
            Device.SetFlags(new string[] {
                "AppTheme_Experimental",
                "MediaElement_Experimental"
                });

            InitializeComponent();
            var cct = new CreateCartTable();
            if (!cct.CreateTable())
            {
                //DisplayAlert("Error", "Error al crear la tabla", "Ok");
            }
            Settings.UserName = "bAlvarez01";
            Settings.IdUser = "56";

            if (Settings.ExistUser)
            {
                MainPage = new ProductsView();
            }
            else
            {
                MainPage = new LoginView();
            }

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

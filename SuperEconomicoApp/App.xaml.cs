using SuperEconomicoApp.Helpers;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using SuperEconomicoApp.Views.Ubication;
using SuperEconomicoApp.Views.TabbedOrder;

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

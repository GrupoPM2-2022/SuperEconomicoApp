using SuperEconomicoApp.Helpers;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SuperEconomicoApp.Views;

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

            //MainPage = new Views.LoginView();
            //MainPage = new NavigationPage(new Views.LoginView());
            //MainPage = new NavigationPage(new Views.SettingsPage());

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

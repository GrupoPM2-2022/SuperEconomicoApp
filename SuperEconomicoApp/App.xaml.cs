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
            //MainPage = new AddUser();
            MainPage = new  NavigationPage(new AddUser());
            //MainPage = new NavigationPage(new Views.LoginView());
            //MainPage = new NavigationPage(new Views.SettingsPage());


         //   string uname = Preferences.Get("Username", String.Empty);
         //  if (String.IsNullOrEmpty(uname))
         // {
         //  MainPage = new Views.LoginView();
         //      //MainPage = new Views.ProductsView();
         //  }
         //  else
         // {
         //       MainPage = new Views.ProductsView();
         //}

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

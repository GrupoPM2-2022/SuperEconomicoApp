using SuperEconomicoApp.Helpers;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SuperEconomicoApp.Views;
using SuperEconomicoApp.Views.Reusable;
using SuperEconomicoApp.Views.Ubication;
using SuperEconomicoApp.Views.TabbedOrder;
using SuperEconomicoApp.Views.Delivery;

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
            //MainPage = new NavigationPage(new SettingsPage());

            if (Settings.ExistUser)
            {
                if (Settings.TypeUser.Equals("repartidor"))
                {
                    MainPage = new TabbedDeliveryView();
                }
                else
                {
                    MainPage = new ProductsView();
                }
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
            Settings.CurrentPage = "Delivery";
        }

        protected override void OnResume()
        {
        }
    }
}

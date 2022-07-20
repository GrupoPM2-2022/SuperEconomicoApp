using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.ViewsModels.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views.Delivery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveOrdersDeliveryView : ContentPage
    {
        public ActiveOrdersDeliveryView()
        {
            InitializeComponent();
            BindingContext = new ActiveOrdersDeliveryViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Settings.CurrentPage = "Delivery";
        }
    }
}
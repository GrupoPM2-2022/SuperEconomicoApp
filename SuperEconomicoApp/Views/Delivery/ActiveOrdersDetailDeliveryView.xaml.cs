using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
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
    public partial class ActiveOrdersDetailDeliveryView : ContentPage
    {
        //public ActiveOrdersDetailDeliveryView(ContentOrderDelivery order)
        //{
        //    InitializeComponent();
        //    BindingContext = new ActiveOrdersDetailDeliveryViewModel(order, map);
        //}

        ActiveOrdersDetailDeliveryViewModel activeOrderModels;
        public ActiveOrdersDetailDeliveryView(ContentOrderDelivery order)
        {
            InitializeComponent();
            activeOrderModels = new ActiveOrdersDetailDeliveryViewModel(order, map);
            BindingContext = activeOrderModels;
        }

        public void TraceRoute(string[] coordinatesOrigin, string[] coordinatesDestiny) {
            activeOrderModels.TraceRoute(coordinatesOrigin, coordinatesDestiny);
        }

    }

}
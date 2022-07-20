using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.ViewsModels.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views.Delivery
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrdersHistoryDeliveryView : ContentPage
    {
        OrdersHistoryDeliveryViewModel ordersHistoryViewModel;
        public OrdersHistoryDeliveryView()
        {
            InitializeComponent();
            ordersHistoryViewModel = new OrdersHistoryDeliveryViewModel();
            BindingContext = ordersHistoryViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ordersHistoryViewModel.LoadConfiguration();
        }
    }
}
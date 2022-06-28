using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmOrderView : ContentPage
    {
        public ConfirmOrderView(ObservableCollection<UserCartItem> listOrderDetails, Order order)
        {
            InitializeComponent();
            BindingContext = new ConfirmOrderViewModel(listOrderDetails, order);
        }
    }
}
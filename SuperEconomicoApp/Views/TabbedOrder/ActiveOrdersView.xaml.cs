using SuperEconomicoApp.ViewsModels.TabbedOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views.TabbedOrder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActiveOrdersView : ContentPage
    {
        public ActiveOrdersView()
        {
            InitializeComponent();
            BindingContext = new ActiveOrdersViewModel();
        }
    }
}
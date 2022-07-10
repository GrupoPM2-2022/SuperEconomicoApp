using SuperEconomicoApp.Model;
using SuperEconomicoApp.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowOrderDetailView : ContentPage
    {
        
        public ShowOrderDetailView(Order order)
        {
            InitializeComponent();
            BindingContext = new ShowOrderDetailViewModel(order, map);
        }
    }
}
using SuperEconomicoApp.ViewsModels.Ubication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views.Ubication
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDirectionView : ContentPage
    {
        public AddDirectionView()
        {
            InitializeComponent();
            BindingContext = new AddDirectionViewModel();
        }
    }
}
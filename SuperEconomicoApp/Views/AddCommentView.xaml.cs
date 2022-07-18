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
    public partial class AddCommentView : ContentPage
    {
        public AddCommentView(Order order)
        {
            InitializeComponent();
            BindingContext = new AddCommentViewModel(order);
        }
    }
}
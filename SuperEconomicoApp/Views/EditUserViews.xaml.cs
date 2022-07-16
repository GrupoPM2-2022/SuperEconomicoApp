using ImageCircle.Forms.Plugin.Abstractions;
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
    public partial class EditUserViews : ContentPage
    {
        public EditUserViews(User user)
        {
            InitializeComponent();
            BindingContext = new EditUserViewModels(user, imageForm);
            DateTime dateTime = DateTime.Now;
            txtBirthDate.MinimumDate = dateTime.AddYears(-118);
            txtBirthDate.MaximumDate = dateTime.AddYears(-18);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private void btnActualizar_Clicked(object sender, EventArgs e)
        {

        }
    }
}
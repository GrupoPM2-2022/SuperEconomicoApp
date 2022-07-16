using SuperEconomicoApp.Helpers;
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
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void login_Clicked(object sender, EventArgs e)
        {
            var cct = new CreateCartTable();
            if (!cct.CreateTable())
            {
                DisplayAlert("Error", "Error al crear la tabla", "Ok");
            }

        }

        private void btnRecoveryPass(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new SendRecoveryPass());
        }
    }
}
using SuperEconomicoApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void btnRecoveryPass(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new SendRecoveryPass());

            //var value = Preferences.Get("TokenFirebase", "No existe");

            //await DisplayAlert("", value, "OK");
        }
    }
}
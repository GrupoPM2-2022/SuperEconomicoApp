using Newtonsoft.Json;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendRecoveryPass : ContentPage
    {
        public SendRecoveryPass()
        {
            InitializeComponent();
        }

        private async void btnSendCodigo_Clicked(object sender, EventArgs e)
        {
            if (!ValidarCorreo(RecoveryEmail.Text))
            {
                await DisplayAlert("Notificacion", "Formato de correo invalido ", "OK");
                return;
            }

            UserService userService = new UserService();
            User user = await userService.GetUserByEmail(RecoveryEmail.Text);

            if (user == null)
            {
                await DisplayAlert("Notificacion", "Correo no existe", "OK");
                return ;
            }

            await Application.Current.MainPage.Navigation.PushModalAsync(new RecoveryPass(user));
        }

        public bool ValidarCorreo(string email)
        {
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }


    }
}
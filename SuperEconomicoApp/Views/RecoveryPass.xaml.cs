using Newtonsoft.Json;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecoveryPass : ContentPage

    {

        string code;
        User user;
        public RecoveryPass()
        {
            InitializeComponent();
            lblsendcodeMs.Text += "estebacerrasl";
        }
        public RecoveryPass(User user)
        {
            this.user = user;  
            InitializeComponent();
            sendEmail(user);
        }


        private async void sendEmail(User user)
        {
            try
            {
                var jsonData = new SuperEconomicoApp.Model.SendEmail();
                jsonData.asunto = "Recuperación de contraseña - SuperEconomicoApp";
                jsonData.contenido = this.code = numberRandom()+"";
                jsonData.destinatarioNombre = String.Concat(user.name, " ", user.lastname);
                jsonData.destinatario = user.email;

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(RestApiMethods.EndPointSendEmail);//mando a llamar la url del RestApi
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                var payload = JsonConvert.SerializeObject(jsonData);//convierto a json
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                request.Content = c;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await DisplayAlert("Notificación", "Correo enviado con exito", "OK");
                }
                else
                {
                    await DisplayAlert("Notificación", "Error al enviar correo", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Notificación", "Error al conectar", "OK");
            }

        }

        private int numberRandom()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        private async  void changePass_Clicked(object sender, EventArgs e)
        {
            UserService userService = new UserService();
            try
            {
                if (this.code != codeVeri.Text)
                {
                    await DisplayAlert("Notificacion", "Contraseña de verificación invalida", "OK");
                    return;
                }
                if(newPass.Text != repeatPass.Text)
                {
                    await DisplayAlert("Notificacion", "Contraseña ingresada no coinciden", "OK");
                    newPass.Text = repeatPass.Text = "";
                    newPass.Focus();
                    return;
                }
                this.user.password = newPass.Text;
                bool updateUser =  await userService.UpdateUser(this.user);
                if (updateUser)
                {
                    await DisplayAlert("Notificacion", "Contraseña actualizada", "OK");
                    var existingPages = Navigation.NavigationStack.ToList(); foreach (var page in existingPages) { Navigation.RemovePage(page); }
                    await Application.Current.MainPage.Navigation.PushModalAsync(new Views.LoginView());
                }
                else
                {
                    await DisplayAlert("Notificacion", "Usuario no actualizado", "OK");
                }
                
            }
            catch
            {
                
            }

        }

        private void validateData()
        {
            
        }
    }
}
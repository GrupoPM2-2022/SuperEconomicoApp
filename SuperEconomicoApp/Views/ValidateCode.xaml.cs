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
using System.Threading;
using System.Windows.Input;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ValidateCode : ContentPage
    {
        int Code;
        User user;
        public ValidateCode()
        {
            InitializeComponent();
           
        }

        public ValidateCode(string code, User user)
        {
            InitializeComponent();
            lblmensaje.Text = "Te hemos enviado un codigo de verificacion al correo: "+user.email+" que ingresaste, favor intruduce el codigo.";
            this.Code = int.Parse(code);
            this.user = user;
            sendEmail(user);
        }

        private void number1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(number1.Text.Length >= 1) { number2.Focus(); }
            
        }

        private void number2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (number2.Text.Length >= 1) { number3.Focus(); }
        }

        private void number3_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (number3.Text.Length >= 1) { number4.Focus(); }
        }

        private void number4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (number4.Text.Length >= 1) { btnVerifyCode.Focus();  btnVerifyCode.BackgroundColor = Color.FromHex("#2c3e50"); }
            if(string.IsNullOrEmpty(number1.Text) || string.IsNullOrEmpty(number2.Text) || string.IsNullOrEmpty(number3.Text) 
                || string.IsNullOrEmpty(number4.Text)) { btnVerifyCode.BackgroundColor = Color.Default; }
            
        }

        private void btnVerifyCode_Clicked(object sender, EventArgs e)
        {
            int concatCode = int.Parse(string.Concat(number1.Text, number2.Text, number3.Text, number4.Text));
            if (concatCode != this.Code)
            {
                DisplayAlert("Mensaje", "El codigo ingresado no es valido favor vuelva a intentarlo", "OK");
                number1.Text = "";
                number2.Text = "";
                number3.Text = "";
                number4.Text = "";
            }
            else
            {
                saveUser(this.user);
                //Thread.Sleep(3000);//RETARDO DE 3 SEGUNDOS PARA QUE GUARDE EN LA BASE DE DATOS
                var existingPages = Navigation.NavigationStack.ToList(); foreach (var page in existingPages) { Navigation.RemovePage(page); }
                Application.Current.MainPage.Navigation.PushModalAsync(new Views.LoginView());
            }
            

        }

        public async void saveUser(User datauser)
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(RestApiMethods.EndPointAddUser2);
                request.Method = HttpMethod.Post;
                request.Headers.Add("Accept", "application/json");
                var payload = JsonConvert.SerializeObject(datauser);
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                request.Content = c;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await DisplayAlert("Notificación", "El Usuario agregado con éxito: ", "OK");
                }
                else
                {
                    await DisplayAlert("Notificación", "Error al conectar", "OK");
                }
               
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Notificación", "Error " + ex, "OK");
            }
        }
        
        private async void sendEmail(User user)
        {
            try
            {
                var jsonData = new SuperEconomicoApp.Model.SendEmail();
                jsonData.asunto = "Codigo de verificación - SuperEconomicoApp";
                jsonData.contenido = user.cod_temp+"";
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
                    await DisplayAlert("Notificación", "Correo enviado con exito " + response, "OK");
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



        private void btnResendCode(object sender, EventArgs e)
        {
            sendEmail(this.user);
        }

        private async void btnEmail_incorrect(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;
using SuperEconomicoApp.Services;
using SuperEconomicoApp;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUser : ContentPage
    {
        public AddUser()
        {
            InitializeComponent();
            DateTime dateTime = DateTime.Now;
            txtbirthdate.MinimumDate = dateTime.AddYears(-118);
            txtbirthdate.MaximumDate = dateTime.AddYears(-18);
        }


        byte[] imageToSave;
        private async void AddImg(object sender, EventArgs e)
        {
            try
            {

                var takepic = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "PhotoApp",
                    Name = DateTime.Now.ToString() + "_foto.jpg",
                    SaveToAlbum = true
                });

                if (takepic != null)
                {
                    imageToSave = null;
                    MemoryStream memoryStream = new MemoryStream();

                    takepic.GetStream().CopyTo(memoryStream);
                    imageToSave = memoryStream.ToArray();

                    img.Source = ImageSource.FromStream(() => { return takepic.GetStream(); });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Se ha generado el siguiente error al agregar la imagen " + ex, "Aceptar");
            }
        }

        HttpClient user = new HttpClient();



        private void btnSave_Clicked(object sender, EventArgs e)
        {
            if(validateData() == true)
            {
                saveUser();
            }
        }

        private int numberRandom()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public async void saveUser()
        {
            try
            {
                var user = new User();
                user.name = txtname.Text;
                user.lastname = txtlastname.Text;
                user.phone = txtphone.Text;
                user.email = txtemail.Text;
                user.birthdate = txtbirthdate.Date;
                user.password = txtpassword.Text;
                user.image = imageToSave;
                user.state = "activo";
                user.typeuser = "cliente";
                user.cod_temp = numberRandom();
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(RestApiMethods.EndPointAddUser);//mando a llamar la url del RestApi
                request.Method = HttpMethod.Put;
                request.Headers.Add("Accept", "application/json");
                var payload = JsonConvert.SerializeObject(user);
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
                request.Content = c;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await DisplayAlert("Notificación", "El Usuario agregado con éxito: " + txtemail.Text, "OK");
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

        private bool validateData()
        {
            if (string.IsNullOrEmpty(txtname.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar su nombre ", "OK");
                txtname.Focus();
            }
            else if (string.IsNullOrEmpty(txtlastname.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar su apellido ", "OK");
                txtlastname.Focus();    
            }
            else if (string.IsNullOrEmpty(txtphone.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar su telefono ", "OK");
                txtphone.Focus();
            }
            else if (string.IsNullOrEmpty(txtemail.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar su correo electronico ", "OK");
                txtemail.Focus();
            }
            else if (string.IsNullOrEmpty(txtpassword.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar una contraseña para su cuenta " , "OK");
                txtpassword.Focus();
            } else if (txtpassword.Text.Length <= 5)
            {
                DisplayAlert("Advertencia", "La contraseña debe contener al menos 6 caracteres", "OK");
                txtpassword.Focus();
            }      
            else if(txtpassword.Text != txtpassword2.Text)
            {
                DisplayAlert("Advertencia", "La contraseña ingresada no coinciden, favor volver a ingresar", "OK");
                txtpassword.Text = "";
                txtpassword2.Text = "";
                txtpassword.Focus();
            }
            else
            {
                return true;
            }
            return false;   

        }
    }
}
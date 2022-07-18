using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using SuperEconomicoApp.Services;
using System.Text.RegularExpressions;
using Plugin.Media.Abstractions;

namespace SuperEconomicoApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddUser : ContentPage
    {
        HttpClient user = new HttpClient();
        byte[] imageToSave;
        string Code;

        public AddUser()
        {
            InitializeComponent();
            DateTime dateTime = DateTime.Now;
            txtbirthdate.MinimumDate = dateTime.AddYears(-118);
            txtbirthdate.MaximumDate = dateTime.AddYears(-18);
        }


        private async void AddImg(object sender, EventArgs e)
        {
            bool response = await Application.Current.MainPage.DisplayAlert("Advertencia", "Seleccione el tipo de imagen que desea", "Camara", "Galeria");

            if (response)
                GetImageFromCamera();
            else
                GetImageFromGallery();
        }

        private async void GetImageFromGallery()
        {
            try
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                    });

                    if (file == null)
                        return;

                    img.Source = ImageSource.FromStream(() => { return file.GetStream(); });
                    imageToSave = File.ReadAllBytes(file.Path);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al seleccionar la imagen.", "Ok");
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al seleccionar la imagen.", "Ok");
            }

        }

        private async void GetImageFromCamera()
        {
            try
            {
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                });

                if (file == null)
                    return;

                img.Source = ImageSource.FromStream(() => { return file.GetStream(); });
                imageToSave = File.ReadAllBytes(file.Path);
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al tomar la fotografia.", "Ok");
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            UserService userService = new UserService();
            User user = await userService.GetUserByEmail(txtemail.Text);

            if (user != null)
            {
                if (await DisplayAlert("Notificación", "Correo igresado ya fue registrado.\n \n ¿Desea recuperar su contraseña?", "SI", "NO"))
                {
                    await Application.Current.MainPage.Navigation.PushModalAsync(new SendRecoveryPass());
                }

                return;
            }

            if (validateData() == true)
            {
                this.Code = Convert.ToString(numberRandom());
                await Application.Current.MainPage.Navigation.PushModalAsync(new ValidateCode(this.Code, dataUser()));

            }
        }

        private int numberRandom()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public User dataUser()
        {
            User user = new User();
            user.name = txtname.Text;
            user.lastname = txtlastname.Text;
            user.phone = txtphone.Text;
            user.email = txtemail.Text;
            user.birthdate = txtbirthdate.Date;
            user.password = txtpassword.Text;
            user.image = imageToSave;
            user.state = "activo";
            user.typeuser = "cliente";
            user.cod_temp = this.Code;
            user.dni = txtdni.Text;
            return user;

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
            else if (string.IsNullOrEmpty(txtdni.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar su DNI", "OK");
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
            else if (!ValidarCorreo(txtemail.Text))
            {
                DisplayAlert("Advertencia", "Formato de correo invalido ", "OK");
                txtemail.Focus();
            }
            else if (string.IsNullOrEmpty(txtpassword.Text))
            {
                DisplayAlert("Campo obligatorio", "Favor ingresar una contraseña para su cuenta ", "OK");
                txtpassword.Focus();
            }
            else if (txtpassword.Text.Length <= 5)
            {
                DisplayAlert("Advertencia", "La contraseña debe contener al menos 6 caracteres", "OK");
                txtpassword.Focus();
            }
            else if (txtpassword.Text != txtpassword2.Text)
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

        public bool ValidarCorreo(string email)
        {
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }

    }
}
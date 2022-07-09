using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SuperEconomicoApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class EditUserViewModels : BaseViewModel
    {

        #region VARIABLES
        private DateTime _BirthDate;
        private string _Name;
        private string _Lastname;
        private string _Telephone;
        private string _Email;
        private string EmailCurrent;
        private byte[] _Image;
        private bool _IsImageBD;
        private CircleImage circleImage;
        UserService userService;
        #endregion

        public EditUserViewModels(User user, CircleImage imageForm)
        {
            circleImage = imageForm;
            userService = new UserService();
            LoadConfiguration(user);
        }

        #region OBJETOS
        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set { _BirthDate = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Lastname
        {
            get { return _Lastname; }
            set { _Lastname = value; }
        }

        public string Telephone
        {
            get { return _Telephone; }
            set { _Telephone = value; }
        }

        public string EmailNew
        {
            get { return _Email; }
            set { _Email = value; }
        }

        public byte[] Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        public bool IsImageBD
        {
            get { return _IsImageBD; }
            set { _IsImageBD = value; }
        }

        #endregion
        #region PROCESOS
        private async void SelectImage()
        {
            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());

            if (file == null)
                return;

            Image = File.ReadAllBytes(file.Path);
            circleImage.Source = ImageSource.FromStream(() => { return file.GetStream(); });
        }

        private void ShowTypeImage(string type)
        {
            if (type.Equals("default"))
            {
                circleImage.Source = "person2.png";
                circleImage.IsVisible = true;
                _IsImageBD = false;
            }
            else
            {
                circleImage.IsVisible = false;
                _IsImageBD = true;
            }
        }

        private async Task UpdateUser()
        {
            string response = ValidateFields();
            if (!response.Equals("Ok"))
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", response, "Ok");
                return;
            }

            if (!EmailNew.Equals(EmailCurrent))
            {
                if (await userService.GetUserByEmail(EmailNew) != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "El correo que ingresaste ya pertenece a otro usuario.", "Ok");
                    return;
                }
                else
                {

                }
            }


        }


        private string ValidateFields()
        {

            if (string.IsNullOrEmpty(Name))
            {
                return "Debes ingresar tu nombre.";
            }
            else if (string.IsNullOrEmpty(Lastname))
            {
                return "Debes ingresar tu apellido.";
            }
            else if (string.IsNullOrEmpty(Telephone))
            {
                return "Debes ingresar tu numero de telefono.";
            }
            else if (string.IsNullOrEmpty(EmailNew))
            {
                return "Debes ingresar tu correo eletronico.";
            }
            else if (BirthDate == null)
            {
                return "Debes ingresar tu fecha de nacimiento.";
            }
            else if (!EmailNew.Equals(EmailCurrent))
            {
                if (!ValidarCorreo(EmailNew))
                {
                    return "El correo electronico ingresado no es valido.";
                }
            }

            return "Ok";
        }

        private void LoadConfiguration(User user)
        {
            Name = user.name;
            Lastname = user.lastname;
            BirthDate = user.birthdate;
            EmailCurrent = user.email;
            EmailNew = user.email;
            Telephone = user.phone;

            if (user.image == null)
            {
                ShowTypeImage("default");
            }
            else
            {
                ShowTypeImage("principal");
                Image = user.image;
            }

        }

        public bool ValidarCorreo(string email)
        {
            Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }
        #endregion

        #region COMANDOS
        public ICommand SelectImageCommand => new Command(() => SelectImage());
        public ICommand UpdateUserCommand => new Command(async () => await UpdateUser());

        #endregion


    }
}

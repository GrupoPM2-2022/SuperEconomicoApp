using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SuperEconomicoApp.Services;
using SuperEconomicoApp.Views;
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
        private string _Dni;
        private byte[] _Image;
        private bool _IsImageBD;
        private CircleImage circleImage;
        UserService userService;
        User userSelected;
        #endregion

        public EditUserViewModels(User user, CircleImage imageForm)
        {
            circleImage = imageForm;
            userService = new UserService();
            userSelected = user;
            LoadConfiguration();
        }

        #region OBJETOS
        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set
            {
                _BirthDate = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        public string Lastname
        {
            get { return _Lastname; }
            set
            {
                _Lastname = value;
                OnPropertyChanged();
            }
        }

        public string Dni
        {
            get { return _Dni; }
            set
            {
                _Dni = value;
                OnPropertyChanged();
            }
        }


        public string Telephone
        {
            get { return _Telephone; }
            set
            {
                _Telephone = value;
                OnPropertyChanged();
            }
        }

        public string EmailNew
        {
            get { return _Email; }
            set
            {
                _Email = value;
                OnPropertyChanged();
            }
        }

        public byte[] Image
        {
            get { return _Image; }
            set
            {
                _Image = value;
                OnPropertyChanged();
            }
        }

        public bool IsImageBD
        {
            get { return _IsImageBD; }
            set
            {
                _IsImageBD = value;
                OnPropertyChanged();
            }
        }

        #endregion
        #region PROCESOS
        private async void SelectImage()
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

                    if (IsImageBD)
                    {
                        ShowTypeImage("default");
                    }

                    circleImage.Source = ImageSource.FromStream(() => { return file.GetStream(); });
                    Image = File.ReadAllBytes(file.Path);
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

                if (IsImageBD)
                    ShowTypeImage("default");

                circleImage.Source = ImageSource.FromStream(() => { return file.GetStream(); });
                Image = File.ReadAllBytes(file.Path);
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al tomar la fotografia.", "Ok");
            }
        }

        private void ShowTypeImage(string type)
        {
            if (type.Equals("default"))
            {
                circleImage.IsVisible = true;
                IsImageBD = false;
            }
            else
            {
                circleImage.IsVisible = false;
                IsImageBD = true;
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
            }

            userSelected.name = Name;
            userSelected.lastname = Lastname;
            userSelected.phone = Telephone;
            userSelected.image = Image;
            userSelected.email = EmailNew;
            userSelected.birthdate = BirthDate;
            userSelected.dni = Dni;

            bool confirmationUpdate = await userService.UpdateUser(userSelected);

            if (confirmationUpdate)
            {
                await Application.Current.MainPage.DisplayAlert("Confirmacion", "Usuario actualizado correctamente.", "Ok");
                await Application.Current.MainPage.Navigation.PushModalAsync(new AccountUserView());
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "Se produjo un error al actualizar el usuario.", "Ok");
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
            else if (string.IsNullOrEmpty(Dni))
            {
                return "Debes ingresar tu DNI.";
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

        private void LoadConfiguration()
        {
            Name = userSelected.name;
            Lastname = userSelected.lastname;
            BirthDate = userSelected.birthdate;
            EmailCurrent = userSelected.email;
            EmailNew = userSelected.email;
            Telephone = userSelected.phone;
            Dni = userSelected.dni;

            if (userSelected.image == null || userSelected.image.Length == 0)
            {
                circleImage.Source = "person2.png";
                ShowTypeImage("default");
            }
            else
            {
                ShowTypeImage("principal");
                Image = userSelected.image;
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

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
    public partial class ValidateCode : ContentPage
    {
        int Code;
        public ValidateCode()
        {
            InitializeComponent();
           
        }

        public ValidateCode(int code, string email)
        {
            InitializeComponent();
            lblmensaje.Text = "Te hemos enviado un codigo de verificacion al correo: "+email+" que ingresaste, favor intruduce el codigo";
            this.Code = code;
            DisplayAlert("DAtos", Code+"", "OK");
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
            string concatCode = string.Concat(number1.Text, number2.Text, number3.Text,number4.Text);
            DisplayAlert("Codigo", concatCode, "OK");
        }
    }
}
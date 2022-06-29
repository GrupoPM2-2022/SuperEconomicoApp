using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.ViewsModels
{
    public class PaymentMethodViewModel : BaseViewModel
    {

        #region VARIABLES
        public List<MethodPayment> listMethodPayment { get; set; }
        public ObservableCollection<UserCartItem> ListOrderDetails { get; set; }
        public Order OrderObject { get; set; }

        private string _TextButton = "Continuar";
        #endregion

        public string TextButton
        {
            get { return _TextButton; }
            set { 
                _TextButton = value;
                OnPropertyChanged();
            }
        }

        #region CONSTRUCTOR
        public PaymentMethodViewModel(ObservableCollection<UserCartItem> listOrderDetails, Order order)
        {
            ListOrderDetails = listOrderDetails;
            OrderObject = order;
            ShowPaymentMethods();
        }
        #endregion


        #region PROCESOS

        private void ShowPaymentMethods()
        {
            listMethodPayment = new List<MethodPayment>()
            {
                new MethodPayment()
                {
                    Name = "Efectivo",
                    Image = "efectivo.png"
                },
                new MethodPayment()
                {
                    Name = "Tarjeta de crédito/débito",
                    Image = "visa.png",
                }
            };
        }

        private void SelectedPayment(MethodPayment param) {
            if (!param.Name.Equals("Efectivo"))
            {
                TextButton = "Continuar Con Tarjeta";
                OrderObject.payment_type = param.Name;
                return;
            }
            TextButton = "Continuar Con Efectivo";
            OrderObject.payment_type = param.Name;
        }

        private async Task ConfirmOrder()
        {
            if (TextButton.Equals("Continuar"))
            {
                await Application.Current.MainPage.DisplayAlert("Title", "Selecciona un método de pago", "Ok");
                return;
            }
            await Application.Current.MainPage.Navigation.PushModalAsync(new Views.ConfirmOrderView(ListOrderDetails, OrderObject));
        }
        #endregion

        #region COMANDOS
        public Command SelectedPaymentCommand => new Command<MethodPayment> ((item) => SelectedPayment(item));
        public Command ConfirmOrderCommand => new Command(async () => await ConfirmOrder());

        
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                MessagingCenter.Subscribe<LocationMessage>(this, "Location", message =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        //UpdateUbicationDistributor(message.Latitude.ToString() + "," + message.Longitude.ToString());

                        // INDICAMOS AL DISPOSITIVO QUE INVOQUE EL SUBPROCESO PRINCIPAL Y ACTUALIZAR LAS COORDENADAS
                        locationLabel.Text += $"{Environment.NewLine}{message.Latitude}, {message.Longitude}, {DateTime.Now.ToLongTimeString()}";
                        Console.WriteLine($"{message.Latitude}, {message.Longitude}, {DateTime.Now.ToLongTimeString()}");
                    });
                });

                MessagingCenter.Subscribe<StopServiceMessage>(this, "ServiceStopped", message =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        locationLabel.Text = "Entrega Finalizada Exitoxamente!";
                    });
                });

                MessagingCenter.Subscribe<LocationErrorMessage>(this, "LocationError", message =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        locationLabel.Text = "Hubo un error al actualizar la ubicación!";
                    });
                });

                if (Preferences.Get("LocationServiceRunning", false) == true)
                {
                    Console.WriteLine("ENTRANDO A LA EJECUCION EN CASO DE REINICIO DEL DISPOSITIVO");
                    StartService();
                }
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //var permission = await Permissions.RequestAsync<Permissions.LocationAlways>();
            //if (permission == PermissionStatus.Denied)
            //{
            //    return;
            //}
            StartService();
        }

        private void BtnOrderDelivered_Clicked(object sender, EventArgs e)
        {
            StopService();
        }

        private void StartService()
        {
            var startServiceMessage = new StartServiceMessage();
            MessagingCenter.Send(startServiceMessage, "ServiceStarted");
            Preferences.Set("LocationServiceRunning", true);
            locationLabel.Text = "Se ha iniciado el servicio de ubicación!";
        }

        private void StopService()
        {
            var stopServiceMessage = new StopServiceMessage();
            MessagingCenter.Send(stopServiceMessage, "ServiceStopped");
            Preferences.Set("LocationServiceRunning", false);
        }
    }
}

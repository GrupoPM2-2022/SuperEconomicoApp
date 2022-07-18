using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace SuperEconomicoApp.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        AndroidNotificationManager androidNotification = new AndroidNotificationManager();
        public override void OnMessageReceived(RemoteMessage message)
        {

            //Log.Debug(TAG, "From: " + message.From);
            //Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);


            IDictionary<string, string> MessageData = message.Data;

            string title = MessageData["notiTitle"];
            string body = MessageData["notiBody"];

            

            androidNotification.CrearNotificacionLocal(title, body);
        }
        public override void OnNewToken(string token)
        {   
            //117.6.0.1
            base.OnNewToken(token);
            Preferences.Set("TokenFirebase", token);

            sedRegisterToken(token);
        }

      

        public void sedRegisterToken(string token)
        {
            //Tu código para registrar el token a tu servidor y base de datos

             Log.Debug("Token: ",token);
        }
    }
}
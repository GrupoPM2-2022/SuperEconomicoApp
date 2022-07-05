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

            Log.Debug(TAG, "From: " + message.From);
            Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
            androidNotification.CrearNotificacionLocal(message.GetNotification().Title, message.GetNotification().Body);
        }
        public override void OnNewToken(string token)
        {
            //dpXclu7QTk2yUNkeunOthh:APA91bEcBFMIMXRyEv2W9Gfjhbb-b2IUtMaZLC86v0lhEJBSEIeH0GwRsYBLSPWlGbEe0IE_T_b39OJz4ZdG6sRCY5uXfD-iNSc12Z-Bfsflho0GoDVgQCqgPaDFWf1XjNJgLlsW4L5w
            base.OnNewToken(token);
            //ehP0KiI1Rlyxy0uwJ5qs - z:APA91bEerPs5nKz2swuxI8SuFwdeKhYYoxh4bZGE4 - I2JvCPHdIKw2ATsVsSvXn - xY5HbUssC9y86lwhkDzwHQ9uMkzxu3Nqb - RdqFJbg75o9ROabqTMo43a0l2Dkdn84n6910tSU82U
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
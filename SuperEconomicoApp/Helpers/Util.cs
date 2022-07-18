using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace SuperEconomicoApp.Helpers
{
    public class Util
    {

        public static bool CheckConnectionInternet() {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                return true;
            }
            return false;
        }

    }
}

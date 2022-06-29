using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SuperEconomicoApp.Services
{
    public class RestApiMethods 
    {
        private static readonly string ipaddress = "192.168.0.3/movil2-RestApi/";
        private static readonly string StringHttp = "http://";

        public static readonly string EndPointAddUser = StringHttp + ipaddress + "CreateUser.php";
        public static readonly string EndPointAddUser2 = "https://dennisdomain.com/microservices/api_php/api/user/";

        public static readonly string EndPointSendEmail = "https://dennisdomain.com/microservices/email/email.php";
    }
}
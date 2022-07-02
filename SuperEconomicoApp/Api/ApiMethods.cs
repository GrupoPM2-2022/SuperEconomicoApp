using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Api
{
    public class ApiMethods
    {
        public static readonly string URL = "https://dennisdomain.com/microservices/api_php/api/";

        public static readonly string URL_ORDERS = URL + "orders/";

        public static readonly string URL_PRODUCTS = URL + "product/";

        public static readonly string GET_DIRECTION_BY_USER = URL + "userDirection/?id=";
        
        public static readonly string URL_DIRECTION = URL + "direction/";
        
        public static readonly string URL_USER = URL + "userMethod/";

    }
}

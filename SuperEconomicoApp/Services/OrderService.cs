using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperEconomicoApp.Api;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperEconomicoApp.Services
{
    public class OrderService
    {
        private static HttpClient client = new HttpClient();

        public OrderService()
        {
        }

        private void RemoveItemsFromCart()
        {
            var cis = new CartItemService();
            cis.RemoveItemsFromCart();
        }

        public async Task<bool> CreateOrder(Order order)
        {
            try
            {
                var coordinates = Settings.Coordinates.Split(',');
                Uri requestUri = new Uri(ApiMethods.URL_ORDERS + "?latitude=" + coordinates[0] + "&longitude=" + coordinates[1]);
                var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

                var json = JsonConvert.SerializeObject(order, settings);
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(requestUri, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    RemoveItemsFromCart();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR_INSERT_BD ->: " + ex.Message);
            }
            return false;
        }

        public async Task<OrdersByUser> GetOrdersUserByMethod(string method)
        {
            try
            {
                OrdersByUser ordersActiveByUser = new OrdersByUser();
                var uri = new Uri(ApiMethods.URL_ORDERS_USER + Settings.IdUser + "&method="+method);
                var response = await client.GetAsync(uri);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ordersActiveByUser = JsonConvert.DeserializeObject<OrdersByUser>(content);

                    return ordersActiveByUser;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

    }
}

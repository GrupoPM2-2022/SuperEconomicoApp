using Newtonsoft.Json;
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
                ////string parametersGet = "?latitude=" + coordinates[0] + "&longitude=" + coordinates[1];
                ////Uri requestUri = new Uri(ApiMethods.URL_ORDERS + parametersGet);
                //Uri requestUri = new Uri(ApiMethods.URL_ORDERS + "/");
                //var client = new HttpClient();
                //var json = JsonConvert.SerializeObject(order);
                //var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                //var result = await client.PostAsync(requestUri, contentJson);
                //if (result.StatusCode == HttpStatusCode.OK)
                //{
                //    RemoveItemsFromCart();
                //    return true;
                //}

                var client = new HttpClient();
                Uri requestUri = new Uri(ApiMethods.URL_ORDERS);
                client.DefaultRequestHeaders.Add("latitude", coordinates[0]);
                client.DefaultRequestHeaders.Add("longitude", coordinates[1]);

                var json = JsonConvert.SerializeObject(order);
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

    }
}

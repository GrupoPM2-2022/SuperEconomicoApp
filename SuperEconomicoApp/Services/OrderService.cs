using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using SuperEconomicoApp.Api;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

        public async Task<bool> CreateOrder(Order order) {
            try
            {
                Uri requestUri = new Uri(ApiMethods.URL_ORDERS);
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(order);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, contentJson);
                if (response.IsSuccessStatusCode)
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

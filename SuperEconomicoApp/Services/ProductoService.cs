using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperEconomicoApp.Model;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net.Http;
using SuperEconomicoApp.Api;
using Newtonsoft.Json;

namespace SuperEconomicoApp.Services
{
    public class ProductoService
    {
        private static HttpClient client = new HttpClient();

        public async static Task<ObservableCollection<ProductoItem>> GetAllProducts() {
            ObservableCollection<ProductoItem> products = new ObservableCollection<ProductoItem>();
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(ApiMethods.URL_PRODUCTS);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<ObservableCollection<ProductoItem>>(content);
                    return products;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

    }
}

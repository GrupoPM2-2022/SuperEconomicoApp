using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperEconomicoApp.Model;
using System.Linq;
using System.Net.Http;
using System.Collections.ObjectModel;
using SuperEconomicoApp.Api;
using Newtonsoft.Json;

namespace SuperEconomicoApp.Services
{
    public class CategoryService
    {
        private static HttpClient client = new HttpClient();

        public async static Task<ObservableCollection<Category>> GetAllCategories()
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(ApiMethods.URL_CATEGORY);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    categories = JsonConvert.DeserializeObject<ObservableCollection<Category>>(content);
                    return categories;
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


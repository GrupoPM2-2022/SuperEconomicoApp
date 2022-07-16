using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperEconomicoApp.Api;
using SuperEconomicoApp.Helpers;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SuperEconomicoApp.Services
{
    public class DirectionService
    {
        private static HttpClient client = new HttpClient();
        public DirectionService()
        {
        }

        public async Task<ObservableCollection<Direction>> GetDirectionByUser()
        {
            ObservableCollection<Direction> directions = new ObservableCollection<Direction>();

            var uri = new Uri(ApiMethods.GET_DIRECTION_BY_USER + Settings.IdUser + "&method=getDirectionForIdUser");

            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                JObject results = JObject.Parse(content);

                var result2 = results["directions"];

                foreach (var item in result2)
                {
                    Direction direction = new Direction()
                    {
                        id = Convert.ToInt32(item["id"].ToString()),
                        description = item["description"].ToString(),
                        latitude = item["latitude"].ToString(),
                        longitude = item["longitude"].ToString(),
                        id_user = Convert.ToInt32(item["id_user"].ToString())
                    };
                    directions.Add(direction);
                }
                Console.WriteLine(directions.ToString());
            }
            return directions;
        }


        public async Task<bool> DeleteDirection(string id)
        {
            var uri = new Uri(ApiMethods.URL_DIRECTION + "?id=" + id);
            var result = await client.DeleteAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CreateDirection(Direction direction)
        {
            Uri requestUri = new Uri(ApiMethods.URL_DIRECTION);
            var jsonObject = JsonConvert.SerializeObject(direction);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        
        public async Task<bool> UpdateDirection(Direction direction)
        {
            Uri requestUri = new Uri(ApiMethods.URL_DIRECTION+"?id="+direction.id);
            var jsonObject = JsonConvert.SerializeObject(direction);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

    }
}

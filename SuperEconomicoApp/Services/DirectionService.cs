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

        public DirectionService()
        {
        }

        public async Task<ObservableCollection<Direction>> GetDirectionByUser()
        {
            ObservableCollection<Direction> directions = new ObservableCollection<Direction>();

            var uri = new Uri(ApiMethods.GET_DIRECTION_BY_USER + Settings.IdUser + "&method=getDirectionForIdUser");
            HttpClient myClient = new HttpClient();

            var response = await myClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                JObject results = JObject.Parse(content);

                var result2 = results["directions"];

                foreach (var item in result2)
                {
                    Direction direction = new Direction()
                    {
                        Id = Convert.ToInt32(item["id"].ToString()),
                        Description = item["description"].ToString(),
                        Latitude = item["latitude"].ToString(),
                        Longitude = item["longitude"].ToString(),
                        IdUser = Convert.ToInt32(item["id_user"].ToString())
                    };
                    directions.Add(direction);
                }
                Console.WriteLine(directions.ToString());
            }
            return directions;
        }

    }
}

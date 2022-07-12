using Newtonsoft.Json;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SuperEconomicoApp.Services
{
    public class ApiServices
    {
        private static ApiServices _ServiceClientInstance;

        public static ApiServices ServiceClientInstance
        {
            get
            {
                if (_ServiceClientInstance == null)
                    _ServiceClientInstance = new ApiServices();
                return _ServiceClientInstance;
            }
        }

        private static HttpClient client = new HttpClient();
        public ApiServices()
        {
            client.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
        }

        public async Task<GoogleDirection> GetDirections(string originLatitude, String originLongitude, string destinationLatitude, string destinationLongitude)
        {
            GoogleDirection googleDirection = new GoogleDirection();

            var response = await client.GetAsync($"api/directions/json?transit_routing_preference=less_driving&origin={originLatitude}, {originLongitude}&destination={destinationLatitude},{destinationLongitude}&key={Constants.GOOGLE_API_KEY}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(json))  // IsNullOrWhiteSpace = Indica si una cadena está vacía o consta sólo de caracteres de espacio en blanco
                {
                    googleDirection = await Task.Run(() =>
                        JsonConvert.DeserializeObject<GoogleDirection>(json)
                    ).ConfigureAwait(false);
                }

            }

            return googleDirection;
        }
    }
}

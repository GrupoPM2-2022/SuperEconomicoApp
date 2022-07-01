using Newtonsoft.Json;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SuperEconomicoApp.Services
{
    public class GoogleServiceApi
    {
        private static HttpClient client = new HttpClient();
        public GoogleServiceApi()
        {
        }

        public async Task<GoogleDistanceMatrix> CalculateDistanceTwoCoordinates(string originCoordinates, string destinationCoordinates)
        {
            GoogleDistanceMatrix googleApi = new GoogleDistanceMatrix();
            string URL = "https://maps.googleapis.com/maps/api/distancematrix/json?origins="+ originCoordinates + "&destinations="+ destinationCoordinates + "&sensor=false&key="+Constants.GOOGLE_API_KEY;
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(URL);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                googleApi = JsonConvert.DeserializeObject<GoogleDistanceMatrix>(content);
            }

            return googleApi;
        }


    }
}

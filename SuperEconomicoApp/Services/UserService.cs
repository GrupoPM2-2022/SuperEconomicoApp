using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SuperEconomicoApp.Model;
using Firebase.Database.Query;
using System.Net.Http;
using SuperEconomicoApp.Api;
using Newtonsoft.Json;
using SuperEconomicoApp.Helpers;

namespace SuperEconomicoApp.Services
{
    public class UserService
    {
        private static HttpClient client = new HttpClient();

        public UserService()
        {
        }


        public async Task<User> GetUserByEmail(string email)
        {
            User user = new User();
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(ApiMethods.URL_USERMETHOD + "?value=" + email + "&method=getUserForEmail");
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.Equals("[]"))
                    {
                        return null;
                    }
                    user = JsonConvert.DeserializeObject<User>(content);
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<User> GetUserById()
        {
            User user = new User();
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(ApiMethods.URL_USER + "?id=" + Settings.IdUser);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.Equals("[]"))
                    {
                        return null;
                    }
                    user = JsonConvert.DeserializeObject<User>(content);
                }
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<bool> UpdateUser(User user)
        {
            
            Uri requestUri = new Uri(ApiMethods.URL_USER + "?id=" + user.id);
            var jsonObject = JsonConvert.SerializeObject(user);
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SuperEconomicoApp.Model;
using System.Net.Http;
using SuperEconomicoApp.Api;
using Newtonsoft.Json;
using SuperEconomicoApp.Helpers;
using Plugin.CloudFirestore;
using Xamarin.Essentials;

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
                Uri uri = new Uri(ApiMethods.URL_USERMETHOD + "?value=" + email + "&method=getUserForEmail");
                var response = await client.GetAsync(uri);
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
            try
            {
                User user = new User();
                var uri = new Uri(ApiMethods.URL_USER + "?id=" + Settings.IdUser);
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (content.Equals("[]"))
                    {
                        return null;
                    }
                    user = JsonConvert.DeserializeObject<User>(content);
                    return user;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR_USER ", ex.Message);
            }

            return null;
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                Uri requestUri = new Uri(ApiMethods.URL_USER + "?id=" + user.id);
                var jsonObject = JsonConvert.SerializeObject(user);
                var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public async static Task<bool> SaveUserFirebase(String idDocument)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    string coordinatesUser = location.Latitude.ToString() + "," + location.Longitude.ToString();
                    await CrossCloudFirestore.Current
                             .Instance
                             .Collection("Ubication")
                             .Document(idDocument)
                             .SetAsync(new Ubication
                             {
                                 status = "ACTIVO",
                                 ubication = coordinatesUser,
                             });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }


    }

}

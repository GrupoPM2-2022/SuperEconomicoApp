using Firebase.Database;
using Firebase.Database.Query;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperEconomicoApp.Helpers
{
    public class AddCategoryData
    {
        public List<Category> Categories { get; set; }

        FirebaseClient client;

        public AddCategoryData()
        {
            client = new FirebaseClient("https://supereconomicoapp-default-rtdb.firebaseio.com/");
            Categories = new List<Category>()
            {
                new Category(){
                    CategoryID = 1,
                    CategoryName = "Cervezas",
                    CategoryPoster = "cervezaMain",
                    ImageUrl = "cerveza.png"
                },
                new Category(){
                    CategoryID = 2,
                    CategoryName = "Carnes",
                    CategoryPoster = "carneMain",
                    ImageUrl = "carne.png"
                },
                new Category(){
                    CategoryID = 3,
                    CategoryName = "Pañales",
                    CategoryPoster = "panalesMain",
                    ImageUrl = "panal.png"
                },
                 new Category(){
                    CategoryID = 4,
                    CategoryName = "Leche",
                    CategoryPoster = "lecheMain",
                    ImageUrl = "leche.png"
                }
            };
        }

        public async Task AddCategoriesAsync()
        {
            try
            {
                foreach (var category in Categories)
                {
                    await client.Child("Categories").PostAsync(new Category()
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        CategoryPoster = category.CategoryPoster,
                        ImageUrl = category.ImageUrl
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

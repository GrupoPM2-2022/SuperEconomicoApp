using Firebase.Database;
using Firebase.Database.Query;
using SuperEconomicoApp.Model;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SuperEconomicoApp.Helpers
{
    public class AddProductoItemData
    {
        FirebaseClient client;

        public List<ProductoItem> ProductoItems { get; set; }

        public AddProductoItemData()
        {
            client = new FirebaseClient("https://supereconomicoapp-default-rtdb.firebaseio.com/");
            ProductoItems = new List<ProductoItem>()
            {
                  new ProductoItem
                {
                    ProductID = 1,
                    CategoryID = 4,
                    ImageUrl = "productoLeche22",
                    Name = "Leche Evaporada Ceteco ",
                    Description = "Entera - Deslactosa",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                    Price = 348
                },
                new ProductoItem
                {
                    ProductID = 2,
                    CategoryID = 1,
                    ImageUrl = "productoCerveza11",
                    Name = "Cervezas Hollandia",
                    Description = "Cervezas - Licores",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                    Price = 120
                },
                new ProductoItem
                {
                    ProductID = 3,
                    CategoryID = 2,
                    ImageUrl = "productoCarne11",
                    Name = "Carne para Estofado",
                    Description = "Vaca - Cerdo - Pollo",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "EmptyHeart",
                    Price = 62
                },
                new ProductoItem
                {
                    ProductID = 4,
                    CategoryID = 3,
                    ImageUrl = "productoPanales22",
                    Name = "Pañal Huggies",
                    Description = "Mediano - Grandes",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                    Price = 575
                },
                  new ProductoItem
                {
                    ProductID = 5,
                    CategoryID = 4,
                    ImageUrl = "productoLeche11",
                    Name = "Leche en Polvo Ceteco ",
                    Description = "Entera - Deslactosa",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                    Price = 348
                },
               
                new ProductoItem
                {
                    ProductID = 6,
                    CategoryID = 2,
                    ImageUrl = "productoCarne22",
                    Name = "Carne de Res para Asar",
                    Description = "Vaca - Cerdo - Pollo",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "EmptyHeart",
                    Price = 115
                },
                
                 new ProductoItem
                {
                    ProductID = 7,
                    CategoryID = 1,
                    ImageUrl = "productoCerveza3",
                    Name = "Cervezas Hollandia Lata",
                    Description = "Cervezas - Licores",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                    Price = 96
                },
                  new ProductoItem
                {
                    ProductID = 8,
                    CategoryID = 4,
                    ImageUrl = "productoLeche3",
                    Name = "Leche Crecimiento 1+",
                    Description = "Entera - Deslactosa",
                    Rating = " 4.8",
                    RatingDetail = " (121 raitings)",
                    HomeSelected = "CompleteHeart",
                   Price = 556
                  }
                //  new ProductoItem
                //{
                //    ProductID = 9,
                //    CategoryID = 4,
                //    ImageUrl = "lecheMain",
                //    Name = "Leche Crecimiento 1+",
                //    Description = "Entera - Deslactosa",
                //    Rating = " 4.8",
                //    RatingDetail = " (121 raitings)",
                //    HomeSelected = "CompleteHeart",
                //    Price = 556
                //},
                //new ProductoItem
                //{
                //    ProductID = 10,
                //    CategoryID = 1,
                //    ImageUrl = "cervezaMain",
                //    Name = "Cervezas Hollandia Lata",
                //    Description = "Cervezas - Licores - Vinos",
                //    Rating = " 4.8",
                //    RatingDetail = " (121 raitings)",
                //    HomeSelected = "CompleteHeart",
                //    Price = 96
                //},
                //new ProductoItem
                //{
                //    ProductID = 11,
                //    CategoryID = 2,
                //    ImageUrl = "carneMain",
                //    Name = "Carne Parrillada",
                //    Description = "Vaca - Cerdo - Pollo",
                //    Rating = " 4.8",
                //    RatingDetail = " (121 raitings)",
                //    HomeSelected = "EmptyHeart",
                //    Price = 170
                //},
                //new ProductoItem
                //{
                //    ProductID = 12,
                //    CategoryID = 3,
                //    ImageUrl = "panalesMain",
                //    Name = "Pañal Huggies Sec",
                //    Description = "Mediano - Grandes - Pequeños",
                //    Rating = " 4.8",
                //    RatingDetail = " (121 raitings)",
                //    HomeSelected = "CompleteHeart",
                //    Price = 305
                //}

             };
        }

        public async Task AddProductoItemAsync()
        {
            try
            {
                foreach (var item in ProductoItems)
                {
                    await client.Child("ProductoItems").PostAsync(new ProductoItem()
                    {
                        CategoryID = item.CategoryID,
                        ProductID = item.ProductID,
                        Description = item.Description,
                        HomeSelected = item.HomeSelected,
                        ImageUrl = item.ImageUrl,
                        Name = item.Name,
                        Price = item.Price,
                        Rating = item.Rating,
                        RatingDetail = item.RatingDetail
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

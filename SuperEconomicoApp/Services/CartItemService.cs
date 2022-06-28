using System;
using SuperEconomicoApp.Model;
using Xamarin.Forms;

namespace SuperEconomicoApp.Services
{
    public class CartItemService
    {
        public int GetUserCartCount()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var count = cn.Table<CartItem>().Count();
            cn.Close();
            return count;
        }

        public void RemoveItemsFromCart()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            cn.DeleteAll<CartItem>();
            cn.Commit();
            cn.Close();
        }

        public void RemoveProductById(UserCartItem item) {
            var connection = DependencyService.Get<ISQLite>().GetConnection();

            CartItem cartItem = new CartItem()
            {
                CartItemId = item.CartItemId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,
                ImageProduct = item.ImageProduct,
            };
            connection.Delete(cartItem);
            connection.Commit();
            connection.Close();
        }
    }
}
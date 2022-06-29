using System;
using System.Linq;
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

        public void RemoveProductById(UserCartItem item)
        {
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

        public void AddProductTocart(UserCartItem productoItem, int totalQuantity)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            try
            {
                CartItem ci = new CartItem()
                {
                    ProductId = productoItem.ProductId,
                    ProductName = productoItem.ProductName,
                    Price = productoItem.Price,
                    Quantity = totalQuantity,
                    ImageProduct = productoItem.ImageProduct,
                    Stock = productoItem.Stock
                };
                var item = cn.Table<CartItem>().ToList().FirstOrDefault(c => c.ProductId == productoItem.ProductId);

                if (item == null)
                {
                    cn.Insert(ci);
                }
                else
                {
                    item.Quantity = totalQuantity;
                    cn.Update(item);
                }

                cn.Commit();
            }
            catch (Exception)
            {
                throw;
            } finally
            {
                cn.Close();
            }
        }
    }
}
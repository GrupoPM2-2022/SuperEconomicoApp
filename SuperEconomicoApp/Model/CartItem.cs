using SQLite;


namespace SuperEconomicoApp.Model
{
    [Table("CartProducts")]
    public class CartItem
    {
        [AutoIncrement, PrimaryKey]
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string ImageProduct { get; set; }
    }
}

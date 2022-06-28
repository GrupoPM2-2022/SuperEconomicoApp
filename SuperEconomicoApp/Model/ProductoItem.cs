using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public class ProductoItem
    {
        public int Product_Id { get; set; }
        public int Category_Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int Stock { get; set; }
        public double Discount { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{

    public class OrderDetails
    {
        public int product_id { get; set; }
        public int quantity { get; set; }
        public string name_product { get; set; }
        public double price { get; set; }
        public double discount { get; set; }

    }
}

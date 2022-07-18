using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{

    public class OrderDetails
    {
        private double _price = 0.00;
        private double _discount = 0.00;

        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public string name_product { get; set; }
        public string image_product { get; set; }
        public double price { get { return _price; } set { _price = value; } }
        public double discount { get { return _discount; } set { _discount = value; } }

    }
}

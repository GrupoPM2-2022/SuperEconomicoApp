using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public class Order
    {
        public string order_id { get; set; }
        public int client_user_id{ get; set; }
        public int delivery_user_id { get; set; }
        public string order_date { get; set; }
        public string deliver_date { get; set; }
        public string score { get; set; }
        public string comment { get; set; }
        public double total { get; set; }
        public double full_discount { get; set; }
        public string client_location { get; set; }
        public string payment_type { get; set; }
        public string status { get; set; }
        public List<OrderDetails> orders_detail { get; set; }

    }
}

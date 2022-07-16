using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SuperEconomicoApp.Model
{
    public class Order
    {
        public string order_id { get; set; }
        public int client_user_id { get; set; }
        public int delivery_user_id { get; set; }
        public DateTime order_date { get; set; }
        public string deliver_date { get; set; }
        public string score { get; set; }
        public string comment { get; set; }
        public double total { get; set; }
        public double full_discount { get; set; }
        public string client_location { get; set; }
        public string payment_type { get; set; }
        public string status { get; set; }
        public string delivery_name { get; set; }
        public string delivery_lastname { get; set; }
        public string phone_delivery { get; set; }
        public byte[] delivery_image { get; set; }
        public string sucursal { get; set; }
        public List<OrderDetails> orders_detail { get; set; }
        
        [JsonIgnore]
        public string NameButton { get; set; }
        
        [JsonIgnore]
        public string ColorStatus { get; set; }
    }

    public class OrdersByUser
    {
        public string id { get; set; }
        public string password { get; set; }
        public string typeuser { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string image { get; set; }
        public string state { get; set; }
        public string birthdate { get; set; }
        public string phone { get; set; }
        public string conf_phone { get; set; }
        public IList<Order> orders { get; set; }
    }
}

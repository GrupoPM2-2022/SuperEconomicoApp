using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public class ContentOrderDelivery
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("client_user_id")]
        public int ClientUserId { get; set; }

        [JsonProperty("delivery_user_id")]
        public int DeliveryUserId { get; set; }

        [JsonProperty("order_date")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("deliver_date")]
        public DateTime DeliveryDate { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("full_discount")]
        public double FullDiscount { get; set; }

        [JsonProperty("client_location")]
        public string ClientLocation { get; set; }
        
        [JsonProperty("name_ubication")]
        public string NameUbication { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sucursal")]
        public string Sucursal { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("client_lastname")]
        public string ClientLastname { get; set; }

        [JsonProperty("phone_client")]
        public string PhoneClient { get; set; }

        [JsonProperty("client_image")]
        public byte[] ClientImage { get; set; }

        [JsonProperty("orders_detail")]
        public List<OrderDetails> OrdersDetail { get; set; }

        [JsonIgnore]
        public bool IsVisibleStatus { get; set; }

        [JsonIgnore]
        public string TextButton { get; set; }

    }

    public class OrdersDelivery
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("typeuser")]
        public string TypeUser { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("birthdate")]
        public string BirthDate { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("conf_phone")]
        public string ConfPhone { get; set; }

        [JsonProperty("dni")]
        public string Dni { get; set; }

        [JsonProperty("cod_firebase")]
        public string CodFirebase { get; set; }

        public IList<ContentOrderDelivery> orders { get; set; }

    }
}

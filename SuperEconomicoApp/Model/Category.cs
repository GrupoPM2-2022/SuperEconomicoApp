using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public class Category
    {
        [JsonProperty("category_id")]
        public int CategoryID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public byte[] Image { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

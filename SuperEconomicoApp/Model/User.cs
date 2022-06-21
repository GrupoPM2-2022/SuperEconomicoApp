using System;
using System.Collections.Generic;
using System.Text;

namespace SuperEconomicoApp.Model
{
    public class User
    {
        public int id { get; set; }

        public string name { get; set; }

        public string lastname { get; set; }

        public string birthdate { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public byte[] image { get; set; }

        public string state { get; set; }

        public string cod_temp { get; set; }

    }
}

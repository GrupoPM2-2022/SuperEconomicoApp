using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;

namespace SuperEconomicoApp
{
    public class User : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string nombre = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nombre));
        }

        public int id { get; set; }

        public string name { get; set; }

        public string lastname { get; set; }

        public DateTime birthdate { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public byte[] image { get; set; }

        public string state { get; set; }

        public string cod_temp { get; set; }

        public string phone { get; set; }

        public string typeuser { get; set; }

        public string conf_phone { get; set; }



    }
}
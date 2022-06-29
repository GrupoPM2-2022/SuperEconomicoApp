using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace SuperEconomicoApp.Model
{
    public class SendEmail
    {
       public string asunto { get; set; }
       public string contenido { get; set; }
       public string destinatario { get; set; }
       public string remitente { get; set; }
       public string destinatarioNombre { get; set; }
    }
}
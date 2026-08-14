using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arac_Kiralama.models
{
    public class kullanici
    {
        public int Id { get; set; }
        public string Telefon { get; set; }
        public string Sifre { get; set; }

        public string Eposta { get; set; }
    }
}

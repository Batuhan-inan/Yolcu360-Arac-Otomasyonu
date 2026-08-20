using System;

namespace Arac_Kiralama.models
{
    public class kullanici
    {
        public int Id { get; set; }
        public string Telefon { get; set; }
        public string Sifre { get; set; }
        public string Eposta { get; set; }
        public string Rol { get; set; } = "Musteri";
    }
}

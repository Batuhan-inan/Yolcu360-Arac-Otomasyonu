using System;

namespace Arac_Kiralama.models
{
    public class Odeme
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string AracModeli { get; set; }
        public string OdenenTutar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public string Durum { get; set; }
    }
}

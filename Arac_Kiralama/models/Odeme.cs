using System;

namespace Arac_Kiralama.models
{
    public class Odeme
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public string AracModeli { get; set; }
        public string KiralamaSirketi { get; set; }
        public string Fiyat { get; set; }
        public DateTime OdemeTarihi { get; set; }
        public string iyzicoPaymentId { get; set; }
        public string Durum { get; set; }
    }
}

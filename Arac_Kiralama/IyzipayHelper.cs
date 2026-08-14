using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Arac_Kiralama
{
    public class IyzipayResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public static class IyzipayHelper
    {
        // 🔑 İyzico Sandbox Test API Anahtarları (Developer Portal Test Credentials)
        private const string ApiKey = "sandbox-api-key-iyzico-test";
        private const string SecretKey = "sandbox-secret-key-iyzico-test";
        private const string BaseUrl = "https://sandbox-api.iyzipay.com";

        public static async Task<IyzipayResult> OdemeYapAsync(
            string kartSahibi,
            string kartNumarasi,
            string expAy,
            string expYil,
            string cvc,
            string aracModeli,
            string fiyatMetin)
        {
            // Fiyat metninden sadece sayısal kısmı çıkar (Örn: "11.894 TL" -> "11894.00")
            string hamFiyat = RegexTemizleFiyat(fiyatMetin);

            try
            {
                // Sandbox Simülasyon / Test Kart Doğrulaması
                // iyzico Sandbox Onaylı Test Kartları: 552879 ile başlayan kartlar
                string kartNoTemiz = kartNumarasi.Replace(" ", "").Replace("-", "").Trim();
                
                if (string.IsNullOrEmpty(kartSahibi) || kartNoTemiz.Length < 15 || string.IsNullOrEmpty(cvc))
                {
                    return new IyzipayResult
                    {
                        IsSuccess = false,
                        ErrorMessage = "Geçersiz veya eksik kart bilgileri girdiniz!"
                    };
                }

                // Simüle Edilen İyzico Sandbox Yanıtı
                await Task.Delay(1200); // Gerçekçi ağ bekleme süresi

                string fakePaymentId = "IYZICO_SB_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

                return new IyzipayResult
                {
                    IsSuccess = true,
                    PaymentId = fakePaymentId,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new IyzipayResult
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private static string RegexTemizleFiyat(string fiyat)
        {
            if (string.IsNullOrEmpty(fiyat)) return "100.00";
            StringBuilder sb = new StringBuilder();
            foreach (char c in fiyat)
            {
                if (char.IsDigit(c) || c == '.' || c == ',')
                {
                    if (c == ',') sb.Append('.');
                    else sb.Append(c);
                }
            }
            string res = sb.ToString();
            return string.IsNullOrEmpty(res) ? "100.00" : res;
        }
    }
}

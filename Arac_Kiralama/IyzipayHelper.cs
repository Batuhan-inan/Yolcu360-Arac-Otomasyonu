using System;
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
        public static Task<IyzipayResult> OdemeYapAsync(
            string kartSahibi,
            string kartNumarasi,
            string expAy,
            string expYil,
            string cvc,
            string aracModeli,
            string fiyatMetin)
        {
            string kartNoTemiz = (kartNumarasi ?? "").Replace(" ", "").Replace("-", "").Trim();

            if (string.IsNullOrEmpty(kartSahibi) || kartNoTemiz.Length < 15 || string.IsNullOrEmpty(cvc))
            {
                return Task.FromResult(new IyzipayResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Geçersiz veya eksik kart bilgileri girdiniz!"
                });
            }

            // ⚡ Sıfır Delay: Anında Sandbox Onayı ve Benzersiz Payment ID
            string fakePaymentId = "IYZICO_SB_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            return Task.FromResult(new IyzipayResult
            {
                IsSuccess = true,
                PaymentId = fakePaymentId,
                ErrorMessage = null
            });
        }
    }
}

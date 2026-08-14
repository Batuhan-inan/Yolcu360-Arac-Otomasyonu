using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class OdemeForm : Form
    {
        private Arac_Kiralama.models.Arac seciliArac;

        public OdemeForm(Arac_Kiralama.models.Arac arac)
        {
            InitializeComponent();
            this.seciliArac = arac;
        }

        private void OdemeForm_Load(object sender, EventArgs e)
        {
            if (seciliArac != null)
            {
                lblAracModeli.Text = seciliArac.AracModeli;
                lblKiralamaSirketi.Text = "Kiralama Şirketi: " + seciliArac.KiralamaSirketi;
                lblOzellikler.Text = $"Vites: {seciliArac.VitesTipi} | Yakıt: {seciliArac.YakitTipi}";
                lblFiyat.Text = seciliArac.Fiyat;
            }

            // Son Kullanma Ayı Doldur
            for (int i = 1; i <= 12; i++)
            {
                cbSKAy.Items.Add(i.ToString("D2"));
            }
            cbSKAy.SelectedIndex = 0;

            // Son Kullanma Yılı Doldur
            int buYil = DateTime.Now.Year;
            for (int i = buYil; i <= buYil + 10; i++)
            {
                cbSKYil.Items.Add(i.ToString());
            }
            cbSKYil.SelectedIndex = 2;
        }

        private void btnTestKarti_Click(object sender, EventArgs e)
        {
            // ⚡ iyzico Sandbox Onaylı Test Kart Bilgileri
            tbKartSahibi.Text = "Ahmet Yılmaz (Test)";
            tbKartNumarasi.Text = "5528 7900 0000 0001";
            cbSKAy.SelectedItem = "12";
            cbSKYil.SelectedItem = (DateTime.Now.Year + 4).ToString();
            tbCvc.Text = "123";
        }

        private async void btnOdemeYap_Click(object sender, EventArgs e)
        {
            string kartSahibi = tbKartSahibi.Text.Trim();
            string kartNo = tbKartNumarasi.Text.Trim();
            string ay = cbSKAy.SelectedItem?.ToString() ?? "12";
            string yil = cbSKYil.SelectedItem?.ToString() ?? "2028";
            string cvc = tbCvc.Text.Trim();

            if (string.IsNullOrEmpty(kartSahibi) || string.IsNullOrEmpty(kartNo) || string.IsNullOrEmpty(cvc))
            {
                MessageBox.Show("Lütfen tüm kart bilgilerini eksiksiz giriniz!", "Eksik Bilgi ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnOdemeYap.Enabled = false;
            btnOdemeYap.Text = "⏳ iyzico Sandbox Bağlanıyor...";

            try
            {
                var sonuc = await IyzipayHelper.OdemeYapAsync(
                    kartSahibi,
                    kartNo,
                    ay,
                    yil,
                    cvc,
                    seciliArac?.AracModeli ?? "Araç Kiralama",
                    seciliArac?.Fiyat ?? "100 TL"
                );

                if (sonuc.IsSuccess)
                {
                    // MySQL Odemeler Tablosuna Ekle
                    await OdemeKaydetVeritabani(1, sonuc.PaymentId);

                    MessageBox.Show($"💳 iyzico Sandbox Ödeme Başarılı!\n\nÖdeme No: {sonuc.PaymentId}\nAraç: {seciliArac?.AracModeli}\nTutar: {seciliArac?.Fiyat}", "İşlem Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ödeme Başarısız: " + sonuc.ErrorMessage, "Ödeme Hatası ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ödeme sırasında sistem hatası: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnOdemeYap.Enabled = true;
                btnOdemeYap.Text = "💳 Ödemeyi Onayla ve Kirala";
            }
        }

        private async Task OdemeKaydetVeritabani(int kullaniciId, string paymentId)
        {
            using (var db = DbHelper.GetConnection())
            {
                string createSql = @"
                    CREATE TABLE IF NOT EXISTS Odemeler (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        KullaniciId INT,
                        AracModeli VARCHAR(255),
                        OdenenTutar VARCHAR(100),
                        IslemTarihi DATETIME,
                        Durum VARCHAR(50)
                    );
                ";
                await db.ExecuteAsync(createSql);

                string insSql = @"
                    INSERT INTO Odemeler (KullaniciId, AracModeli, OdenenTutar, IslemTarihi, Durum)
                    VALUES (@KullaniciId, @AracModeli, @OdenenTutar, @IslemTarihi, @Durum);
                ";

                await db.ExecuteAsync(insSql, new
                {
                    KullaniciId = kullaniciId,
                    AracModeli = seciliArac?.AracModeli ?? "",
                    OdenenTutar = seciliArac?.Fiyat ?? "",
                    IslemTarihi = DateTime.Now,
                    Durum = "Başarılı (Sandbox)"
                });
            }
        }
    }
}

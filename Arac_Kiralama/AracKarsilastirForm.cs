using Arac_Kiralama.models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class AracKarsilastirForm : Form
    {
        private List<Arac> karsilastirilacakAraclar;

        public AracKarsilastirForm(List<Arac> araclar)
        {
            InitializeComponent();
            this.karsilastirilacakAraclar = araclar ?? new List<Arac>();
            ModernKoyuKarsilastirmaTasarimiOlustur();
        }

        private void ModernKoyuKarsilastirmaTasarimiOlustur()
        {
            this.Text = "⚖️ Araç Karşılaştırma ve Kıyaslama Paneli";
            this.Size = new Size(1050, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(24, 27, 32);
            this.ForeColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            var lblBaslik = new Label();
            lblBaslik.Text = "⚖️ Seçilen Araçların Detaylı Karşılaştırması";
            lblBaslik.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblBaslik.ForeColor = Color.White;
            lblBaslik.Location = new Point(25, 20);
            lblBaslik.AutoSize = true;
            this.Controls.Add(lblBaslik);

            if (karsilastirilacakAraclar.Count == 0)
            {
                var lblUyari = new Label();
                lblUyari.Text = "Karşılaştırılacak araç bulunamadı!";
                lblUyari.Location = new Point(25, 70);
                lblUyari.ForeColor = Color.Salmon;
                this.Controls.Add(lblUyari);
                return;
            }

            // En uygun fiyatlı aracı tespit et
            double minFiyat = double.MaxValue;
            Arac enEkonomik = null;
            foreach (var a in karsilastirilacakAraclar)
            {
                double val = FiyatSayisal(a.Fiyat);
                if (val > 0 && val < minFiyat)
                {
                    minFiyat = val;
                    enEkonomik = a;
                }
            }

            int kartGenislik = 300;
            int bosluk = 25;
            int startX = 25;

            for (int i = 0; i < Math.Min(3, karsilastirilacakAraclar.Count); i++)
            {
                var arac = karsilastirilacakAraclar[i];
                bool isEnEkonomik = (arac == enEkonomik);

                var pnlKart = new Panel();
                pnlKart.Location = new Point(startX + i * (kartGenislik + bosluk), 75);
                pnlKart.Size = new Size(kartGenislik, 500);
                pnlKart.BackColor = Color.FromArgb(38, 43, 55);

                var pnlUst = new Panel();
                pnlUst.Dock = DockStyle.Top;
                pnlUst.Height = 6;
                pnlUst.BackColor = isEnEkonomik ? Color.FromArgb(16, 185, 129) : Color.FromArgb(13, 110, 253);
                pnlKart.Controls.Add(pnlUst);

                int y = 20;

                if (isEnEkonomik)
                {
                    var lblRozet = new Label();
                    lblRozet.Text = "🏆 EN AVANTAJLI SEÇİM";
                    lblRozet.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                    lblRozet.ForeColor = Color.FromArgb(16, 185, 129);
                    lblRozet.Location = new Point(15, y);
                    lblRozet.AutoSize = true;
                    pnlKart.Controls.Add(lblRozet);
                    y += 25;
                }

                var lblModel = new Label();
                lblModel.Text = arac.AracModeli;
                lblModel.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
                lblModel.ForeColor = Color.White;
                lblModel.Location = new Point(15, y);
                lblModel.Size = new Size(270, 45);
                pnlKart.Controls.Add(lblModel);
                y += 50;

                y = SatirEkle(pnlKart, "🏢 Kiralama Şirketi:", arac.KiralamaSirketi, y);
                y = SatirEkle(pnlKart, "⚙️ Vites Türü:", arac.VitesTipi, y);
                y = SatirEkle(pnlKart, "⛽ Yakıt Tipi:", arac.YakitTipi, y);
                y = SatirEkle(pnlKart, "💵 Günlük Fiyat:", arac.Fiyat, y, Color.FromArgb(245, 158, 11));

                double sayisal = FiyatSayisal(arac.Fiyat);
                if (sayisal > 0)
                {
                    y = SatirEkle(pnlKart, "📊 3 Günlük Tahmini:", $"{sayisal * 3:N0} ₺", y, Color.FromArgb(99, 102, 241));
                }

                var btnKirala = new Button();
                btnKirala.Text = "💳 Bu Aracı Kirala";
                btnKirala.Size = new Size(270, 40);
                btnKirala.Location = new Point(15, 435);
                btnKirala.FlatStyle = FlatStyle.Flat;
                btnKirala.FlatAppearance.BorderSize = 0;
                btnKirala.BackColor = isEnEkonomik ? Color.FromArgb(16, 185, 129) : Color.FromArgb(13, 110, 253);
                btnKirala.ForeColor = Color.White;
                btnKirala.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                btnKirala.Cursor = Cursors.Hand;
                btnKirala.Click += (s, e) =>
                {
                    OdemeForm of = new OdemeForm(arac);
                    of.ShowDialog();
                };
                pnlKart.Controls.Add(btnKirala);

                this.Controls.Add(pnlKart);
            }
        }

        private int SatirEkle(Panel pnl, string baslik, string deger, int y, Color? degerRenk = null)
        {
            var lblBaslik = new Label();
            lblBaslik.Text = baslik;
            lblBaslik.Font = new Font("Segoe UI", 9f);
            lblBaslik.ForeColor = Color.FromArgb(148, 163, 184);
            lblBaslik.Location = new Point(15, y);
            lblBaslik.AutoSize = true;
            pnl.Controls.Add(lblBaslik);

            var lblDeger = new Label();
            lblDeger.Text = string.IsNullOrEmpty(deger) ? "-" : deger;
            lblDeger.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblDeger.ForeColor = degerRenk ?? Color.FromArgb(241, 245, 249);
            lblDeger.Location = new Point(15, y + 18);
            lblDeger.Size = new Size(270, 22);
            pnl.Controls.Add(lblDeger);

            return y + 48;
        }

        private static readonly Regex FiyatRegex = new Regex(@"[^\d,.]", RegexOptions.Compiled);

        private double FiyatSayisal(string fiyatMetin)
        {
            if (string.IsNullOrEmpty(fiyatMetin)) return 0;
            string temiz = FiyatRegex.Replace(fiyatMetin.Split('(')[0], "").Replace(".", "").Replace(",", ".");
            if (double.TryParse(temiz, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double res))
            {
                return res;
            }
            return 0;
        }
    }
}

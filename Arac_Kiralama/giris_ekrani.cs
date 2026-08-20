using Arac_Kiralama.models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class giris_ekrani : Form
    {
        private Panel pnlKart;
        private Label lblBaslik;
        private Label lblAltBaslik;

        public giris_ekrani()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            ModernKoyuGirisTemasiUygula();
            this.Resize += (s, e) => KartOrtala();
        }

        private void ModernKoyuGirisTemasiUygula()
        {
            // 1. Form Genel Ayarları
            this.BackColor = Color.FromArgb(24, 27, 32);
            this.ForeColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 10f, FontStyle.Regular);

            // 2. Ortalanmış Koyu Kart Paneli
            pnlKart = new Panel();
            pnlKart.Size = new Size(420, 390);
            pnlKart.BackColor = Color.FromArgb(38, 43, 55);
            this.Controls.Add(pnlKart);

            // 3. Başlıklar
            lblBaslik = new Label();
            lblBaslik.Text = "🚗 Yolcu360 Otomasyonu";
            lblBaslik.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblBaslik.ForeColor = Color.White;
            lblBaslik.AutoSize = false;
            lblBaslik.Size = new Size(360, 32);
            lblBaslik.Location = new Point(30, 25);
            lblBaslik.TextAlign = ContentAlignment.MiddleCenter;
            pnlKart.Controls.Add(lblBaslik);

            lblAltBaslik = new Label();
            lblAltBaslik.Text = "Lütfen hesap bilgilerinizle giriş yapın";
            lblAltBaslik.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblAltBaslik.ForeColor = Color.FromArgb(148, 163, 184);
            lblAltBaslik.AutoSize = false;
            lblAltBaslik.Size = new Size(360, 22);
            lblAltBaslik.Location = new Point(30, 60);
            lblAltBaslik.TextAlign = ContentAlignment.MiddleCenter;
            pnlKart.Controls.Add(lblAltBaslik);

            // 4. Etiketler (E-posta ve Şifre)
            label1.Text = "E-posta Adresi";
            label1.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(203, 213, 225);
            label1.Location = new Point(40, 105);
            label1.Parent = pnlKart;

            label2.Text = "Şifre";
            label2.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(203, 213, 225);
            label2.Location = new Point(40, 180);
            label2.Parent = pnlKart;

            // 5. Giriş Kutuları (TextBox)
            tbEposta.BackColor = Color.FromArgb(30, 34, 45);
            tbEposta.ForeColor = Color.White;
            tbEposta.Font = new Font("Segoe UI", 11f);
            tbEposta.BorderStyle = BorderStyle.FixedSingle;
            tbEposta.Size = new Size(340, 28);
            tbEposta.Location = new Point(40, 130);
            tbEposta.Parent = pnlKart;

            tbSifre.BackColor = Color.FromArgb(30, 34, 45);
            tbSifre.ForeColor = Color.White;
            tbSifre.Font = new Font("Segoe UI", 11f);
            tbSifre.BorderStyle = BorderStyle.FixedSingle;
            tbSifre.PasswordChar = '•';
            tbSifre.Size = new Size(340, 28);
            tbSifre.Location = new Point(40, 205);
            tbSifre.Parent = pnlKart;

            // 6. Giriş Butonu
            btngiris.FlatStyle = FlatStyle.Flat;
            btngiris.FlatAppearance.BorderSize = 0;
            btngiris.BackColor = Color.FromArgb(13, 110, 253);
            btngiris.ForeColor = Color.White;
            btngiris.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btngiris.Text = "Giriş Yap 🚀";
            btngiris.Size = new Size(340, 42);
            btngiris.Location = new Point(40, 275);
            btngiris.Cursor = Cursors.Hand;
            btngiris.Parent = pnlKart;

            KartOrtala();
        }

        private void KartOrtala()
        {
            if (pnlKart != null)
            {
                pnlKart.Left = (this.ClientSize.Width - pnlKart.Width) / 2;
                pnlKart.Top = (this.ClientSize.Height - pnlKart.Height) / 2;
            }
        }

        public kullanici GirisYapanKullanici { get; private set; }

        private void btngiris_Click(object sender, EventArgs e)
        {
            string eposta = tbEposta.Text.Trim();
            string sifre = tbSifre.Text.Trim();

            // Giriş alanlarının boş olup olmadığını kontrol ediyoruz 
            if (string.IsNullOrEmpty(eposta) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen e-posta ve şifrenizi giriniz!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcı adı ve şifreyi kontrol ediyoruz
            string sql = "SELECT * FROM kullanicilar WHERE eposta=@eposta AND sifre=@sifre";

            var kullanici = DbHelper.QueryFirstOrDefault<kullanici>(sql, new { Eposta = eposta, Sifre = sifre });

            if (kullanici != null)
            {
                // Giriş yapan kullanıcıyı saklayıp formu kapatıyoruz 🚪
                this.GirisYapanKullanici = kullanici;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("E-posta veya şifre hatalı! ❌", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

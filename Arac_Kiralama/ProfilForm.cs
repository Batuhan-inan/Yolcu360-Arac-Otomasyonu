using Arac_Kiralama.models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class ProfilForm : Form
    {
        private kullanici oturumKullanici;
        private TextBox tbEposta;
        private TextBox tbTelefon;
        private TextBox tbEskiSifre;
        private TextBox tbYeniSifre;
        private TextBox tbYeniSifreTekrar;
        private DataGridView dgvAdminKullanicilar;
        private TabControl tabControl;

        public ProfilForm(kullanici kullanici)
        {
            InitializeComponent();
            this.oturumKullanici = kullanici;
            ModernKoyuProfilTasarimiOlustur();
        }

        private async void ProfilForm_Load(object sender, EventArgs e)
        {
            if (oturumKullanici != null)
            {
                tbEposta.Text = oturumKullanici.Eposta;
                tbTelefon.Text = oturumKullanici.Telefon;

                if (IsAdmin(oturumKullanici))
                {
                    await TumKullanicilariYukle();
                }
            }
        }

        private bool IsAdmin(kullanici k)
        {
            if (k == null) return false;
            return string.Equals(k.Rol, "Admin", StringComparison.OrdinalIgnoreCase) ||
                   (k.Eposta != null && k.Eposta.ToLower().Contains("admin"));
        }

        private void ModernKoyuProfilTasarimiOlustur()
        {
            this.Text = IsAdmin(oturumKullanici) ? "👑 Yönetici & Kullanıcı Yönetim Paneli" : "👤 Kullanıcı Profilim";
            this.Size = new Size(850, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(24, 27, 32);
            this.ForeColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;

            // 1. SEKME: PROFİL BİLGİLERİ VE ŞİFRE DEĞİŞTİRME
            var tabProfil = new TabPage("👤 Profil Bilgileri & Şifre");
            tabProfil.BackColor = Color.FromArgb(24, 27, 32);

            // Bilgi Kartı
            var pnlBilgi = new Panel();
            pnlBilgi.Location = new Point(25, 20);
            pnlBilgi.Size = new Size(360, 440);
            pnlBilgi.BackColor = Color.FromArgb(38, 43, 55);

            var lblBilgiBaslik = new Label();
            lblBilgiBaslik.Text = "Hesap Detayları";
            lblBilgiBaslik.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblBilgiBaslik.ForeColor = Color.White;
            lblBilgiBaslik.Location = new Point(20, 20);
            lblBilgiBaslik.AutoSize = true;
            pnlBilgi.Controls.Add(lblBilgiBaslik);

            // Rol Rozeti
            var lblRol = new Label();
            lblRol.Text = IsAdmin(oturumKullanici) ? "👑 Yetki: Sistem Yöneticisi (Admin)" : "👤 Yetki: Standart Müşteri";
            lblRol.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            lblRol.ForeColor = IsAdmin(oturumKullanici) ? Color.FromArgb(245, 158, 11) : Color.FromArgb(14, 165, 233);
            lblRol.Location = new Point(20, 55);
            lblRol.AutoSize = true;
            pnlBilgi.Controls.Add(lblRol);

            var lblEposta = new Label { Text = "E-posta Adresi:", Location = new Point(20, 95), AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184) };
            pnlBilgi.Controls.Add(lblEposta);

            tbEposta = new TextBox { Location = new Point(20, 118), Size = new Size(315, 28), ReadOnly = true, BackColor = Color.FromArgb(30, 34, 45), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pnlBilgi.Controls.Add(tbEposta);

            var lblTelefon = new Label { Text = "Telefon Numarası:", Location = new Point(20, 160), AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184) };
            pnlBilgi.Controls.Add(lblTelefon);

            tbTelefon = new TextBox { Location = new Point(20, 183), Size = new Size(315, 28), BackColor = Color.FromArgb(30, 34, 45), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pnlBilgi.Controls.Add(tbTelefon);

            var btnTelGuncelle = new Button();
            btnTelGuncelle.Text = "💾 Telefonu Güncelle";
            btnTelGuncelle.Location = new Point(20, 225);
            btnTelGuncelle.Size = new Size(315, 36);
            btnTelGuncelle.FlatStyle = FlatStyle.Flat;
            btnTelGuncelle.FlatAppearance.BorderSize = 0;
            btnTelGuncelle.BackColor = Color.FromArgb(16, 185, 129);
            btnTelGuncelle.ForeColor = Color.White;
            btnTelGuncelle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            btnTelGuncelle.Cursor = Cursors.Hand;
            btnTelGuncelle.Click += BtnTelGuncelle_Click;
            pnlBilgi.Controls.Add(btnTelGuncelle);

            tabProfil.Controls.Add(pnlBilgi);

            // Şifre Değiştirme Kartı
            var pnlSifre = new Panel();
            pnlSifre.Location = new Point(410, 20);
            pnlSifre.Size = new Size(390, 440);
            pnlSifre.BackColor = Color.FromArgb(38, 43, 55);

            var lblSifreBaslik = new Label();
            lblSifreBaslik.Text = "🔒 Güvenlik & Şifre Değiştir";
            lblSifreBaslik.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblSifreBaslik.ForeColor = Color.White;
            lblSifreBaslik.Location = new Point(20, 20);
            lblSifreBaslik.AutoSize = true;
            pnlSifre.Controls.Add(lblSifreBaslik);

            var lblEskiSifre = new Label { Text = "Mevcut Şifreniz:", Location = new Point(20, 75), AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184) };
            pnlSifre.Controls.Add(lblEskiSifre);

            tbEskiSifre = new TextBox { Location = new Point(20, 98), Size = new Size(345, 28), PasswordChar = '•', BackColor = Color.FromArgb(30, 34, 45), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pnlSifre.Controls.Add(tbEskiSifre);

            var lblYeniSifre = new Label { Text = "Yeni Şifre:", Location = new Point(20, 140), AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184) };
            pnlSifre.Controls.Add(lblYeniSifre);

            tbYeniSifre = new TextBox { Location = new Point(20, 163), Size = new Size(345, 28), PasswordChar = '•', BackColor = Color.FromArgb(30, 34, 45), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pnlSifre.Controls.Add(tbYeniSifre);

            var lblYeniSifreTekrar = new Label { Text = "Yeni Şifre (Tekrar):", Location = new Point(20, 205), AutoSize = true, ForeColor = Color.FromArgb(148, 163, 184) };
            pnlSifre.Controls.Add(lblYeniSifreTekrar);

            tbYeniSifreTekrar = new TextBox { Location = new Point(20, 228), Size = new Size(345, 28), PasswordChar = '•', BackColor = Color.FromArgb(30, 34, 45), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pnlSifre.Controls.Add(tbYeniSifreTekrar);

            var btnSifreGuncelle = new Button();
            btnSifreGuncelle.Text = "🔑 Şifreyi Güncelle";
            btnSifreGuncelle.Location = new Point(20, 285);
            btnSifreGuncelle.Size = new Size(345, 40);
            btnSifreGuncelle.FlatStyle = FlatStyle.Flat;
            btnSifreGuncelle.FlatAppearance.BorderSize = 0;
            btnSifreGuncelle.BackColor = Color.FromArgb(13, 110, 253);
            btnSifreGuncelle.ForeColor = Color.White;
            btnSifreGuncelle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnSifreGuncelle.Cursor = Cursors.Hand;
            btnSifreGuncelle.Click += BtnSifreGuncelle_Click;
            pnlSifre.Controls.Add(btnSifreGuncelle);

            tabProfil.Controls.Add(pnlSifre);
            tabControl.TabPages.Add(tabProfil);

            // 2. SEKME: ADMİN KULLANICI YÖNETİMİ (Sadece Admin İse Görünür)
            if (IsAdmin(oturumKullanici))
            {
                var tabAdmin = new TabPage("👥 Kullanıcı Yönetimi (Admin)");
                tabAdmin.BackColor = Color.FromArgb(24, 27, 32);

                var lblAdminBaslik = new Label();
                lblAdminBaslik.Text = "Sistemde Kayıtlı Tüm Kullanıcılar:";
                lblAdminBaslik.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
                lblAdminBaslik.ForeColor = Color.White;
                lblAdminBaslik.Location = new Point(20, 15);
                lblAdminBaslik.AutoSize = true;
                tabAdmin.Controls.Add(lblAdminBaslik);

                dgvAdminKullanicilar = new DataGridView();
                dgvAdminKullanicilar.Location = new Point(20, 50);
                dgvAdminKullanicilar.Size = new Size(790, 360);
                dgvAdminKullanicilar.AllowUserToAddRows = false;
                dgvAdminKullanicilar.ReadOnly = true;
                dgvAdminKullanicilar.RowHeadersVisible = false;
                dgvAdminKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvAdminKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvAdminKullanicilar.BackgroundColor = Color.FromArgb(38, 43, 55);
                dgvAdminKullanicilar.BorderStyle = BorderStyle.None;
                dgvAdminKullanicilar.EnableHeadersVisualStyles = false;
                dgvAdminKullanicilar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
                dgvAdminKullanicilar.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184);
                dgvAdminKullanicilar.DefaultCellStyle.BackColor = Color.FromArgb(30, 34, 45);
                dgvAdminKullanicilar.DefaultCellStyle.ForeColor = Color.White;
                dgvAdminKullanicilar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 99, 235);
                tabAdmin.Controls.Add(dgvAdminKullanicilar);

                var btnKullaniciSil = new Button();
                btnKullaniciSil.Text = "🗑️ Seçilen Kullanıcıyı Sil";
                btnKullaniciSil.Location = new Point(20, 425);
                btnKullaniciSil.Size = new Size(220, 38);
                btnKullaniciSil.FlatStyle = FlatStyle.Flat;
                btnKullaniciSil.FlatAppearance.BorderSize = 0;
                btnKullaniciSil.BackColor = Color.FromArgb(220, 38, 38);
                btnKullaniciSil.ForeColor = Color.White;
                btnKullaniciSil.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                btnKullaniciSil.Cursor = Cursors.Hand;
                btnKullaniciSil.Click += BtnKullaniciSil_Click;
                tabAdmin.Controls.Add(btnKullaniciSil);

                tabControl.TabPages.Add(tabAdmin);
            }

            this.Controls.Add(tabControl);
        }

        private async void BtnTelGuncelle_Click(object sender, EventArgs e)
        {
            string tel = tbTelefon.Text.Trim();
            if (string.IsNullOrEmpty(tel))
            {
                MessageBox.Show("Lütfen geçerli bir telefon numarası giriniz!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "UPDATE kullanicilar SET telefon = @Telefon WHERE Id = @Id;";
                    await db.ExecuteAsync(sql, new { Telefon = tel, Id = oturumKullanici.Id });

                    oturumKullanici.Telefon = tel;
                    MessageBox.Show("Telefon numaranız başarıyla güncellendi! 💾", "Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSifreGuncelle_Click(object sender, EventArgs e)
        {
            string eski = tbEskiSifre.Text.Trim();
            string yeni = tbYeniSifre.Text.Trim();
            string tekrar = tbYeniSifreTekrar.Text.Trim();

            if (string.IsNullOrEmpty(eski) || string.IsNullOrEmpty(yeni))
            {
                MessageBox.Show("Lütfen tüm şifre alanlarını doldurunuz!", "Eksik Bilgi ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (eski != oturumKullanici.Sifre)
            {
                MessageBox.Show("Mevcut şifrenizi hatalı girdiniz! ❌", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (yeni != tekrar)
            {
                MessageBox.Show("Yeni şifreler birbiriyle uyuşmuyor! ❌", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (yeni.Length < 3)
            {
                MessageBox.Show("Yeni şifreniz en az 3 karakter olmalıdır!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "UPDATE kullanicilar SET sifre = @Sifre WHERE Id = @Id;";
                    await db.ExecuteAsync(sql, new { Sifre = yeni, Id = oturumKullanici.Id });

                    oturumKullanici.Sifre = yeni;
                    tbEskiSifre.Clear();
                    tbYeniSifre.Clear();
                    tbYeniSifreTekrar.Clear();

                    MessageBox.Show("Şifreniz başarıyla değiştirildi! 🔑🎉", "Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Şifre güncelleme hatası: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TumKullanicilariYukle()
        {
            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "SELECT Id, Eposta, Telefon, Sifre FROM kullanicilar ORDER BY Id ASC;";
                    var liste = (await db.QueryAsync<kullanici>(sql)).ToList();

                    dgvAdminKullanicilar.DataSource = null;
                    dgvAdminKullanicilar.DataSource = liste;

                    if (dgvAdminKullanicilar.Columns["Sifre"] != null) dgvAdminKullanicilar.Columns["Sifre"].HeaderText = "Şifre";
                    if (dgvAdminKullanicilar.Columns["Rol"] != null) dgvAdminKullanicilar.Columns["Rol"].Visible = false;
                }
            }
            catch { }
        }

        private async void BtnKullaniciSil_Click(object sender, EventArgs e)
        {
            if (dgvAdminKullanicilar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz bir kullanıcı seçin!", "Seçim Yapılmadı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var secili = dgvAdminKullanicilar.SelectedRows[0].DataBoundItem as kullanici;
            if (secili == null) return;

            if (secili.Id == oturumKullanici.Id)
            {
                MessageBox.Show("Kendi oturum açtığınız hesabı silemezsiniz!", "Engellendi 🛑", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var cevap = MessageBox.Show($"'{secili.Eposta}' kullanıcısını sistemden silmek istediğinize emin misiniz?", "Kullanıcı Sil ❓", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cevap == DialogResult.Yes)
            {
                try
                {
                    using (var db = DbHelper.GetConnection())
                    {
                        string sql = "DELETE FROM kullanicilar WHERE Id = @Id;";
                        await db.ExecuteAsync(sql, new { Id = secili.Id });

                        MessageBox.Show("Kullanıcı başarıyla silindi! 🗑️", "Bilgi ℹ️", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await TumKullanicilariYukle();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme hatası: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

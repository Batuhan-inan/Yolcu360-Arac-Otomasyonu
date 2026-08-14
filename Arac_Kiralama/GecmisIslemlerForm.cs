using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class GecmisIslemlerForm : Form
    {
        public GecmisIslemlerForm(int varsayilanSekme = 0)
        {
            InitializeComponent();

            if (varsayilanSekme >= 0 && varsayilanSekme < tabControlGecmis.TabPages.Count)
            {
                tabControlGecmis.SelectedIndex = varsayilanSekme;
            }
        }

        private async void GecmisIslemlerForm_Load(object sender, EventArgs e)
        {
            await KoleksiyonlariYukle();
            await OdemeleriYukle();
        }

        private async Task KoleksiyonlariYukle()
        {
            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "SELECT Id, KoleksiyonAdi, KayitTarihi FROM Koleksiyonlar ORDER BY KayitTarihi DESC;";
                    var koleksiyonlar = (await db.QueryAsync<Arac_Kiralama.models.Koleksiyon>(sql)).ToList();

                    dgvKoleksiyonlar.DataSource = null;
                    dgvKoleksiyonlar.DataSource = koleksiyonlar;

                    if (dgvKoleksiyonlar.Columns["Id"] != null) dgvKoleksiyonlar.Columns["Id"].Visible = false;
                    if (dgvKoleksiyonlar.Columns["KoleksiyonAdi"] != null) dgvKoleksiyonlar.Columns["KoleksiyonAdi"].HeaderText = "Koleksiyon Adı";
                    if (dgvKoleksiyonlar.Columns["KayitTarihi"] != null) dgvKoleksiyonlar.Columns["KayitTarihi"].HeaderText = "Kayıt Tarihi";

                    dgvKoleksiyonlar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koleksiyonlar yüklenirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void dgvKoleksiyonlar_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKoleksiyonlar.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvKoleksiyonlar.SelectedRows[0].DataBoundItem as Arac_Kiralama.models.Koleksiyon;
                if (seciliSatir != null)
                {
                    await KoleksiyonDetayYukle(seciliSatir.Id);
                }
            }
            else
            {
                dgvKoleksiyonDetay.DataSource = null;
            }
        }

        private async Task KoleksiyonDetayYukle(int koleksiyonId)
        {
            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "SELECT AracModeli, KiralamaSirketi, VitesTipi, YakitTipi, Fiyat FROM Araclar WHERE KoleksiyonId = @KoleksiyonId;";
                    var araclar = (await db.QueryAsync<Arac_Kiralama.models.Arac>(sql, new { KoleksiyonId = koleksiyonId })).ToList();

                    dgvKoleksiyonDetay.DataSource = null;
                    dgvKoleksiyonDetay.DataSource = araclar;

                    if (dgvKoleksiyonDetay.Columns["Id"] != null) dgvKoleksiyonDetay.Columns["Id"].Visible = false;
                    if (dgvKoleksiyonDetay.Columns["KoleksiyonId"] != null) dgvKoleksiyonDetay.Columns["KoleksiyonId"].Visible = false;
                    if (dgvKoleksiyonDetay.Columns["AracModeli"] != null) dgvKoleksiyonDetay.Columns["AracModeli"].HeaderText = "Araç Modeli";
                    if (dgvKoleksiyonDetay.Columns["KiralamaSirketi"] != null) dgvKoleksiyonDetay.Columns["KiralamaSirketi"].HeaderText = "Kiralama Şirketi";
                    if (dgvKoleksiyonDetay.Columns["VitesTipi"] != null) dgvKoleksiyonDetay.Columns["VitesTipi"].HeaderText = "Vites Tipi";
                    if (dgvKoleksiyonDetay.Columns["YakitTipi"] != null) dgvKoleksiyonDetay.Columns["YakitTipi"].HeaderText = "Yakıt Türü";
                    if (dgvKoleksiyonDetay.Columns["Fiyat"] != null) dgvKoleksiyonDetay.Columns["Fiyat"].HeaderText = "Fiyat";

                    dgvKoleksiyonDetay.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Araç detayları yüklenirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void tabControlGecmis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlGecmis.SelectedIndex == 1)
            {
                await OdemeleriYukle();
            }
        }

        private async void dgvKoleksiyonDetay_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvKoleksiyonDetay.Rows.Count > e.RowIndex)
            {
                var row = dgvKoleksiyonDetay.Rows[e.RowIndex];
                var arac = row.DataBoundItem as Arac_Kiralama.models.Arac;
                if (arac != null)
                {
                    OdemeForm odemeForm = new OdemeForm(arac);
                    if (odemeForm.ShowDialog() == DialogResult.OK)
                    {
                        await OdemeleriYukle();
                        // 💳 Ödeme bitince otomatik olarak "Geçmiş Ödemeler" sekmesine geç ki kullanıcı görsün!
                        tabControlGecmis.SelectedIndex = 1;
                    }
                }
            }
        }

        private async Task OdemeleriYukle()
        {
            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "SELECT Id, KullaniciId, AracModeli, OdenenTutar, IslemTarihi, Durum FROM Odemeler ORDER BY IslemTarihi DESC;";
                    var odemeler = (await db.QueryAsync<Arac_Kiralama.models.Odeme>(sql)).ToList();

                    dgvOdemeler.DataSource = null;
                    dgvOdemeler.DataSource = odemeler;

                    if (dgvOdemeler.Columns["Id"] != null) dgvOdemeler.Columns["Id"].Visible = false;
                    if (dgvOdemeler.Columns["KullaniciId"] != null) dgvOdemeler.Columns["KullaniciId"].HeaderText = "Kullanıcı ID";
                    if (dgvOdemeler.Columns["AracModeli"] != null) dgvOdemeler.Columns["AracModeli"].HeaderText = "Kiralanan Araç";
                    if (dgvOdemeler.Columns["OdenenTutar"] != null) dgvOdemeler.Columns["OdenenTutar"].HeaderText = "Ödenen Tutar";
                    if (dgvOdemeler.Columns["IslemTarihi"] != null) dgvOdemeler.Columns["IslemTarihi"].HeaderText = "İşlem Tarihi";
                    if (dgvOdemeler.Columns["Durum"] != null) dgvOdemeler.Columns["Durum"].HeaderText = "Durum";

                    dgvOdemeler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ödemeler yüklenirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnKoleksiyonSil_Click(object sender, EventArgs e)
        {
            if (dgvKoleksiyonlar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek istediğiniz bir koleksiyonu seçin!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var seciliKoleksiyon = dgvKoleksiyonlar.SelectedRows[0].DataBoundItem as Arac_Kiralama.models.Koleksiyon;
            if (seciliKoleksiyon == null) return;

            var cevap = MessageBox.Show($"'{seciliKoleksiyon.KoleksiyonAdi}' isimli koleksiyonu ve tüm araçlarını silmek istediğinize emin misiniz?", "Koleksiyonu Sil ❓", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (cevap == DialogResult.Yes)
            {
                try
                {
                    using (var db = DbHelper.GetConnection())
                    {
                        string delAracSql = "DELETE FROM Araclar WHERE KoleksiyonId = @Id;";
                        await db.ExecuteAsync(delAracSql, new { Id = seciliKoleksiyon.Id });

                        string delKolSql = "DELETE FROM Koleksiyonlar WHERE Id = @Id;";
                        await db.ExecuteAsync(delKolSql, new { Id = seciliKoleksiyon.Id });

                        MessageBox.Show("Koleksiyon başarıyla silindi! 🗑️", "Bilgi ℹ️", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await KoleksiyonlariYukle();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Koleksiyon silinirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPngKaydet_Click(object sender, EventArgs e)
        {
            if (dgvKoleksiyonlar.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen indirmek istediğiniz bir koleksiyonu seçin!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var seciliKoleksiyon = dgvKoleksiyonlar.SelectedRows[0].DataBoundItem as Arac_Kiralama.models.Koleksiyon;
            if (seciliKoleksiyon == null) return;

            if (dgvKoleksiyonDetay.Rows.Count == 0)
            {
                MessageBox.Show("Seçilen koleksiyonda indirilecek araç bulunamadı!", "Uyarı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            KoleksiyonuPngOlarakKaydet(seciliKoleksiyon, dgvKoleksiyonDetay);
        }

        private void KoleksiyonuPngOlarakKaydet(Arac_Kiralama.models.Koleksiyon koleksiyon, DataGridView dgvDetay)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Resmi (*.png)|*.png";
                sfd.FileName = $"{koleksiyon.KoleksiyonAdi}_Koleksiyonu.png";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        int genislik = 900;
                        int baslikYukseklik = 80;
                        int sutunBaslikYukseklik = 35;
                        int satirYukseklik = 30;
                        int altYukseklik = 40;

                        int toplamYukseklik = baslikYukseklik + sutunBaslikYukseklik + (dgvDetay.Rows.Count * satirYukseklik) + altYukseklik;

                        using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(genislik, toplamYukseklik))
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                        {
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                            g.Clear(System.Drawing.Color.White);

                            // 1. Üst Başlık Kartı
                            using (System.Drawing.Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(41, 128, 185)))
                            {
                                g.FillRectangle(bgBrush, 0, 0, genislik, baslikYukseklik);
                            }

                            using (System.Drawing.Font baslikFont = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold))
                            using (System.Drawing.Font tarihFont = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular))
                            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                            {
                                g.DrawString($"📌 Koleksiyon: {koleksiyon.KoleksiyonAdi}", baslikFont, textBrush, 20, 15);
                                g.DrawString($"Kayıt Tarihi: {koleksiyon.KayitTarihi:dd.MM.yyyy HH:mm} | Toplam Araç: {dgvDetay.Rows.Count}", tarihFont, textBrush, 22, 50);
                            }

                            // 2. Tablo Sütun Başlıkları
                            int currentY = baslikYukseklik;
                            using (System.Drawing.Brush headerBg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(52, 73, 94)))
                            using (System.Drawing.Font headerFont = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
                            using (System.Drawing.Brush headerText = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                            {
                                g.FillRectangle(headerBg, 0, currentY, genislik, sutunBaslikYukseklik);

                                g.DrawString("Araç Modeli", headerFont, headerText, 20, currentY + 7);
                                g.DrawString("Kiralama Şirketi", headerFont, headerText, 250, currentY + 7);
                                g.DrawString("Vites Tipi", headerFont, headerText, 470, currentY + 7);
                                g.DrawString("Yakıt Türü", headerFont, headerText, 620, currentY + 7);
                                g.DrawString("Fiyat", headerFont, headerText, 760, currentY + 7);
                            }

                            // 3. Tablo Satırları (Doğrudan Nesneden Güvenle Okunur 🎯)
                            currentY += sutunBaslikYukseklik;
                            using (System.Drawing.Font rowFont = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Regular))
                            using (System.Drawing.Font priceFont = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold))
                            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(44, 62, 80)))
                            using (System.Drawing.Brush altRowBg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(245, 247, 250)))
                            using (System.Drawing.Pen gridPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(230, 235, 240)))
                            {
                                int index = 0;
                                foreach (DataGridViewRow row in dgvDetay.Rows)
                                {
                                    if (row.IsNewRow) continue;

                                    if (index % 2 == 1)
                                    {
                                        g.FillRectangle(altRowBg, 0, currentY, genislik, satirYukseklik);
                                    }

                                    // 🟢 Sütun ismi aramadan doğrudan nesneden güvenle okuma:
                                    var arac = row.DataBoundItem as Arac_Kiralama.models.Arac;
                                    string model = arac != null ? arac.AracModeli : (row.Cells[0]?.Value?.ToString() ?? "");
                                    string sirket = arac != null ? arac.KiralamaSirketi : (row.Cells[1]?.Value?.ToString() ?? "");
                                    string vites = arac != null ? arac.VitesTipi : (row.Cells[2]?.Value?.ToString() ?? "");
                                    string yakit = arac != null ? arac.YakitTipi : (row.Cells[3]?.Value?.ToString() ?? "");
                                    string fiyat = arac != null ? arac.Fiyat : (row.Cells[4]?.Value?.ToString() ?? "");

                                    g.DrawString(model, rowFont, textBrush, 20, currentY + 5);
                                    g.DrawString(sirket, rowFont, textBrush, 250, currentY + 5);
                                    g.DrawString(vites, rowFont, textBrush, 470, currentY + 5);
                                    g.DrawString(yakit, rowFont, textBrush, 620, currentY + 5);
                                    g.DrawString(fiyat, priceFont, textBrush, 760, currentY + 5);

                                    g.DrawLine(gridPen, 0, currentY + satirYukseklik, genislik, currentY + satirYukseklik);
                                    currentY += satirYukseklik;
                                    index++;
                                }
                            }

                            // 4. Alt Bilgi
                            using (System.Drawing.Brush footerBg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(236, 240, 241)))
                            using (System.Drawing.Font footerFont = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic))
                            using (System.Drawing.Brush footerText = new System.Drawing.SolidBrush(System.Drawing.Color.Gray))
                            {
                                g.FillRectangle(footerBg, 0, currentY, genislik, altYukseklik);
                                g.DrawString("Yolcu360 Araç Kiralama Otomasyonu ile Oluşturuldu", footerFont, footerText, 20, currentY + 10);
                            }

                            bmp.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        }

                        MessageBox.Show($"'{koleksiyon.KoleksiyonAdi}' koleksiyonu başarıyla PNG olarak kaydedildi! 🖼️", "Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("PNG kaydedilirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

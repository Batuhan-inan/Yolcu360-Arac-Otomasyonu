using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Arac_Kiralama
{
    public partial class DashboardForm : Form
    {
        private Chart chartModeller;
        private Chart chartFiyatlar;
        private Label lblToplamKiralama;
        private Label lblToplamTutar;
        private Label lblEnPopulerArac;
        private Label lblOrtalamaTutar;
        private DataGridView dgvSonKiralamalar;

        public DashboardForm()
        {
            InitializeComponent();
            ModernKoyuDashboardTasarimiOlustur();
        }

        private async void DashboardForm_Load(object sender, EventArgs e)
        {
            await IstatistikleriYukle();
        }

        private void ModernKoyuDashboardTasarimiOlustur()
        {
            this.Text = "📊 Yönetici & İstatistik Dashboard Paneli";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(24, 27, 32);
            this.ForeColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 9.5f);

            // 1. ÜST KPI KARTLARI
            var pnlKpi1 = KpiKartOlustur("Toplam Kiralama", "0 Rezervasyon", Color.FromArgb(99, 102, 241), 20, 20, out lblToplamKiralama);
            var pnlKpi2 = KpiKartOlustur("Toplam Hacim", "0 ₺", Color.FromArgb(16, 185, 129), 285, 20, out lblToplamTutar);
            var pnlKpi3 = KpiKartOlustur("En Popüler Araç", "-", Color.FromArgb(13, 110, 253), 550, 20, out lblEnPopulerArac);
            var pnlKpi4 = KpiKartOlustur("Ortalama Kiralama", "0 ₺", Color.FromArgb(245, 158, 11), 815, 20, out lblOrtalamaTutar);

            this.Controls.Add(pnlKpi1);
            this.Controls.Add(pnlKpi2);
            this.Controls.Add(pnlKpi3);
            this.Controls.Add(pnlKpi4);

            // 2. PASTA GRAFİĞİ (En Çok Kiralanan Modeller)
            chartModeller = new Chart();
            chartModeller.Location = new Point(20, 150);
            chartModeller.Size = new Size(515, 300);
            chartModeller.BackColor = Color.FromArgb(38, 43, 55);

            var area1 = new ChartArea("Area1");
            area1.BackColor = Color.FromArgb(38, 43, 55);
            chartModeller.ChartAreas.Add(area1);

            var legend1 = new Legend("Legend1");
            legend1.BackColor = Color.FromArgb(38, 43, 55);
            legend1.ForeColor = Color.White;
            legend1.Font = new Font("Segoe UI", 9f);
            chartModeller.Legends.Add(legend1);

            var title1 = new Title("🚗 Popüler Araç Modelleri Dağılımı", Docking.Top, new Font("Segoe UI", 11f, FontStyle.Bold), Color.White);
            chartModeller.Titles.Add(title1);

            var series1 = new Series("Modeller");
            series1.ChartType = SeriesChartType.Doughnut;
            series1.Font = new Font("Segoe UI", 8.5f);
            chartModeller.Series.Add(series1);
            this.Controls.Add(chartModeller);

            // 3. SÜTUN GRAFİĞİ (Model / Kiralama Sayıları)
            chartFiyatlar = new Chart();
            chartFiyatlar.Location = new Point(555, 150);
            chartFiyatlar.Size = new Size(515, 300);
            chartFiyatlar.BackColor = Color.FromArgb(38, 43, 55);

            var area2 = new ChartArea("Area2");
            area2.BackColor = Color.FromArgb(38, 43, 55);
            area2.AxisX.LabelStyle.ForeColor = Color.White;
            area2.AxisX.LineColor = Color.FromArgb(71, 85, 105);
            area2.AxisX.MajorGrid.LineColor = Color.FromArgb(51, 65, 85);
            area2.AxisY.LabelStyle.ForeColor = Color.White;
            area2.AxisY.LineColor = Color.FromArgb(71, 85, 105);
            area2.AxisY.MajorGrid.LineColor = Color.FromArgb(51, 65, 85);
            chartFiyatlar.ChartAreas.Add(area2);

            var title2 = new Title("📊 Model Başına Kiralama Sayıları", Docking.Top, new Font("Segoe UI", 11f, FontStyle.Bold), Color.White);
            chartFiyatlar.Titles.Add(title2);

            var series2 = new Series("Adet");
            series2.ChartType = SeriesChartType.Column;
            series2.Color = Color.FromArgb(13, 110, 253);
            chartFiyatlar.Series.Add(series2);
            this.Controls.Add(chartFiyatlar);

            // 4. SON KİRALAMALAR MİNİ TABLOSU
            var lblTabloBaslik = new Label();
            lblTabloBaslik.Text = "📋 Son Kiralama Hareketleri";
            lblTabloBaslik.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblTabloBaslik.ForeColor = Color.White;
            lblTabloBaslik.Location = new Point(20, 465);
            lblTabloBaslik.Size = new Size(300, 25);
            this.Controls.Add(lblTabloBaslik);

            dgvSonKiralamalar = new DataGridView();
            dgvSonKiralamalar.Location = new Point(20, 495);
            dgvSonKiralamalar.Size = new Size(1050, 170);
            dgvSonKiralamalar.AllowUserToAddRows = false;
            dgvSonKiralamalar.ReadOnly = true;
            dgvSonKiralamalar.RowHeadersVisible = false;
            dgvSonKiralamalar.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSonKiralamalar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSonKiralamalar.BackgroundColor = Color.FromArgb(38, 43, 55);
            dgvSonKiralamalar.BorderStyle = BorderStyle.None;
            dgvSonKiralamalar.EnableHeadersVisualStyles = false;
            dgvSonKiralamalar.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42);
            dgvSonKiralamalar.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(148, 163, 184);
            dgvSonKiralamalar.DefaultCellStyle.BackColor = Color.FromArgb(30, 34, 45);
            dgvSonKiralamalar.DefaultCellStyle.ForeColor = Color.White;
            dgvSonKiralamalar.DefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 99, 235);
            this.Controls.Add(dgvSonKiralamalar);
        }

        private Panel KpiKartOlustur(string baslik, string varsayilanDeger, Color vurgurRengi, int x, int y, out Label lblDeger)
        {
            var pnl = new Panel();
            pnl.Location = new Point(x, y);
            pnl.Size = new Size(250, 110);
            pnl.BackColor = Color.FromArgb(38, 43, 55);

            var pnlCizgi = new Panel();
            pnlCizgi.Dock = DockStyle.Left;
            pnlCizgi.Width = 5;
            pnlCizgi.BackColor = vurgurRengi;
            pnl.Controls.Add(pnlCizgi);

            var lblBaslik = new Label();
            lblBaslik.Text = baslik;
            lblBaslik.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            lblBaslik.ForeColor = Color.FromArgb(148, 163, 184);
            lblBaslik.Location = new Point(15, 15);
            lblBaslik.AutoSize = true;
            pnl.Controls.Add(lblBaslik);

            lblDeger = new Label();
            lblDeger.Text = varsayilanDeger;
            lblDeger.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
            lblDeger.ForeColor = Color.White;
            lblDeger.Location = new Point(15, 45);
            lblDeger.AutoSize = true;
            pnl.Controls.Add(lblDeger);

            return pnl;
        }

        private async Task IstatistikleriYukle()
        {
            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    string sql = "SELECT * FROM Odemeler ORDER BY IslemTarihi DESC;";
                    var odemeler = (await db.QueryAsync<Arac_Kiralama.models.Odeme>(sql)).ToList();

                    if (odemeler.Count == 0) return;

                    // 1. KPI Hesaplamaları
                    lblToplamKiralama.Text = $"{odemeler.Count} Rezervasyon";

                    double toplamTutar = 0;
                    foreach (var odeme in odemeler)
                    {
                        string temiz = Regex.Replace(odeme.OdenenTutar ?? "", @"[^\d,.]", "").Replace(".", "").Replace(",", ".");
                        if (double.TryParse(temiz, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                        {
                            toplamTutar += val;
                        }
                    }

                    lblToplamTutar.Text = $"{toplamTutar:N0} ₺";
                    double ortalama = toplamTutar / odemeler.Count;
                    lblOrtalamaTutar.Text = $"{ortalama:N0} ₺";

                    var enCokKiralanan = odemeler.GroupBy(o => o.AracModeli)
                                                .OrderByDescending(g => g.Count())
                                                .FirstOrDefault();
                    if (enCokKiralanan != null)
                    {
                        lblEnPopulerArac.Text = $"{enCokKiralanan.Key} ({enCokKiralanan.Count()})";
                    }

                    // 2. Grafikleri Doldur
                    chartModeller.Series["Modeller"].Points.Clear();
                    chartFiyatlar.Series["Adet"].Points.Clear();

                    var grupModeller = odemeler.GroupBy(o => o.AracModeli)
                                              .Select(g => new { Model = g.Key, Adet = g.Count() })
                                              .OrderByDescending(x => x.Adet)
                                              .Take(5);

                    foreach (var g in grupModeller)
                    {
                        chartModeller.Series["Modeller"].Points.AddXY(g.Model, g.Adet);
                        chartFiyatlar.Series["Adet"].Points.AddXY(g.Model, g.Adet);
                    }

                    // 3. Tabloyu Doldur
                    dgvSonKiralamalar.DataSource = odemeler.Take(10).ToList();
                    if (dgvSonKiralamalar.Columns["Id"] != null) dgvSonKiralamalar.Columns["Id"].Visible = false;
                    if (dgvSonKiralamalar.Columns["KullaniciId"] != null) dgvSonKiralamalar.Columns["KullaniciId"].HeaderText = "Müşteri ID";
                    if (dgvSonKiralamalar.Columns["AracModeli"] != null) dgvSonKiralamalar.Columns["AracModeli"].HeaderText = "Araç Modeli";
                    if (dgvSonKiralamalar.Columns["OdenenTutar"] != null) dgvSonKiralamalar.Columns["OdenenTutar"].HeaderText = "Tutar";
                    if (dgvSonKiralamalar.Columns["IslemTarihi"] != null) dgvSonKiralamalar.Columns["IslemTarihi"].HeaderText = "Tarih";
                    if (dgvSonKiralamalar.Columns["Durum"] != null) dgvSonKiralamalar.Columns["Durum"].HeaderText = "Durum";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İstatistikler yüklenirken hata oluştu: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

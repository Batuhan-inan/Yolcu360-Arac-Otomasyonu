using Arac_Kiralama.models;
using CefSharp;
using CefSharp.WinForms;
using Dapper;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Arac_Kiralama.keyhandler;

namespace Arac_Kiralama
{
    public partial class Anaform : Form
    {

        // Rastgele sayı üretici
        private static readonly Random rnd = new Random();

        // 1. Veritabanından Kullanıcıyı Çekelim
        private kullanici aktifKullanici = null;

        private async Task<bool> ElemanaGercekTiklamaYap(string selector)//captcha için rastgele element seçici ve tıklama
        {
            if (chromiumWebBrowser != null && chromiumWebBrowser.IsBrowserInitialized)
            {
                // 1. Elemanın sınırlarını (left, top, width, height) alıyoruz (Bulunamazsa görünür butonu otomatik yakalar)
                string jsKonumGetir = $@"
                    (function() {{
                        let el = document.querySelector('{selector}');
                        
                        if (!el && '{selector}'.includes('button')) {{
                            el = Array.from(document.querySelectorAll('button')).find(b => b.offsetWidth > 0 && b.offsetHeight > 0) || document.querySelector('button');
                        }}

                        if (el) {{
                            let rect = el.getBoundingClientRect();
                            if (rect.width > 0 && rect.height > 0) {{
                                return {{
                                    left: rect.left,
                                    top: rect.top,
                                    width: rect.width,
                                    height: rect.height
                                }};
                            }}
                        }}
                        return null;
                    }})();
                ";

                var response = await chromiumWebBrowser.EvaluateScriptAsync(jsKonumGetir);

                if (response.Success && response.Result != null)
                {
                    dynamic rect = response.Result;
                    double left = Convert.ToDouble(rect.left);
                    double top = Convert.ToDouble(rect.top);
                    double width = Convert.ToDouble(rect.width);
                    double height = Convert.ToDouble(rect.height);

                    // 2. Elemanın sınırları içinde hedeflenecek nokta (%30 - %70 marjin içi)
                    int hedefX = (int)(left + rnd.Next((int)(width * 0.3), (int)(width * 0.7)));
                    int hedefY = (int)(top + rnd.Next((int)(height * 0.3), (int)(height * 0.7)));

                    int mevcutX = hedefX + rnd.Next(-120, 120);
                    int mevcutY = hedefY + rnd.Next(-120, 120);

                    // 3. reCAPTCHA için insansı kavisli fare hareketi
                    chromiumWebBrowser.GetBrowserHost().SendMouseMoveEvent(mevcutX, mevcutY, false, CefEventFlags.None);
                    await Task.Delay(rnd.Next(40, 100));

                    int adimSayisi = rnd.Next(8, 14);
                    for (int i = 1; i <= adimSayisi; i++)
                    {
                        double t = (double)i / adimSayisi;
                        int araX = (int)(mevcutX + (hedefX - mevcutX) * t);
                        int araY = (int)(mevcutY + (hedefY - mevcutY) * t);

                        if (i < adimSayisi)
                        {
                            araX += rnd.Next(-2, 3);
                            araY += rnd.Next(-2, 3);
                        }

                        chromiumWebBrowser.GetBrowserHost().SendMouseMoveEvent(araX, araY, false, CefEventFlags.None);
                        await Task.Delay(rnd.Next(10, 25));
                    }

                    await Task.Delay(rnd.Next(60, 150));

                    // 4. Tam hedef noktasında Sabit Tıklama (Mouse Down/Up aynı noktada -> Sürükleme/Maviye boyama YAPMAZ)
                    chromiumWebBrowser.GetBrowserHost().SendMouseMoveEvent(hedefX, hedefY, false, CefEventFlags.None);
                    chromiumWebBrowser.GetBrowserHost().SendMouseClickEvent(hedefX, hedefY, MouseButtonType.Left, false, 1, CefEventFlags.None);
                    await Task.Delay(rnd.Next(50, 90));
                    chromiumWebBrowser.GetBrowserHost().SendMouseClickEvent(hedefX, hedefY, MouseButtonType.Left, true, 1, CefEventFlags.None);

                    await Task.Delay(100);

                    // 5. Garanti Odaklanma & JS Click Yedeği
                    string jsFocus = $@"
                        (function() {{
                            let el = document.querySelector('{selector}');
                            if (!el && '{selector}'.includes('button')) {{
                                el = Array.from(document.querySelectorAll('button')).find(b => b.offsetWidth > 0 && b.offsetHeight > 0) || document.querySelector('button');
                            }}
                            if (el) {{
                                if (typeof el.focus === 'function') el.focus();
                                if (typeof el.click === 'function') el.click();
                            }}
                        }})();
                    ";
                    chromiumWebBrowser.ExecuteScriptAsync(jsFocus);

                    return true;
                }
            }
            return false;
        }
        private async Task TelefonuGirAndDevamEt(string telefon)//captcha için rastgele element seçici ve yazma
        {
            if (chromiumWebBrowser != null && chromiumWebBrowser.IsBrowserInitialized)
            {
                // 1. Telefon input kutusuna ID (#phn-input) ile tıklamayı deniyoruz
                bool tiklandi = await ElemanaGercekTiklamaYap("#phn-input");

                // 🚨 Eleman bulunamazsa MessageBox uyarısı verir ve yazma işlemine GEÇMEDEN durur (return)
                if (!tiklandi)
                {
                    MessageBox.Show(
                        "Telefon giriş kutusu ('#phn-input') sayfada bulunamadı! Numarayı yazma işlemi iptal edildi.",
                        "Eleman Bulunamadı ⚠️",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                await Task.Delay(rnd.Next(200, 350));

                // 2. Sayfa metnini bozmayan (Ctrl+A kullanmayan) güvenli odaklanma ve temizleme
                string temizleJs = @"
                    (function() {
                        let input = document.querySelector('#phn-input');
                        if (input) {
                            input.focus();
                            input.value = '';
                        }
                    })();
                ";
                chromiumWebBrowser.ExecuteScriptAsync(temizleJs);
                await Task.Delay(rnd.Next(100, 200));

                // 3. Telefon numarasını karakter karakter insansı hızda yazıyoruz (KeyDown + Char + KeyUp)
                for (int i = 0; i < telefon.Length; i++)
                {
                    char chh = telefon[i];
                    int vkCode = (int)chh;

                    // KeyDown
                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.KeyDown,
                        WindowsKeyCode = vkCode,
                        FocusOnEditableField = true
                    });
                    await Task.Delay(rnd.Next(15, 30));

                    // Char
                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.Char,
                        WindowsKeyCode = chh,
                        FocusOnEditableField = true
                    });
                    await Task.Delay(rnd.Next(15, 30));

                    // KeyUp
                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.KeyUp,
                        WindowsKeyCode = vkCode,
                        FocusOnEditableField = true
                    });

                    // Karakter arası değişken bekleme
                    await Task.Delay(rnd.Next(50, 130));

                    // Telefon numarasında gruplar arası insan hatırlama duraksaması (4. ve 7. rakam sonrası)
                    if (i == 3 || i == 6)
                    {
                        await Task.Delay(rnd.Next(200, 400));
                    }
                }

                await Task.Delay(rnd.Next(400, 800));

                // 4. ENTER Tuş Simülasyonu (Formu doğrudan göndermek için)
                chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                {
                    Type = CefSharp.KeyEventType.KeyDown,
                    WindowsKeyCode = (int)Keys.Enter,
                    FocusOnEditableField = true
                });
                await Task.Delay(50);
                chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                {
                    Type = CefSharp.KeyEventType.KeyUp,
                    WindowsKeyCode = (int)Keys.Enter,
                    FocusOnEditableField = true
                });

                await Task.Delay(rnd.Next(300, 500));

                // 5. SMS Dinleyiciyi Başlat
                SmsDinleyiciBaslat();

                // 6. Devam Et butonuna tıkla (Sayfadaki görünür ilk butonu otomatik yakalar)
                bool devamBtnTiklandi = await ElemanaGercekTiklamaYap("button");
                if (!devamBtnTiklandi)
                {
                    MessageBox.Show("Sayfada tıklanacak buton bulunamadı!", "Eleman Bulunamadı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private ChromiumWebBrowser chromiumWebBrowser;
        public Anaform()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

            // 📐 Panel sol sınırını 690px yaparak sola doğru büyütüyoruz ve sağ kenara sıfırlıyoruz:
            this.panel.Left = 690;
            this.panel.Width = this.ClientSize.Width - this.panel.Left - 15;
            this.panel.Height = this.ClientSize.Height - this.panel.Top - 15;
            this.panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void BaslatTarayici()
        {//  1. CefSharp daha önce başlatılmadıysa ayarları yap ve başlat
            if (Cef.IsInitialized == false)
            {
                //  Oturum verilerinin kaydedileceği klasör yolu
                string profilKlasoru = System.IO.Path.Combine(Application.StartupPath, "TarayiciProfili");

                //  Eskisi gibi silmiyoruz! Klasör yoksa yeni oluşturuyoruz:
                if (!System.IO.Directory.Exists(profilKlasoru))
                {
                    System.IO.Directory.CreateDirectory(profilKlasoru);
                }

                var settings = new CefSettings();

                // 💾 Oturumu ve çerezleri bu klasöre kaydediyoruz
                settings.CachePath = profilKlasoru;
                settings.PersistSessionCookies = true;

                // CefSharp Ayarlarını Başlat 
                
                settings.LogSeverity = LogSeverity.Disable;
                settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";
                settings.CefCommandLineArgs.Add("disable-blink-features", "AutomationControlled");
                settings.CefCommandLineArgs.Add("enable-media-stream", "1");


                settings.CefCommandLineArgs.Add("enable-webgl", "1");
                settings.CefCommandLineArgs.Add("ignore-gpu-blocklist", "1");

                settings.CefCommandLineArgs.Add("sec-ch-ua", "\"Chromium\";v=\"122\", \"Not(A:Brand\";v=\"24\", \"Google Chrome\";v=\"122\"");
                settings.CefCommandLineArgs.Add("sec-ch-ua-mobile", "?0");
                settings.CefCommandLineArgs.Add("sec-ch-ua-platform", "\"Windows\"");
            }

            // 🌐 2. Tarayıcıyı oluştur ve Login sayfasına git
            chromiumWebBrowser = new ChromiumWebBrowser("https://yolcu360.com/tr/login/");
            chromiumWebBrowser.Dock = DockStyle.Fill;
            panel.Controls.Add(chromiumWebBrowser);

            // Otomasyon izlerini gizlemek için script 🥷
            chromiumWebBrowser.FrameLoadStart += (sender, args) =>
            {
                // Sadece ana sayfa (Main Frame) yüklenirken çalıştırıyoruz
                if (args.Frame.IsMain)
                {
                    string hideWebdriverScript = @"
            Object.defineProperty(navigator, 'webdriver', { get: () => undefined });
            Object.defineProperty(navigator, 'languages', { get: () => ['tr-TR', 'tr', 'en-US', 'en'] });
            Object.defineProperty(navigator, 'plugins', { get: () => [1, 2, 3, 4, 5] }); ";

                    // Sayfa kaynakları yüklenmeye başlar başlamaz JS'i enjekte ediyoruz
                    args.Frame.ExecuteJavaScriptAsync(hideWebdriverScript);
                }
            };

            chromiumWebBrowser.KeyboardHandler = new CustomKeyboardHandler();
            chromiumWebBrowser.FrameLoadEnd += chromiumWebBrowser_FrameLoadEnd;

        }
        private bool telefonGirildi = false;
        private bool aramaYapildi = false;
        private bool filtreUygulandi = false;

        private string jsonOturumYolu = System.IO.Path.Combine(Application.StartupPath, "OturumVerileri.json");

        private async Task OturumKaydetJsonAsync()
        {
            try
            {
                if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    string jsGetStorage = @"
                        (function() {
                            let localData = {};
                            for (let i = 0; i < localStorage.length; i++) {
                                let key = localStorage.key(i);
                                localData[key] = localStorage.getItem(key);
                            }
                            let sessionData = {};
                            for (let i = 0; i < sessionStorage.length; i++) {
                                let key = sessionStorage.key(i);
                                sessionData[key] = sessionStorage.getItem(key);
                            }
                            return JSON.stringify({ localStorage: localData, sessionStorage: sessionData });
                        })();
                    ";

                    var response = await chromiumWebBrowser.EvaluateScriptAsync(jsGetStorage);
                    if (response.Success && response.Result != null)
                    {
                        string jsonVerisi = response.Result.ToString();
                        System.IO.File.WriteAllText(jsonOturumYolu, jsonVerisi);
                    }
                }
            }
            catch { }
        }

        private async Task OturumYukleJsonAsync()
        {
            try
            {
                if (System.IO.File.Exists(jsonOturumYolu) && chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    string jsonVerisi = System.IO.File.ReadAllText(jsonOturumYolu);
                    if (!string.IsNullOrEmpty(jsonVerisi))
                    {
                        string jsRestore = $@"
                            (function() {{
                                try {{
                                    let data = {jsonVerisi};
                                    if (data.localStorage) {{
                                        for (let key in data.localStorage) {{
                                            localStorage.setItem(key, data.localStorage[key]);
                                        }}
                                    }}
                                    if (data.sessionStorage) {{
                                        for (let key in data.sessionStorage) {{
                                            sessionStorage.setItem(key, data.sessionStorage[key]);
                                        }}
                                    }}
                                }} catch(e) {{}}
                            }})();
                        ";
                        await chromiumWebBrowser.EvaluateScriptAsync(jsRestore);
                    }
                }
            }
            catch { }
        }

        private async void chromiumWebBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                await OturumYukleJsonAsync();
                // 🖥️ Masaüstü Görünümünü Kesin Olarak Zorla (CSS & Viewport Enjeksiyonu)
                string masaustuZorlaJs = @"
                    (function() {
                        let meta = document.querySelector('meta[name=""viewport""]');
                        if (!meta) {
                            meta = document.createElement('meta');
                            meta.name = 'viewport';
                            if (document.head) document.head.appendChild(meta);
                        }
                        meta.setAttribute('content', 'width=1366, initial-scale=0.5');

                        let style = document.getElementById('force-desktop-style');
                        if (!style) {
                            style = document.createElement('style');
                            style.id = 'force-desktop-style';
                            if (document.head) document.head.appendChild(style);
                        }
                        style.innerHTML = `
                            html, body {
                                min-width: 1366px !important;
                                zoom: 0.75 !important;
                            }
                        `;
                    })();
                ";
                e.Frame.ExecuteJavaScriptAsync(masaustuZorlaJs);

                // 🔵 1. DURUM: ARAMA SONUÇLARI SAYFASINDAYSAK (URL '/search' içeriyorsa): 🚗
                if (e.Url.Contains("/search"))
                {
                    if (!filtreUygulandi)
                    {
                        filtreUygulandi = true; // Tekrar tekrar çalışmasını engelle

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(async () =>
                            {
                                string vitesSecimi = cbVites.SelectedItem?.ToString() ?? cbVites.Text;
                                string yakitSecimi = cbYakit.SelectedItem?.ToString() ?? cbYakit.Text;
                                await FiltreleriUygula(vitesSecimi, yakitSecimi);
                                await AraclariVeriTablosunaAktar();
                            }));
                        }
                        else
                        {
                            string vitesSecimi = cbVites.SelectedItem?.ToString() ?? cbVites.Text;
                            string yakitSecimi = cbYakit.SelectedItem?.ToString() ?? cbYakit.Text;
                            await FiltreleriUygula(vitesSecimi, yakitSecimi);
                            await AraclariVeriTablosunaAktar();
                        }
                    }
                }
                // 🔵 2. DURUM: GİRİŞ / ANA ARAMA SAYFASINDAYSAK:
                else
                {
                    await Task.Delay(1000);

                    // 1. Ekranda Telefon Kutusunun (#phn-input) açık olup olmadığını kontrol et
                    string phnKontrolJs = "document.querySelector('#phn-input') !== null;";
                    var response = await chromiumWebBrowser.EvaluateScriptAsync(phnKontrolJs);
                    bool phnKutusuAcikMi = response.Success && response.Result != null && Convert.ToBoolean(response.Result);

                    // 🟢 Ekranda Giriş Penceresi (Telefon Kutusu) Varsa:
                    if (phnKutusuAcikMi)
                    {
                        if (!telefonGirildi)
                        {
                            telefonGirildi = true; // Çift çalışmayı engelle

                            using (var db = DbHelper.GetConnection())
                            {
                                aktifKullanici = db.QueryFirstOrDefault<kullanici>("SELECT * FROM Kullanicilar LIMIT 1");
                            }

                            if (aktifKullanici != null)
                            {
                                await TelefonuGirAndDevamEt(aktifKullanici.Telefon);
                            }
                        }
                    }
                    // 🔵 Giriş Penceresi Kapandıysa / Yoksa (Ana Arama Ekranındaysak):
                    else
                    {
                        if (!aramaYapildi)
                        {
                            aramaYapildi = true; // Çift çalışmayı engelle
                            await Task.Delay(500);
                            aracgetir();
                        }
                    }
                }
            }
        }
        // Program kapanırken CefSharp'ı düzgünce kapatmak için
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Form kapanırken dinleyiciyi durdur
            if (tcpListener != null)
            {
                try { tcpListener.Stop(); } catch { }
            }

            Cef.Shutdown();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dtpVerisTarihi.MinDate = dtpAlisTarihi.Value;
        }

        private void dtpVerisTarihi_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Anaform_Load(object sender, EventArgs e)
        {
            // 📐 Form Tam Ekran Yapıldığında Panel ve DataGridView Otomatik Esnesin:
            this.panel.Left = 690;
            this.panel.Width = this.ClientSize.Width - this.panel.Left - 15;
            this.panel.Height = this.ClientSize.Height - this.panel.Top - 15;
            this.panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgbVeriEkranı.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

            dtpAlisTarihi.MinDate = DateTime.Now;
            dtpVerisTarihi.MinDate = DateTime.Now;

            cbVites.Items.AddRange(new string[] { "Tümü", "Otomatik", "Manuel" });
            cbVites.SelectedIndex = 0;
            cbYakit.Items.AddRange(new string[] { "Tümü", "Benzin", "Dizel", "LPG", "Elektrik", "Hibrit" });
            cbYakit.SelectedIndex = 0;
            string[] saatler = { "00:00", "00:30", "01:00", "01:30", "02:00", "02:30", "03:00", "03:30", "04:00", "04:30", "05:00", "05:30", "06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "23:30" };
            cbAlısSaati.Items.AddRange(saatler);
            cbAlısSaati.SelectedIndex = 0;
            cbDonusSaati.Items.AddRange(saatler);
            cbDonusSaati.SelectedIndex = 0;

            KoleksiyonButonDurumuGuncelle();
        }

        private void KoleksiyonButonDurumuGuncelle()
        {
            bool veriVarMi = dgbVeriEkranı != null && dgbVeriEkranı.Rows.Count > 0;
            if (btnKoleksiyonKaydet != null) btnKoleksiyonKaydet.Visible = veriVarMi;
            if (tbKoleksiyonAdi != null) tbKoleksiyonAdi.Visible = veriVarMi;
        }

        private async void btnKoleksiyonKaydet_Click(object sender, EventArgs e)
        {
            // 1. Koleksiyon Adı Boş Kontrolü
            string koleksiyonAdi = tbKoleksiyonAdi.Text.Trim();
            if (string.IsNullOrEmpty(koleksiyonAdi))
            {
                MessageBox.Show("Lütfen koleksiyon için bir isim giriniz!", "Eksik Bilgi ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbKoleksiyonAdi.Focus();
                return;
            }

            try
            {
                using (var db = DbHelper.GetConnection())
                {
                    db.Open();

                    // 2. Ana Koleksiyon Kaydı (KoleksiyonId Alınır)
                    string insKoleksiyon = "INSERT INTO Koleksiyonlar (KoleksiyonAdi, KayitTarihi) VALUES (@KoleksiyonAdi, @KayitTarihi); SELECT LAST_INSERT_ID();";
                    int yeniKoleksiyonId = await db.ExecuteScalarAsync<int>(insKoleksiyon, new { KoleksiyonAdi = koleksiyonAdi, KayitTarihi = DateTime.Now });

                    // 3. DataGridView'deki Araçları İlişkili Olarak Ekle
                    string insArac = @"INSERT INTO Araclar (KoleksiyonId, AracModeli, KiralamaSirketi, VitesTipi, YakitTipi, Fiyat) 
                                       VALUES (@KoleksiyonId, @AracModeli, @KiralamaSirketi, @VitesTipi, @YakitTipi, @Fiyat);";

                    List<Arac_Kiralama.models.Arac> aracListesi = new List<Arac_Kiralama.models.Arac>();
                    foreach (DataGridViewRow row in dgbVeriEkranı.Rows)
                    {
                        if (row.IsNewRow) continue;

                        aracListesi.Add(new Arac_Kiralama.models.Arac
                        {
                            KoleksiyonId = yeniKoleksiyonId,
                            AracModeli = row.Cells["Model"].Value?.ToString() ?? "",
                            KiralamaSirketi = row.Cells["Sirket"].Value?.ToString() ?? "",
                            VitesTipi = row.Cells["Vites"].Value?.ToString() ?? "",
                            YakitTipi = row.Cells["Yakit"].Value?.ToString() ?? "",
                            Fiyat = row.Cells["Fiyat"].Value?.ToString() ?? ""
                        });
                    }

                    await db.ExecuteAsync(insArac, aracListesi);

                    MessageBox.Show($"'{koleksiyonAdi}' isimli koleksiyon ve {aracListesi.Count} adet araç veritabanına başarıyla kaydedildi! 🎉", "Başarılı ✅", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbKoleksiyonAdi.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı kayıt hatası: " + ex.Message, "Hata ❌", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGecmisKoleksiyonlar_Click(object sender, EventArgs e)
        {
            GecmisIslemlerForm form = new GecmisIslemlerForm(0); // 0 = Geçmiş Koleksiyonlar Sekmesi
            form.ShowDialog();
        }

        private void btnGecmisOdemeler_Click(object sender, EventArgs e)
        {
            GecmisIslemlerForm form = new GecmisIslemlerForm(1); // 1 = Geçmiş Ödemeler Sekmesi
            form.ShowDialog();
        }

        private bool PortMusaitMi(int port)
        {
            System.Net.NetworkInformation.IPGlobalProperties ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.IPEndPoint[] tcpEndPoints = ipGlobalProperties.GetActiveTcpListeners();
            foreach (var endPoint in tcpEndPoints) //dolu portlar(tcpendpoints)'ı teker teker bak senin girdiğin(port) değişkenine denk olan var mı? kontrol et
            {
                if (endPoint.Port == port)
                {
                    return false; // Port meşgul
                }
            }
            return true; // Port müsait
        }
        private System.Net.Sockets.TcpListener tcpListener;
        private async void SmsDinleyiciBaslat()
        {
            try
            {
                int port = 8080;
                if (!PortMusaitMi(port))
                {
                    return;
                }

                if (tcpListener != null)
                {
                    try { tcpListener.Stop(); } catch { }
                }

                // 🌐 IPAddress.Any ile hem localhost hem Wi-Fi IP (192.168.1.161) doğrudan dinlenir (İzin gerekmez)
                tcpListener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, port);
                tcpListener.Start();

                await Task.Run(async () =>
                {
                    while (tcpListener != null)
                    {
                        try
                        {
                            using (var client = await tcpListener.AcceptTcpClientAsync())
                            using (var stream = client.GetStream())
                            using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                            using (var writer = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8) { AutoFlush = true })
                            {
                                string istekSatiri = await reader.ReadLineAsync();
                                if (!string.IsNullOrEmpty(istekSatiri))
                                {
                                    string cozulmusMetin = System.Net.WebUtility.UrlDecode(istekSatiri);
                                    Match eslesme = Regex.Match(cozulmusMetin, @"\d{6}");
                                    if (eslesme.Success)
                                    {
                                        string netSmsKodu = eslesme.Value;

                                        this.Invoke(new Action(() =>
                                        {
                                            SmsKodunuKutuyaYazAndDogrula(netSmsKodu);
                                        }));
                                    }
                                }

                                // MacroDroid'e standart HTTP 200 yanıtı dönüyoruz
                                string httpYanit = "HTTP/1.1 200 OK\r\nContent-Type: text/plain; charset=utf-8\r\nConnection: close\r\n\r\nOK";
                                await writer.WriteAsync(httpYanit);
                            }
                        }
                        catch { }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("SMS TCP Dinleyici Başlatılamadı: " + ex.Message);
            }
        }

        private async void SmsKodunuKutuyaYazAndDogrula(string netSmsKodu)
        {
            if (chromiumWebBrowser != null && chromiumWebBrowser.IsBrowserInitialized)
            {
                // 1. SMS input kutusuna (#sms_input) insansı fare hareketiyle tıkla
                bool tiklendi = await ElemanaGercekTiklamaYap("#sms_input");
                if (!tiklendi)
                {
                    MessageBox.Show("SMS giriş kutusu ('#sms_input') bulunamadı!", "Eleman Bulunamadı ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await Task.Delay(rnd.Next(150, 300));

                // 2. JavaScript ile değeri anında kutucuğa ve veri maskesine yerleştir
                string jsSmsYaz = $@"
                    (function() {{
                        let el = document.querySelector('#sms_input');
                        if (el) {{
                            el.focus();
                            el.value = '{netSmsKodu}';
                            el.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            el.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        }}
                    }})();
                ";
                chromiumWebBrowser.ExecuteScriptAsync(jsSmsYaz);
                await Task.Delay(rnd.Next(100, 200));

                // 3. Her bir rakamı insansı tuş simülasyonu ile gönder
                foreach (char rakam in netSmsKodu)
                {
                    int vkCode = (int)rakam;

                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.KeyDown,
                        WindowsKeyCode = vkCode,
                        FocusOnEditableField = true
                    });
                    await Task.Delay(rnd.Next(15, 30));

                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.Char,
                        WindowsKeyCode = rakam,
                        FocusOnEditableField = true
                    });
                    await Task.Delay(rnd.Next(15, 30));

                    chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                    {
                        Type = CefSharp.KeyEventType.KeyUp,
                        WindowsKeyCode = vkCode,
                        FocusOnEditableField = true
                    });

                    await Task.Delay(rnd.Next(60, 150));
                }

                await Task.Delay(rnd.Next(300, 500));

                // 4. ENTER Tuş Simülasyonu (Doğrudan Gönderim)
                chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                {
                    Type = CefSharp.KeyEventType.KeyDown,
                    WindowsKeyCode = (int)Keys.Enter,
                    FocusOnEditableField = true
                });
                await Task.Delay(50);
                chromiumWebBrowser.GetBrowserHost().SendKeyEvent(new CefSharp.KeyEvent
                {
                    Type = CefSharp.KeyEventType.KeyUp,
                    WindowsKeyCode = (int)Keys.Enter,
                    FocusOnEditableField = true
                });

                await Task.Delay(rnd.Next(300, 600));

                // 5. 'button[data-cms-key="button_apply"]' ("Doğrula") Butonuna Tıkla
                string jsDogrulaTikla = @"
                    (function() {
                        let btn = document.querySelector('button[data-cms-key=""button_apply""]') || 
                                  Array.from(document.querySelectorAll('button')).find(b => b.innerText && b.innerText.includes('Doğrula'));
                        if (btn) {
                            btn.click();
                        }
                    })();
                ";
                chromiumWebBrowser.ExecuteScriptAsync(jsDogrulaTikla);

                await ElemanaGercekTiklamaYap("button[data-cms-key='button_apply']");
            }
        }

        private void btnAra_Click(object sender, EventArgs e)
        {
            this.panel.BringToFront();
            aramaYapildi = false;

            if (dgbVeriEkranı != null)
            {
                dgbVeriEkranı.Rows.Clear();
            }
            KoleksiyonButonDurumuGuncelle();

            if (chromiumWebBrowser == null)
            {
                BaslatTarayici();
            }
            else
            {
                // Sayfayı Yolcu360 ana sayfasına yönlendir ki arama kutuları tekrar aktifleşsin
                chromiumWebBrowser.Load("https://yolcu360.com/tr/");
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private async Task<bool> alisyeri(string nereden)
        {
            if (string.IsNullOrEmpty(nereden)) return false;

            if (chromiumWebBrowser != null && chromiumWebBrowser.IsBrowserInitialized)
            {
                // 1. Alış Yeri kutusuna (#inputPickUpLocation) tıkla
                bool kutuBulundu = await ElemanaGercekTiklamaYap("#inputPickUpLocation");
                if (!kutuBulundu) return false;

                await Task.Delay(200);

                // 2. KUTUYU TEMİZLE -> HARF HARF KLAVYE OLAYLARI İLE YAZ (React Arama Tetikleyici) ⌨️
                string klavyeYazJs = $@"
                    (async function() {{
                        let input = document.querySelector('#inputPickUpLocation');
                        if (!input) return false;

                        input.focus();
                        input.click();

                        // 1. Temizle
                        let tracker = input._valueTracker;
                        if (tracker) tracker.setValue('');
                        input.value = '';
                        input.dispatchEvent(new Event('input', {{ bubbles: true }}));

                        // 2. Harf harf klavye basma olayları ile yaz (React search API'yi tetikler)
                        let metin = '{nereden.Replace("'", "\\'")}';
                        let val = '';

                        for (let char of metin) {{
                            val += char;
                            let trackerLoop = input._valueTracker;
                            if (trackerLoop) trackerLoop.setValue(val);
                            input.value = val;
                            
                            let keyEvt = {{ key: char, char: char, keyCode: char.charCodeAt(0), bubbles: true, cancelable: true }};
                            input.dispatchEvent(new KeyboardEvent('keydown', keyEvt));
                            input.dispatchEvent(new KeyboardEvent('keypress', keyEvt));
                            input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                            input.dispatchEvent(new KeyboardEvent('keyup', keyEvt));
                            await new Promise(r => setTimeout(r, 60));
                        }}

                        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        return true;
                    }})();
                ";

                await chromiumWebBrowser.EvaluateScriptAsync(klavyeYazJs);

                // Öneri listesinin ekranda yenilenmesi için bekle
                await Task.Delay(800);

                // 3. YENİ GELEN ŞEHİR SONUÇLARINI SEÇ 🎯
                string akilliSecimJs = $@"
                    (async function() {{
                        function bekle(ms) {{ return new Promise(r => setTimeout(r, ms)); }}

                        function gercekTikla(el) {{
                            if (!el) return;
                            el.focus();
                            ['pointerdown', 'mousedown', 'pointerup', 'mouseup', 'click'].forEach(evtType => {{
                                el.dispatchEvent(new MouseEvent(evtType, {{ bubbles: true, cancelable: true, view: window }}));
                            }});
                            try {{ el.click(); }} catch(e) {{}}
                        }}

                        let hedefMetin = '{nereden.Replace("'", "\\'")}'.trim().toLocaleLowerCase('tr-TR');

                        // Yeni şehir sonuçları ekrana düşene kadar bekle
                        let retries = 20;
                        let items = [];
                        while (retries-- > 0) {{
                            items = Array.from(document.querySelectorAll('.location-item'));
                            if (items.length === 0) {{
                                items = Array.from(document.querySelectorAll('.search-autocomplete > div'));
                            }}

                            let buldu = items.some(el => el.innerText.trim().toLocaleLowerCase('tr-TR').includes(hedefMetin));
                            if (buldu) break;

                            await bekle(150);
                        }}

                        if (items.length === 0) return false;

                        // 1. Adım: Tam Eşleşme (Sadece Ana Başlık)
                        let tamEslesen = items.find(el => {{
                            let anaBaslik = (el.querySelector('.font-bold')?.innerText || el.innerText.split('\n')[0]).trim().toLocaleLowerCase('tr-TR');
                            return anaBaslik === hedefMetin;
                        }});

                        if (tamEslesen) {{
                            gercekTikla(tamEslesen);
                            return true;
                        }}

                        // 2. Adım: Başlayan veya İçinde Geçen Eşleşme
                        let icerenEslesen = items.find(el => {{
                            let anaBaslik = (el.querySelector('.font-bold')?.innerText || el.innerText.split('\n')[0]).trim().toLocaleLowerCase('tr-TR');
                            return anaBaslik.startsWith(hedefMetin) || anaBaslik.includes(hedefMetin);
                        }});

                        if (icerenEslesen) {{
                            gercekTikla(icerenEslesen);
                            return true;
                        }}

                        // 3. Adım: Yüklenen İlk Seçenek
                        gercekTikla(items[0]);
                        return true;
                    }})();
                ";

                var res = await chromiumWebBrowser.EvaluateScriptAsync(akilliSecimJs);
                bool tiklandi = res.Success && res.Result != null && Convert.ToBoolean(res.Result);

                return true;
            }
            return false;
        }

        private async Task TarihAraligiSec(DateTime alisTarihi, DateTime donusTarihi)
        {
            string alisAy = alisTarihi.ToString("MMMM", new System.Globalization.CultureInfo("tr-TR")).ToLower();
            string donusAy = donusTarihi.ToString("MMMM", new System.Globalization.CultureInfo("tr-TR")).ToLower();
            string baslangicGunu = alisTarihi.Day.ToString();
            string bitisGunu = donusTarihi.Day.ToString();

            string jsScript = $@"
        (async function() {{
            const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

            function safeClick(el) {{
                if (!el) return;
                el.focus?.();
                ['pointerdown', 'mousedown', 'pointerup', 'mouseup', 'click'].forEach(type => {{
                    el.dispatchEvent(new MouseEvent(type, {{ bubbles: true, cancelable: true, view: window }}));
                }});
                try {{ el.click(); }} catch(e) {{}}
            }}

            // 1. Takvimi Aç 📅
            let takvimKutusu = document.querySelector('.dp__main') || document.querySelector('div[class*=""dp__main""]');
            if (takvimKutusu) {{
                safeClick(takvimKutusu);
                await bekle(600);
            }}

            // Görünür ayları oku (Sol ve Sağ takvim)
            function getGorunurAylar() {{
                let monthBtns = Array.from(document.querySelectorAll('button[aria-label=""Open months overlay""], .dp__month_year_select'));
                return monthBtns.map(b => (b.innerText || b.textContent || '').trim().toLowerCase());
            }}

            // 2. Alış ayına ilerle ➡️
            let hedefAlisAy = '{alisAy}'.toLowerCase();
            let maxTur1 = 12;
            while (maxTur1-- > 0) {{
                let gorunurAylar = getGorunurAylar();
                if (gorunurAylar.some(ay => ay.includes(hedefAlisAy))) break;

                let nextBtns = Array.from(document.querySelectorAll('button[aria-label=""Next month""], .dp__month_year_col_nav'));
                let nextBtn = nextBtns[nextBtns.length - 1] || nextBtns[0];
                if (nextBtn) {{
                    safeClick(nextBtn);
                    await bekle(400);
                }} else break;
            }}

            // 3. Gün Bulma Fonksiyonu
            function gunSec(gunStr, ayStr) {{
                let hedefSayi = parseInt(gunStr);
                let calendars = Array.from(document.querySelectorAll('.dp__instance_calendar'));
                
                for (let cal of calendars) {{
                    let calAy = (cal.querySelector('button[aria-label=""Open months overlay""]')?.innerText || '').toLowerCase();
                    if (!ayStr || calAy.includes(ayStr)) {{
                        let cells = Array.from(cal.querySelectorAll('.dp__cell_inner, .dp__calendar_item, [class*=""cell""]')).filter(el => {{
                            let pasif = el.classList.contains('dp__outer_month') || el.classList.contains('dp__cell_disabled') || el.classList.contains('disabled');
                            let txt = (el.innerText || el.textContent || '').trim();
                            return !pasif && (txt === gunStr || parseInt(txt) === hedefSayi);
                        }});
                        if (cells.length > 0) return cells[0];
                    }}
                }}

                let allCells = Array.from(document.querySelectorAll('.dp__cell_inner, .dp__calendar_item, [class*=""cell""]')).filter(el => {{
                    let pasif = el.classList.contains('dp__outer_month') || el.classList.contains('dp__cell_disabled') || el.classList.contains('disabled');
                    let txt = (el.innerText || el.textContent || '').trim();
                    return !pasif && (txt === gunStr || parseInt(txt) === hedefSayi);
                }});
                return allCells[0] || null;
            }}

            // 4. Alış Gününü Seç 🎯
            let elBaslangic = gunSec('{baslangicGunu}', hedefAlisAy);
            if (elBaslangic) {{
                safeClick(elBaslangic);
            }}
            await bekle(800);

            // 5. Veriş ayına ilerle ➡️
            let hedefDonusAy = '{donusAy}'.toLowerCase();
            let maxTur2 = 12;
            while (maxTur2-- > 0) {{
                let gorunurAylar = getGorunurAylar();
                if (gorunurAylar.some(ay => ay.includes(hedefDonusAy))) break;

                let nextBtns = Array.from(document.querySelectorAll('button[aria-label=""Next month""], .dp__month_year_col_nav'));
                let nextBtn = nextBtns[nextBtns.length - 1] || nextBtns[0];
                if (nextBtn) {{
                    safeClick(nextBtn);
                    await bekle(400);
                }} else break;
            }}

            // 6. Veriş Gününü Seç 🎯
            let elBitis = gunSec('{bitisGunu}', hedefDonusAy);
            if (elBitis) {{
                safeClick(elBitis);
            }}
            await bekle(600);

            // 7. Seçilen Tarihleri Onayla 🎯
            let secUygulaBtn = document.querySelector('.dp__action_select') || 
                               document.querySelector('.dp__select') || 
                               document.querySelector('button[class*=""select""]') || 
                               Array.from(document.querySelectorAll('button')).find(b => b.innerText && (b.innerText.toLowerCase().includes('seç') || b.innerText.toLowerCase().includes('uygula')));

            if (secUygulaBtn) {{
                safeClick(secUygulaBtn);
            }} else {{
                document.body.click();
            }}
        }})();
    ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsScript);
            }
        }

        private async Task SaatleriSec(string alisSaati, string donusSaati)
        {
            // 1. ADIM: ALIŞ SAATİ SEÇİMİ (1. Kutu)
            string jsAlisSaat = $@"
                (async function() {{
                    const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

                    let saatKutulari = Array.from(document.querySelectorAll('div[class=""flex items-center gap-2 h-full w-full min-w-0""]'));
                    if (saatKutulari.length === 0) {{
                        saatKutulari = Array.from(document.querySelectorAll('div[class*=""min-w-0""]'));
                    }}

                    if (saatKutulari.length > 0) {{
                        // 1. Kutu (Alış Saati)
                        saatKutulari[0].focus();
                        saatKutulari[0].click();
                        await bekle(400);

                        let saatLiListesi = Array.from(document.querySelectorAll('.hour-li'));
                        let hedef = saatLiListesi.find(li => li.innerText.trim() === '{alisSaati}');
                        if (hedef) {{
                            hedef.focus();
                            hedef.click();
                        }}
                    }}
                }})();
            ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsAlisSaat);
            }

            // 🛑 ALIŞ SAATİ BİTTİKTEN SONRA BEKLEME (700ms)
            await Task.Delay(700);

            // 2. ADIM: BIRAKIŞ (DÖNÜŞ) SAATİ SEÇİMİ (2. Kutu)
            string jsDonusSaat = $@"
                (async function() {{
                    const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

                    // Güncel DOM'dan saat kutularını oku
                    let saatKutulari = Array.from(document.querySelectorAll('div[class=""flex items-center gap-2 h-full w-full min-w-0""]'));
                    if (saatKutulari.length === 0) {{
                        saatKutulari = Array.from(document.querySelectorAll('div[class*=""min-w-0""]'));
                    }}

                    if (saatKutulari.length > 1) {{
                        // 2. Kutu (Bırakış/Dönüş Saati)
                        saatKutulari[1].focus();
                        saatKutulari[1].click();
                        await bekle(500);

                        let saatLiListesi = Array.from(document.querySelectorAll('.hour-li'));
                        let hedef = saatLiListesi.find(li => li.innerText.trim() === '{donusSaati}');
                        if (hedef) {{
                            hedef.focus();
                            hedef.click();
                        }}
                    }}
                }})();
            ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsDonusSaat);
            }
        }

        private async Task<bool> ElemanGelenekadarBekle(string elementId, int timeoutSaniye = 15)
        {
            // ⏱️ Zaman aşımı süresini belirliyoruz
            DateTime bitisZamani = DateTime.Now.AddSeconds(timeoutSaniye);

            while (DateTime.Now < bitisZamani)
            {
                try
                {
                    // JavaScript çalıştırılabilir durumda mı?
                    if (chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                    {
                        string script = $"document.getElementById('{elementId}') !== null;";
                        var response = await chromiumWebBrowser.EvaluateScriptAsync(script);

                        // 🟢 Eleman sayfada bulunduysa döngüden başarıyla çık
                        if (response.Success && response.Result is bool varMi && varMi)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Yönlendirme anındaki geçici hataları yut, bir sonraki turda tekrar dene
                }

                // Arka planı kilitlememek için 500 ms bekle ⏳
                await Task.Delay(500);
            }

            // 🚨 Süre doldu ve eleman bulunamadı!
            MessageBox.Show(
                $"Aranan eleman ('{elementId}') {timeoutSaniye} saniye içinde bulunamadı. Lütfen sayfayı kontrol edin.",
                "Zaman Aşımı ⏳",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }

        // 🚪 KAMPANYA POPUP'INI OTOMATİK KAPATMA METODU
        private async Task PopupKapat()
        {
            string jsPopupKapat = @"
                (function() {
                    // 1. Ekran görüntünüzdeki kapatma butonuna tıkla
                    let closeBtn = document.querySelector('.gs_trigger_discount_popup_close_container') || 
                                   document.querySelector('.gs_trigger_discount_popup_modal');
                    if (closeBtn) {
                        closeBtn.click();
                    }

                    // 2. Garanti Kapatma: Popup nesnesini DOM'dan tamamen kaldır (#gs_popup_modal_parent)
                    let popupModal = document.querySelector('#gs_popup_modal_parent');
                    if (popupModal) {
                        popupModal.remove();
                    }
                })();
            ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsPopupKapat);
            }
        }

        private async Task FiltreleriUygula(string vitesTuru, string yakitTuru)
        {
            vitesTuru = (vitesTuru ?? "").Trim();
            yakitTuru = (yakitTuru ?? "").Trim();

            // 🛑 1. ADIM: Ekranda Filtre Paneli / "Vites Tipi" Belirene Kadar Bekle
            bool panelYuklendi = false;
            for (int i = 0; i < 30; i++)
            {
                if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    var testRes = await chromiumWebBrowser.EvaluateScriptAsync(
                        "Array.from(document.querySelectorAll('div, span, p')).some(el => (el.innerText || '').trim().includes('Vites Tipi'));"
                    );
                    if (testRes.Success && testRes.Result is bool hazir && hazir)
                    {
                        panelYuklendi = true;
                        break;
                    }
                }
                await Task.Delay(500);
            }

            if (!panelYuklendi) return;
            await Task.Delay(1000);

            // 🛑 2. ADIM: Tekli Tıklama Ve Akıllı Akordeon Mantığı İle Filtrele
            string jsScript = $@"
        (async function() {{
            const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

            // Temiz Ve Tekli Tıklama Fonksiyonu ⚡
            function temizTikla(el) {{
                if (!el) return;
                try {{
                    if (typeof el.click === 'function') {{
                        el.click();
                    }} else {{
                        el.dispatchEvent(new MouseEvent('click', {{ bubbles: true, cancelable: true, view: window }}));
                    }}
                }} catch(e) {{}}
            }}

            // Akordeon Başlığı Tıklama
            async function akordeonAc(baslikMetni) {{
                let tumu = Array.from(document.querySelectorAll('div, span, p'));
                let hedef = tumu.find(el => {{
                    let txt = (el.innerText || el.textContent || '').trim();
                    return el.offsetWidth > 0 && el.offsetHeight > 0 && txt === baslikMetni;
                }});

                if (!hedef) {{
                    hedef = tumu.find(el => {{
                        let txt = (el.innerText || el.textContent || '').trim();
                        return el.offsetWidth > 0 && el.offsetHeight > 0 && txt.includes(baslikMetni);
                    }});
                }}

                if (hedef) {{
                    let tiklanacak = hedef.closest('div[class*=""cursor-pointer""]') || hedef.closest('div[class*=""flex""]') || hedef.parentElement || hedef;
                    tiklanacak.scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                    await bekle(400);
                    temizTikla(tiklanacak);
                    await bekle(1000);
                }}
            }}

            // Kutucuk Bul Ve Seç (Zaten Görünürse Akordeona Dokunmaz!)
            async function kutucuguBulVeSec(inputId, baslikMetni) {{
                let input = document.getElementById(inputId) || document.querySelector(`input[id*=""${{inputId}}""]`);

                // Kutucuk yoksa veya görünür değilse (akordeon kapalıysa) başlığa tıkla ve aç
                if (!input || input.offsetWidth === 0 || input.offsetHeight === 0) {{
                    await akordeonAc(baslikMetni);
                    await bekle(500);
                    input = document.getElementById(inputId) || document.querySelector(`input[id*=""${{inputId}}""]`);
                }}

                // Kutucuk seçili değilse tıkla
                if (input && !input.checked) {{
                    temizTikla(input);
                    await bekle(500);
                    if (!input.checked && input.parentElement) {{
                        temizTikla(input.parentElement);
                    }}
                    await bekle(800);
                }}
            }}

            // -----------------------------------------------------
            // ⚙️ 1. VİTES FİLTRESİ 🚗
            // -----------------------------------------------------
            if ('{vitesTuru}' === 'Manuel') {{
                await kutucuguBulVeSec('filter-transmission.1', 'Vites Tipi');
            }}
            else if ('{vitesTuru}' === 'Otomatik') {{
                await kutucuguBulVeSec('filter-transmission.2', 'Vites Tipi');
            }}

            await bekle(1000);

            // -----------------------------------------------------
            // ⛽ 2. YAKIT FİLTRESİ ⛽
            // -----------------------------------------------------
            if ('{yakitTuru}' !== 'Tümü') {{
                if ('{yakitTuru}' === 'Benzin') {{
                    await kutucuguBulVeSec('filter-fuel.1', 'Yakıt Tipi');
                    await kutucuguBulVeSec('filter-fuel.8', 'Yakıt Tipi');
                }}
                else if ('{yakitTuru}' === 'Dizel') {{
                    await kutucuguBulVeSec('filter-fuel.2', 'Yakıt Tipi');
                    await kutucuguBulVeSec('filter-fuel.8', 'Yakıt Tipi');
                }}
                else if ('{yakitTuru}' === 'Hibrit') {{
                    await kutucuguBulVeSec('filter-fuel.7', 'Yakıt Tipi');
                }}
                else if ('{yakitTuru}' === 'Elektrik') {{
                    await kutucuguBulVeSec('filter-fuel.11', 'Yakıt Tipi');
                }}
            }}
        }})();
    ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsScript);
            }
        }

        private async void aracgetir()
        {
            // 🛡️ ÇAPRAZ İŞ PARÇACIĞI (CROSS-THREAD) GÜVENLİK KORUMASI
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(aracgetir));
                return;
            }

            bool bulundu = await ElemanGelenekadarBekle("inputPickUpLocation", 15);

            if (bulundu)
            {
                // Form Kontrollerindeki Değerleri UI Thread Üzerinde Güvenle Oku
                string neredenMetin = tbAlısyeri.Text;
                DateTime alisTar = dtpAlisTarihi.Value;
                DateTime verisTar = dtpVerisTarihi.Value;
                string alisSaat = cbAlısSaati.SelectedItem?.ToString() ?? cbAlısSaati.Text;
                string donusSaat = cbDonusSaati.SelectedItem?.ToString() ?? cbDonusSaati.Text;
                string vitesSecimi = cbVites.SelectedItem?.ToString() ?? cbVites.Text;
                string yakitSecimi = cbYakit.SelectedItem?.ToString() ?? cbYakit.Text;

                // 🛑 EKRANDAKİ KAMPANYA POPUP'INI OTOMATİK KAPAT 🚪
                await PopupKapat();
                await Task.Delay(400);

                // 1. Alış Yerini Seç
                bool alisBasarili = await alisyeri(neredenMetin);

                if (alisBasarili)
                {
                    await Task.Delay(500);
                    // 2. Tarihleri Seç
                    await TarihAraligiSec(alisTar, verisTar);

                    await Task.Delay(500);
                    // 3. Saatleri Seç (Sırayla Alış ve Veriş)
                    await SaatleriSec(alisSaat, donusSaat);

                    await Task.Delay(500);
                    // 4. Aramayı Başlat (#search butonuna tıkla)
                    await ElemanaGercekTiklamaYap("#search");

                    // 5. Araç Listesinin Yüklenmesini Bekle Ve Filtreleri Uygula 🚗⛽
                    await Task.Delay(3500);
                    await FiltreleriUygula(vitesSecimi, yakitSecimi);
                    await AraclariVeriTablosunaAktar();
                }
            }
        }

        private void dgbVeriEkranı_Hazirla()
        {
            dgbVeriEkranı.Columns.Clear();
            dgbVeriEkranı.Columns.Add("Model", "Araç Modeli");
            dgbVeriEkranı.Columns.Add("Sirket", "Kiralama Şirketi");
            dgbVeriEkranı.Columns.Add("Vites", "Vites Tipi");
            dgbVeriEkranı.Columns.Add("Yakit", "Yakıt Türü");
            dgbVeriEkranı.Columns.Add("Fiyat", "Fiyat");

            // 🎯 Ekran Bölgenizi %100 Dolduracak Ayarlar:
            dgbVeriEkranı.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgbVeriEkranı.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgbVeriEkranı.AllowUserToAddRows = false;
            dgbVeriEkranı.AllowUserToDeleteRows = false;
            dgbVeriEkranı.ReadOnly = true;
            dgbVeriEkranı.RowHeadersVisible = false;
            dgbVeriEkranı.ScrollBars = ScrollBars.Both;

            // Görsel Hizalamalar
            dgbVeriEkranı.Columns["Vites"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgbVeriEkranı.Columns["Yakit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgbVeriEkranı.Columns["Fiyat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void PopuleEt(System.Collections.IEnumerable list)
        {
            dgbVeriEkranı_Hazirla();
            dgbVeriEkranı.Rows.Clear();

            foreach (var item in list)
            {
                if (item is IDictionary<string, object> dict)
                {
                    string model = dict.ContainsKey("model") ? dict["model"]?.ToString() : "";
                    string sirket = dict.ContainsKey("sirket") ? dict["sirket"]?.ToString() : "";
                    string vites = dict.ContainsKey("vites") ? dict["vites"]?.ToString() : "";
                    string yakit = dict.ContainsKey("yakit") ? dict["yakit"]?.ToString() : "";
                    string fiyat = dict.ContainsKey("fiyat") ? dict["fiyat"]?.ToString() : "";

                    dgbVeriEkranı.Rows.Add(model, sirket, vites, yakit, fiyat);
                }
            }

            KoleksiyonButonDurumuGuncelle();
        }

        private async Task AraclariVeriTablosunaAktar()
        {
            await Task.Delay(1500); // Filtreleme sonrası araç listesinin tazelenmesini bekle

            string jsScript = @"
                (async function() {
                    const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

                    // 📜 1. Sayfayı Aşağı Doğru Kaydırarak Tüm Araçları Yükle (Lazy Load)
                    let sonYukseklik = 0;
                    let ayniYukseklikSayaci = 0;

                    for (let i = 0; i < 20; i++) {
                        window.scrollTo(0, document.body.scrollHeight);
                        await bekle(600);

                        let suAnkiYukseklik = document.body.scrollHeight;
                        if (suAnkiYukseklik === sonYukseklik) {
                            ayniYukseklikSayaci++;
                            if (ayniYukseklikSayaci >= 2) break; // Artık yeni araç yüklenmiyorsa dur
                        } else {
                            ayniYukseklikSayaci = 0;
                            sonYukseklik = suAnkiYukseklik;
                        }
                    }

                    // Sayfayı tekrar en üste getir
                    window.scrollTo(0, 0);
                    await bekle(300);

                    // 🎯 2. Yüklenen Tüm Araç Kartlarını Eksiksiz Çek
                    let priceElements = Array.from(document.querySelectorAll('#car_total_price'));
                    let kartlar = priceElements.map(priceEl => {
                        return priceEl.closest('div[class*=""border""]') || priceEl.closest('div[class*=""bg-white""]') || priceEl.parentElement;
                    });

                    let uniqueKartlar = Array.from(new Set(kartlar.filter(Boolean)));

                    return uniqueKartlar.map(kart => {
                        let modelEl = kart.querySelector('div[class*=""text-dark-gray text-lg font-bold""]') || kart.querySelector('div[class*=""font-bold""]');
                        let model = modelEl ? (modelEl.innerText || modelEl.textContent || '').trim() : '';

                        let imgEl = kart.querySelector('figure img') || kart.querySelector('img[alt]');
                        let sirket = imgEl ? (imgEl.getAttribute('alt') || imgEl.getAttribute('id') || '').trim() : '';

                        let vitesEl = kart.querySelector('span[data-cms-key*=""filter_transmission""]');
                        let vites = vitesEl ? (vitesEl.innerText || vitesEl.textContent || '').trim() : '';

                        let yakitEl = kart.querySelector('span[data-cms-key*=""filter_fuel""]');
                        let yakit = yakitEl ? (yakitEl.innerText || yakitEl.textContent || '').trim() : '';

                        let fiyatEl = kart.querySelector('#car_total_price');
                        let fiyat = fiyatEl ? (fiyatEl.innerText || fiyatEl.textContent || '').trim() : '';

                        return {
                            model: model,
                            sirket: sirket,
                            vites: vites,
                            yakit: yakit,
                            fiyat: fiyat
                        };
                    });
                })();
            ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                var response = await chromiumWebBrowser.EvaluateScriptAsync(jsScript);
                if (response.Success && response.Result != null)
                {
                    var list = response.Result as System.Collections.IEnumerable;
                    if (list != null)
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => PopuleEt(list)));
                        }
                        else
                        {
                            PopuleEt(list);
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
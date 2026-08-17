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

            // Otomasyon izlerini gizlemek için gelişmiş Stealth scripti 🥷
            chromiumWebBrowser.FrameLoadStart += (sender, args) =>
            {
                // Sadece ana sayfa (Main Frame) yüklenirken çalıştırıyoruz
                if (args.Frame.IsMain)
                {
                    string stealthScript = @"
                        // 1. Webdriver Gizleme
                        Object.defineProperty(navigator, 'webdriver', { get: () => undefined });
                        try { delete navigator.__proto__.webdriver; } catch(e) {}

                        // 2. Diller ve Eklentiler
                        Object.defineProperty(navigator, 'languages', { get: () => ['tr-TR', 'tr', 'en-US', 'en'] });
                        Object.defineProperty(navigator, 'plugins', { get: () => [1, 2, 3, 4, 5] });

                        // 3. Donanım Özellikleri (Gerçek PC Profili)
                        Object.defineProperty(navigator, 'hardwareConcurrency', { get: () => 8 });
                        Object.defineProperty(navigator, 'deviceMemory', { get: () => 8 });

                        // 4. Chrome Nesnesi ve Runtime Simülasyonu
                        if (!window.chrome) { window.chrome = {}; }
                        if (!window.chrome.runtime) { window.chrome.runtime = {}; }
                        if (!window.chrome.loadTimes) { window.chrome.loadTimes = function() {}; }
                        if (!window.chrome.csi) { window.chrome.csi = function() {}; }
                        if (!window.chrome.app) { window.chrome.app = {}; }

                        // 5. WebGL Gerçek GPU Maskeleme
                        try {
                            const getParameter = WebGLRenderingContext.prototype.getParameter;
                            WebGLRenderingContext.prototype.getParameter = function(parameter) {
                                if (parameter === 37445) return 'Google Inc. (NVIDIA)';
                                if (parameter === 37446) return 'ANGLE (NVIDIA, NVIDIA GeForce RTX 3060 Direct3D11 vs_5_0 ps_5_0, D3D11)';
                                return getParameter.apply(this, arguments);
                            };
                        } catch(e) {}

                        // 6. İzinler (Permissions API)
                        if (navigator.permissions && navigator.permissions.query) {
                            const originalQuery = navigator.permissions.query;
                            navigator.permissions.query = (parameters) => (
                                parameters.name === 'notifications' ?
                                Promise.resolve({ state: Notification.permission }) :
                                originalQuery(parameters)
                            );
                        }
                    ";

                    // Sayfa kaynakları yüklenmeye başlar başlamaz JS'i enjekte ediyoruz
                    args.Frame.ExecuteJavaScriptAsync(stealthScript);
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
        // ⏱️ Genel Koşul Bekleyici (Şart sağlandığı an true döner, beklemez)
        private static async Task<bool> WaitUntilAsync(
            Func<Task<bool>> condition,
            TimeSpan timeout,
            TimeSpan? pollInterval = null)
        {
            TimeSpan interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
            DateTime bitisZamani = DateTime.Now.Add(timeout);

            while (DateTime.Now < bitisZamani)
            {
                try
                {
                    if (await condition())
                        return true;
                }
                catch { }

                await Task.Delay(interval);
            }

            return false;
        }

        // 🎯 Sayfada CSS Elemanı Gelene Kadar Bekleyen Yardımcı Fonksiyon
        private async Task<bool> WaitForElementAsync(string cssSelector, TimeSpan timeout, TimeSpan? pollInterval = null)
        {
            return await WaitUntilAsync(async () =>
            {
                if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
                {
                    string js = $@"
                        (function() {{
                            let el = document.querySelector('{cssSelector.Replace("'", "\\'")}');
                            return el != null && (el.offsetWidth > 0 || el.offsetHeight > 0 || el.getClientRects().length > 0);
                        }})();
                    ";
                    var res = await chromiumWebBrowser.EvaluateScriptAsync(js);
                    return res.Success && res.Result != null && Convert.ToBoolean(res.Result);
                }
                return false;
            }, timeout, pollInterval);
        }

        private async Task<bool> alisyeri(string nereden)
        {
            if (string.IsNullOrEmpty(nereden)) return false;

            // 🇹🇷 Türkçe Kurallarına Göre Baş Harfi Otomatik Büyüt (örn: 'düzce' -> 'Düzce', 'istanbul' -> 'İstanbul')
            var trKultur = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
            string duzenliNereden = trKultur.TextInfo.ToTitleCase(nereden.Trim().ToLower(trKultur));

            if (chromiumWebBrowser != null && chromiumWebBrowser.IsBrowserInitialized)
            {
                // 1. Kutunun hazır olmasını bekle 🎯
                bool kutuHazir = await WaitForElementAsync("#inputPickUpLocation, input[placeholder*='Alış'], input[placeholder*='Nereden']", TimeSpan.FromSeconds(5));
                if (!kutuHazir) return false;

                // 2. Dispatch ile Kutuyu Temizle ve Harf Harf Yaz ⌨️
                string klavyeYazJs = $@"
                    (async function() {{
                        const bekle = (ms) => new Promise(r => setTimeout(r, ms));
                        let input = document.querySelector('#inputPickUpLocation') || 
                                    document.querySelector('input[placeholder*=""Alış""]') ||
                                    document.querySelector('input[placeholder*=""Nereden""]');
                        if (!input) return false;

                        // Varsa (X) temizleme butonunu tıkla
                        let clearBtn = input.parentElement?.querySelector('button, svg, [class*=""clear""], [class*=""close""]');
                        if (clearBtn) {{
                            clearBtn.dispatchEvent(new MouseEvent('click', {{ bubbles: true, cancelable: true }}));
                        }}

                        input.focus();
                        input.dispatchEvent(new MouseEvent('click', {{ bubbles: true, cancelable: true }}));
                        input.select();
                        await bekle(30);

                        // 🧹 1. Kutuyu Boşalt ve Input/Change Event'leri Dispatch Et
                        let nativeSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value')?.set;
                        if (nativeSetter) nativeSetter.call(input, '');
                        input.value = '';
                        let trk = input._valueTracker;
                        if (trk) trk.setValue('');
                        input.dispatchEvent(new Event('input', {{ bubbles: true, composed: true }}));
                        input.dispatchEvent(new Event('change', {{ bubbles: true, composed: true }}));
                        await bekle(60);

                        // ✍️ 2. Baş Harfi Düzeltilmiş Şehri Harf Harf Yaz (örn: 'Düzce')
                        let metin = '{duzenliNereden.Replace("'", "\\'")}';
                        let val = '';

                        for (let char of metin) {{
                            val += char;
                            if (nativeSetter) nativeSetter.call(input, val);
                            input.value = val;
                            if (input._valueTracker) input._valueTracker.setValue(val);

                            let keyProps = {{ key: char, code: 'Key' + char.toUpperCase(), charCode: char.charCodeAt(0), keyCode: char.charCodeAt(0), which: char.charCodeAt(0), bubbles: true, cancelable: true, composed: true }};
                            
                            input.dispatchEvent(new KeyboardEvent('keydown', keyProps));
                            input.dispatchEvent(new KeyboardEvent('keypress', keyProps));
                            input.dispatchEvent(new Event('input', {{ bubbles: true, composed: true }}));
                            input.dispatchEvent(new KeyboardEvent('keyup', keyProps));
                            await bekle(35);
                        }}

                        input.dispatchEvent(new Event('change', {{ bubbles: true, composed: true }}));
                        return true;
                    }})();
                ";

                await chromiumWebBrowser.EvaluateScriptAsync(klavyeYazJs);

                // 3. Açılan Listede Yeni Sonuçların Gelmesini Bekle ve Şehri Seç 🎯
                string akilliSecimJs = $@"
                    (async function() {{
                        const bekle = (ms) => new Promise(r => setTimeout(r, ms));

                        function normalize(str) {{
                            return (str || '')
                                .toLocaleLowerCase('tr-TR')
                                .replace(/İ/g, 'i').replace(/I/g, 'i').replace(/ı/g, 'i')
                                .replace(/ğ/g, 'g').replace(/ü/g, 'u').replace(/ş/g, 's').replace(/ö/g, 'o').replace(/ç/g, 'c')
                                .trim();
                        }}

                        let hedef = normalize('{duzenliNereden.Replace("'", "\\'")}');
                        let retries = 40;
                        let secilecek = null;

                        while (retries-- > 0) {{
                            let items = Array.from(document.querySelectorAll('.location-item, div[class*=""location-item""]'));
                            
                            // 🛡️ Sadece yeni aranan şehre ait gelen güncel sonuçları filtrele
                            let guncelSonuclar = items.filter(el => {{
                                let t = normalize(el.innerText || '');
                                return t.includes(hedef);
                            }});

                            if (guncelSonuclar.length > 0) {{
                                // 🎯 1. ÖNCELİK: Birebir Şehir Eşleşmesi (Havalimanı/Gar hariç, Merkez/Türkiye dahil)
                                secilecek = guncelSonuclar.find(el => {{
                                    let titleEl = el.querySelector('div.flex.flex-row > div:first-child') || el.querySelector('div > div') || el;
                                    let t = normalize(titleEl.innerText || el.innerText || '');
                                    let tTemiz = t.replace(/,?\s*turkiye$/i, '').replace(/\s*merkez$/i, '').replace(/\s*sehir$/i, '').trim();
                                    return tTemiz === hedef;
                                }});

                                // 🎯 2. ÖNCELİK: Havalimanı/Gar OLMAYAN ilk ana şehir kaydı
                                if (!secilecek) {{
                                    secilecek = guncelSonuclar.find(el => {{
                                        let titleEl = el.querySelector('div.flex.flex-row > div:first-child') || el.querySelector('div > div') || el;
                                        let t = normalize(titleEl.innerText || el.innerText || '');
                                        let poiDegil = !t.includes('havaliman') && !t.includes('havaalan') && !t.includes('gar') && !t.includes('terminal') && !t.includes('otogar');
                                        return poiDegil;
                                    }});
                                }}

                                if (secilecek) break;
                            }}

                            await bekle(50);
                        }}

                        if (secilecek) {{
                            secilecek.scrollIntoView?.({{ block: 'nearest' }});
                            secilecek.focus?.();
                            ['pointerdown', 'mousedown', 'pointerup', 'mouseup', 'click'].forEach(evt => {{
                                secilecek.dispatchEvent(new MouseEvent(evt, {{ bubbles: true, cancelable: true, view: window }}));
                            }});
                            try {{ secilecek.click(); }} catch(e) {{}}
                            return true;
                        }}
                        return false;
                    }})();
                ";

                var res = await chromiumWebBrowser.EvaluateScriptAsync(akilliSecimJs);
                return res.Success && res.Result != null && Convert.ToBoolean(res.Result);
            }
            return false;
        }

        private async Task<bool> TarihAraligiSec(DateTime alisTarihi, DateTime donusTarihi)
        {
            string alisTarihStr = alisTarihi.ToString("yyyy-MM-dd");
            string donusTarihStr = donusTarihi.ToString("yyyy-MM-dd");

            string jsScript = $@"
        (async function() {{
            const bekle = (ms) => new Promise(r => setTimeout(r, ms));

            // 1. Takvim Kutusunu Aç 📅
            let acilacakKutu = document.querySelector('div[modaltitlecmskey=""pickup_and_dropoff_date""] [class*=""cursor-pointer""]') ||
                               document.querySelector('div[modaltitlecmskey=""pickup_and_dropoff_date""]') ||
                               document.querySelector('div[modaltitle*=""Alış ve Bırakış""] [class*=""cursor-pointer""]');

            if (acilacakKutu && !document.querySelector('.cal-months')) {{
                acilacakKutu.click();
            }}

            let r1 = 30;
            while (r1-- > 0) {{
                if (document.querySelector('.cal-months')) break;
                if (r1 === 15 && acilacakKutu) acilacakKutu.click();
                await bekle(25);
            }}

            if (!document.querySelector('.cal-months')) return false;

            // 2. Alış Tarihini Seç (SADECE 1 KEZ SAF td.click()) 🎯
            let hedefAlis = '{alisTarihStr}';
            let maxTur1 = 12;
            let alisSecildi = false;

            while (maxTur1-- > 0) {{
                let elBaslangic = document.querySelector(`td[data-day=""${{hedefAlis}}""]`);
                if (elBaslangic && !elBaslangic.classList.contains('disabled')) {{
                    elBaslangic.click();
                    alisSecildi = true;
                    break;
                }}

                let nextBtn = document.querySelector('button.cal-nav.right-4') || document.querySelector('button[class*=""cal-nav""][class*=""right""]');
                if (nextBtn && !nextBtn.disabled) {{
                    nextBtn.click();
                    await bekle(60);
                }} else break;
            }}

            if (!alisSecildi) return false;

            // 1. Günün seçilip durumun geçmesi için 200ms bekle
            await bekle(200);

            // 3. Bırakış Tarihini Seç (SADECE 1 KEZ SAF td.click()) 🎯
            let hedefDonus = '{donusTarihStr}';
            let maxTur2 = 12;
            let donusSecildi = false;

            while (maxTur2-- > 0) {{
                let elBitis = document.querySelector(`td[data-day=""${{hedefDonus}}""]`);
                if (elBitis && !elBitis.classList.contains('disabled')) {{
                    elBitis.click();
                    donusSecildi = true;
                    break;
                }}

                let nextBtn = document.querySelector('button.cal-nav.right-4') || document.querySelector('button[class*=""cal-nav""][class*=""right""]');
                if (nextBtn && !nextBtn.disabled) {{
                    nextBtn.click();
                    await bekle(60);
                }} else break;
            }}

            if (!donusSecildi) return false;

            // 4. Varsa Seç/Uygula Butonuna Tıkla
            await bekle(100);
            let secUygulaBtn = document.querySelector('button[class*=""select""]') || 
                               document.querySelector('button[class*=""apply""]') || 
                               Array.from(document.querySelectorAll('button')).find(b => b.innerText && (b.innerText.toLowerCase().includes('seç') || b.innerText.toLowerCase().includes('uygula')));

            if (secUygulaBtn) {{
                secUygulaBtn.click();
            }}

            return true;
        }})();
    ";

    if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
    {
        var res = await chromiumWebBrowser.EvaluateScriptAsync(jsScript);
        return res.Success && res.Result != null && Convert.ToBoolean(res.Result);
    }
    return false;
}

        private async Task<bool> SaatleriSec(string alisSaati, string donusSaati)
        {
            if (string.IsNullOrEmpty(alisSaati) || string.IsNullOrEmpty(donusSaati)) return false;

            string jsSaatSecim = $@"
                (async function() {{
                    const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

                    async function saatAyarla(index, hedefSaat) {{
                        // v2 flex kutu seçicisi
                        let saatKutulari = Array.from(document.querySelectorAll('div[class=""flex items-center gap-2 h-full w-full min-w-0""]'));
                        if (saatKutulari.length === 0) {{
                            saatKutulari = Array.from(document.querySelectorAll('div[class*=""min-w-0""]'));
                        }}

                        if (saatKutulari.length > index) {{
                            let kutu = saatKutulari[index];
                            kutu.focus();
                            kutu.click();

                            // Dinamik: 700ms beklemek yerine .hour-li açıldığı an (25ms aralıkla) tıklar
                            let retries = 25;
                            while (retries-- > 0) {{
                                let saatLiListesi = Array.from(document.querySelectorAll('.hour-li'));
                                let hedef = saatLiListesi.find(li => li.innerText.trim() === hedefSaat);
                                if (hedef) {{
                                    hedef.scrollIntoView?.({{ block: 'nearest' }});
                                    hedef.focus();
                                    hedef.click();
                                    return true;
                                }}
                                await bekle(25);
                            }}
                        }}
                        return false;
                    }}

                    // 1. Alış Saati
                    let ok1 = await saatAyarla(0, '{alisSaati}');
                    if (!ok1) return false;

                    await bekle(60);

                    // 2. Bırakış Saati
                    let ok2 = await saatAyarla(1, '{donusSaati}');
                    return ok2;
                }})();
            ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                var res = await chromiumWebBrowser.EvaluateScriptAsync(jsSaatSecim);
                return res.Success && res.Result != null && Convert.ToBoolean(res.Result);
            }
            return false;
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

            if ((string.IsNullOrEmpty(vitesTuru) || vitesTuru == "Tümü") && 
                (string.IsNullOrEmpty(yakitTuru) || yakitTuru == "Tümü")) 
                return;

            // 🛑 1. ADIM: Arama Sonuç Sayfasındaki Sol Filtre Panelinin Gelmesini Dinamik Bekle ⚡
            bool panelHazir = await WaitForElementAsync("#stickyFilterCard, [id*='stickyFilter'], details[data-cms-key*='filter']", TimeSpan.FromSeconds(15));
            if (!panelHazir) return;

            // 🛑 2. ADIM: <details> ve <label> Etiketlerine Tam Tıklama Gönder
            string jsScript = $@"
        (async function() {{
            const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

            function safeClick(el) {{
                if (!el) return;
                el.scrollIntoView?.({{ block: 'center' }});
                el.focus?.();
                ['pointerdown', 'mousedown', 'pointerup', 'mouseup', 'click'].forEach(t => {{
                    el.dispatchEvent(new MouseEvent(t, {{ bubbles: true, cancelable: true, view: window }}));
                }});
                try {{ el.click(); }} catch(e) {{}}
            }}

            async function akordeonGarantiAc(cmsKey) {{
                let details = document.querySelector(`details[data-cms-key=""${{cmsKey}}""]`);
                if (details && !details.open) {{
                    let summary = details.querySelector('summary') || details;
                    safeClick(summary);
                    await bekle(250);
                }}
            }}

            async function filtreSec(filterName, cmsKey) {{
                await akordeonGarantiAc(cmsKey);
                await bekle(150);

                let label = document.querySelector(`label[name=""${{filterName}}""]`) || 
                            document.querySelector(`label[for=""${{filterName}}""]`) ||
                            document.getElementById(filterName)?.closest('label');

                let input = document.getElementById(filterName) || document.querySelector(`input[id=""${{filterName}}""]`);

                if (label) {{
                    safeClick(label);
                    await bekle(200);
                }} else if (input) {{
                    safeClick(input);
                    await bekle(200);
                }}

                if (input && !input.checked) {{
                    input.checked = true;
                    input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                    input.dispatchEvent(new Event('input', {{ bubbles: true }}));
                }}
            }}

            // ⚙️ 1. VİTES FİLTRESİ 🚗
            if ('{vitesTuru}' === 'Manuel') {{
                await filtreSec('filter-transmission.1', 'filter_transmission');
            }}
            else if ('{vitesTuru}' === 'Otomatik') {{
                await filtreSec('filter-transmission.2', 'filter_transmission');
            }}

            await bekle(300);

            // ⛽ 2. YAKIT FİLTRESİ ⛽
            if ('{yakitTuru}' !== 'Tümü' && '{yakitTuru}' !== '') {{
                if ('{yakitTuru}' === 'Benzin') {{
                    await filtreSec('filter-fuel.1', 'filter_fuel');
                    await filtreSec('filter-fuel.8', 'filter_fuel');
                }}
                else if ('{yakitTuru}' === 'Dizel') {{
                    await filtreSec('filter-fuel.2', 'filter_fuel');
                    await filtreSec('filter-fuel.8', 'filter_fuel');
                }}
                else if ('{yakitTuru}' === 'Hibrit') {{
                    await filtreSec('filter-fuel.7', 'filter_fuel');
                }}
                else if ('{yakitTuru}' === 'Elektrik') {{
                    await filtreSec('filter-fuel.11', 'filter_fuel');
                }}
            }}

            return true;
        }})();
    ";

            if (chromiumWebBrowser != null && chromiumWebBrowser.CanExecuteJavascriptInMainFrame)
            {
                await chromiumWebBrowser.EvaluateScriptAsync(jsScript);
                await Task.Delay(2000);
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

            // 1. Arama kutusunun hazır olmasını dinamik bekle 🎯
            bool bulundu = await WaitForElementAsync("#inputPickUpLocation", TimeSpan.FromSeconds(15));

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

                // 1. Alış Yerini Seç
                bool alisBasarili = await alisyeri(neredenMetin);
                if (!alisBasarili)
                {
                    MessageBox.Show($"'{neredenMetin}' lokasyonu Yolcu360'ta bulunamadı veya seçilemedi!\nİşlem durduruldu.", "1. Adım Başarısız ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Tarihleri Seç (Dinamik State Kontrollü)
                bool tarihBasarili = await TarihAraligiSec(alisTar, verisTar);
                if (!tarihBasarili)
                {
                    MessageBox.Show("Tarih aralığı takvimden seçilemedi!\nİşlem durduruldu.", "2. Adım Başarısız ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Saatleri Seç (Dinamik)
                bool saatBasarili = await SaatleriSec(alisSaat, donusSaat);
                if (!saatBasarili)
                {
                    MessageBox.Show("Saatler seçilemedi!\nİşlem durduruldu.", "3. Adım Başarısız ⚠️", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Aramayı Başlat (#search butonuna tıkla)
                await ElemanaGercekTiklamaYap("#search");

                // 5. Araç Listesinin Yüklenmesini DİNAMİK BEKLE (Task.Delay 3500ms yerine!) 🚗⚡
                await WaitForElementAsync(".car-card, div[class*='car-card'], [id*='car_']", TimeSpan.FromSeconds(15));

                // 6. Filtreleri Uygula Ve Araçları Aktar
                await FiltreleriUygula(vitesSecimi, yakitSecimi);
                await AraclariVeriTablosunaAktar();
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
            dgbVeriEkranı.Columns["Model"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgbVeriEkranı.Columns["Sirket"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgbVeriEkranı.Columns["Vites"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgbVeriEkranı.Columns["Yakit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgbVeriEkranı.Columns["Fiyat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void PopuleEt(System.Collections.IEnumerable list)
        {
            if (dgbVeriEkranı.Columns.Count == 0)
            {
                dgbVeriEkranı_Hazirla();
            }
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
            // 🛑 1. ADIM: Araç Fiyatlarının Ekrana Yüklenmesini Dinamik Bekle (Kör Task.Delay Yok!) 🚗⚡
            await WaitForElementAsync("#car_total_price, .car-card, div[class*='car-card']", TimeSpan.FromSeconds(10));

            string jsScript = @"
                (async function() {
                    const bekle = (ms) => new Promise(resolve => setTimeout(resolve, ms));

                    // 📜 1. Sayfayı Aşağı Doğru Kaydırarak Tüm Araçları Yükle (Lazy Load)
                    let sonYukseklik = 0;
                    let ayniYukseklikSayaci = 0;

                    for (let i = 0; i < 20; i++) {
                        window.scrollTo(0, document.body.scrollHeight);
                        await bekle(300);

                        let suAnkiYukseklik = document.body.scrollHeight;
                        if (suAnkiYukseklik === sonYukseklik) {
                            ayniYukseklikSayaci++;
                            if (ayniYukseklikSayaci >= 2) break;
                        } else {
                            ayniYukseklikSayaci = 0;
                            sonYukseklik = suAnkiYukseklik;
                        }
                    }

                    window.scrollTo(0, 0);
                    await bekle(150);

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
# 🚗 Yolcu360 Araç Kiralama Otomasyonu

Bu proje, **C# Windows Forms** ve **CefSharp (Chromium Embedded Framework)** kullanılarak geliştirilmiş bir masaüstü web otomasyonu ve veri sorgulama uygulamasıdır. Yolcu360 platformu üzerindeki dinamik web süreçlerini otomatikleştirerek kullanıcı etkileşimlerini simüle eder; lokasyon seçimi, tarih/saat belirleme ve arama adımlarını otomatik olarak gerçekleştirir.

---

## 📌 Temel Özellikler

* **Dinamik Tarayıcı Entegrasyonu:** CefSharp Chromium motoru ile Single Page Application (SPA) mimarisindeki dinamik web sayfalarını sorunsuz yükler ve yönetir.
* **JavaScript DOM Injection:** `EvaluateScriptAsync` üzerinden form elemanlarına güvenli veri girişi ve tetikleme işlemleri uygular.
* **Asenkron Eleman Bekleme (Polling):** Sayfa yönlendirmelerinde ve dinamik DOM güncellemelerinde hedef HTML elemanlarını periyodik olarak sorgulayan asenkron mekanizma içerir.
* **Bellek & Çökme Koruması:** Yenilenen frame süreçlerinde ortaya çıkan `ObjectDisposedException` hatalarını yakalayan ve akışın kesilmesini önleyen hata kontrol mimarisine sahiptir.
* **Timeout Güvenlik Mekanizması:** Elemanların yüklenememesi durumunda sonsuz döngüye girmeyi engelleyen zaman aşımı kontrolü barındırır.

---

## 🛠️ Kullanılan Teknolojiler

* **Programlama Dili:** C# (.NET)
* **Arayüz:** Windows Forms (WinForms)
* **Web Tarayıcı Motoru:** CefSharp.WinForms (Chromium Embedded Framework)
* **Otomasyon & Scripting:** JavaScript DOM Manipulation & Polling

---

## 🚀 Kurulum ve Çalıştırma

1. Repoyu bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/Batuhan-inan/Yolcu360-Arac-Otomasyonu.git](https://github.com/Batuhan-inan/Yolcu360-Arac-Otomasyonu.git)

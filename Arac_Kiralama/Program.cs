using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (giris_ekrani girisForm = new giris_ekrani())
            {
                // Eğer giriş başarılı olursa DialogResult.OK dönecek
                if (girisForm.ShowDialog() == DialogResult.OK)
                {
                    // 2. Giriş başarılı! Giriş yapan kullanıcı ile Anaform'u başlatıyoruz 🚀
                    Application.Run(new Anaform(girisForm.GirisYapanKullanici));
                }
            }
        }
    }
}

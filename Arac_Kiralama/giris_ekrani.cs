using Arac_Kiralama.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    public partial class giris_ekrani : Form
    {
        public giris_ekrani()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btngiris_Click(object sender, EventArgs e)
        {
            string eposta=tbEposta.Text.Trim();
            string sifre = tbSifre.Text.Trim();

            // Giriş alanlarının boş olup olmadığını kontrol ediyoruz 
            if (string.IsNullOrEmpty(eposta) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen e-posta ve şifrenizi giriniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcı adı ve şifreyi kontrol ediyoruz
            string sql = "SELECT * FROM kullanicilar WHERE eposta=@eposta AND sifre=@sifre";

            var kullanici = DbHelper.QueryFirstOrDefault <kullanici>(sql, new { Eposta = eposta, Sifre = sifre });

            if (kullanici != null)
            {
               

                // Ana form ekranını açıyoruz 🚀
                Anaform anaForm = new Anaform();
                

                // AnaForm kapandığında uygulamanın da tamamen kapanmasını sağlıyoruz 🛑
                anaForm.FormClosed += (s, args) => Application.Exit();

                // LoginForm'u tamamen kapatıyoruz 🚪
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

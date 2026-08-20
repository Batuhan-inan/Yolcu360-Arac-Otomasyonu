namespace Arac_Kiralama
{
    partial class ProfilForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ProfilForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Name = "ProfilForm";
            this.Text = "Kullanıcı Profili & Yönetim Paneli";
            this.Load += new System.EventHandler(this.ProfilForm_Load);
            this.ResumeLayout(false);
        }
    }
}

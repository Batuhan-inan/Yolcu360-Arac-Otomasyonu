namespace Arac_Kiralama
{
    partial class OdemeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelKart = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAracModeli = new System.Windows.Forms.Label();
            this.lblKiralamaSirketi = new System.Windows.Forms.Label();
            this.lblOzellikler = new System.Windows.Forms.Label();
            this.lblFiyat = new System.Windows.Forms.Label();
            this.lblKartSahibi = new System.Windows.Forms.Label();
            this.tbKartSahibi = new System.Windows.Forms.TextBox();
            this.lblKartNo = new System.Windows.Forms.Label();
            this.tbKartNumarasi = new System.Windows.Forms.TextBox();
            this.lblSKT = new System.Windows.Forms.Label();
            this.cbSKAy = new System.Windows.Forms.ComboBox();
            this.cbSKYil = new System.Windows.Forms.ComboBox();
            this.lblCvc = new System.Windows.Forms.Label();
            this.tbCvc = new System.Windows.Forms.TextBox();
            this.btnTestKarti = new System.Windows.Forms.Button();
            this.btnOdemeYap = new System.Windows.Forms.Button();
            this.panelKart.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelKart
            // 
            this.panelKart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.panelKart.Controls.Add(this.lblFiyat);
            this.panelKart.Controls.Add(this.lblOzellikler);
            this.panelKart.Controls.Add(this.lblKiralamaSirketi);
            this.panelKart.Controls.Add(this.lblAracModeli);
            this.panelKart.Controls.Add(this.lblTitle);
            this.panelKart.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelKart.Location = new System.Drawing.Point(0, 0);
            this.panelKart.Name = "panelKart";
            this.panelKart.Size = new System.Drawing.Size(464, 110);
            this.panelKart.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(209, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "💳 iyzico Sandbox Ödeme";
            // 
            // lblAracModeli
            // 
            this.lblAracModeli.AutoSize = true;
            this.lblAracModeli.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAracModeli.ForeColor = System.Drawing.Color.White;
            this.lblAracModeli.Location = new System.Drawing.Point(12, 34);
            this.lblAracModeli.Name = "lblAracModeli";
            this.lblAracModeli.Size = new System.Drawing.Size(121, 25);
            this.lblAracModeli.TabIndex = 1;
            this.lblAracModeli.Text = "Araç Modeli";
            // 
            // lblKiralamaSirketi
            // 
            this.lblKiralamaSirketi.AutoSize = true;
            this.lblKiralamaSirketi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKiralamaSirketi.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblKiralamaSirketi.Location = new System.Drawing.Point(14, 62);
            this.lblKiralamaSirketi.Name = "lblKiralamaSirketi";
            this.lblKiralamaSirketi.Size = new System.Drawing.Size(89, 15);
            this.lblKiralamaSirketi.TabIndex = 2;
            this.lblKiralamaSirketi.Text = "Kiralama Şirketi";
            // 
            // lblOzellikler
            // 
            this.lblOzellikler.AutoSize = true;
            this.lblOzellikler.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblOzellikler.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblOzellikler.Location = new System.Drawing.Point(14, 82);
            this.lblOzellikler.Name = "lblOzellikler";
            this.lblOzellikler.Size = new System.Drawing.Size(95, 13);
            this.lblOzellikler.TabIndex = 3;
            this.lblOzellikler.Text = "Vites / Yakıt Detayı";
            // 
            // lblFiyat
            // 
            this.lblFiyat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFiyat.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblFiyat.ForeColor = System.Drawing.Color.Yellow;
            this.lblFiyat.Location = new System.Drawing.Point(264, 34);
            this.lblFiyat.Name = "lblFiyat";
            this.lblFiyat.Size = new System.Drawing.Size(188, 30);
            this.lblFiyat.TabIndex = 4;
            this.lblFiyat.Text = "00 TL";
            this.lblFiyat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblKartSahibi
            // 
            this.lblKartSahibi.AutoSize = true;
            this.lblKartSahibi.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKartSahibi.Location = new System.Drawing.Point(25, 130);
            this.lblKartSahibi.Name = "lblKartSahibi";
            this.lblKartSahibi.Size = new System.Drawing.Size(149, 17);
            this.lblKartSahibi.TabIndex = 1;
            this.lblKartSahibi.Text = "Kart Üzerindeki İsim:";
            // 
            // tbKartSahibi
            // 
            this.tbKartSahibi.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tbKartSahibi.Location = new System.Drawing.Point(28, 150);
            this.tbKartSahibi.Name = "tbKartSahibi";
            this.tbKartSahibi.Size = new System.Drawing.Size(408, 27);
            this.tbKartSahibi.TabIndex = 2;
            // 
            // lblKartNo
            // 
            this.lblKartNo.AutoSize = true;
            this.lblKartNo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKartNo.Location = new System.Drawing.Point(25, 190);
            this.lblKartNo.Name = "lblKartNo";
            this.lblKartNo.Size = new System.Drawing.Size(104, 17);
            this.lblKartNo.TabIndex = 3;
            this.lblKartNo.Text = "Kart Numarası:";
            // 
            // tbKartNumarasi
            // 
            this.tbKartNumarasi.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tbKartNumarasi.Location = new System.Drawing.Point(28, 210);
            this.tbKartNumarasi.MaxLength = 19;
            this.tbKartNumarasi.Name = "tbKartNumarasi";
            this.tbKartNumarasi.Size = new System.Drawing.Size(408, 27);
            this.tbKartNumarasi.TabIndex = 4;
            // 
            // lblSKT
            // 
            this.lblSKT.AutoSize = true;
            this.lblSKT.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblSKT.Location = new System.Drawing.Point(25, 250);
            this.lblSKT.Name = "lblSKT";
            this.lblSKT.Size = new System.Drawing.Size(142, 17);
            this.lblSKT.TabIndex = 5;
            this.lblSKT.Text = "Son Kullanma Tarihi:";
            // 
            // cbSKAy
            // 
            this.cbSKAy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSKAy.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbSKAy.FormattingEnabled = true;
            this.cbSKAy.Location = new System.Drawing.Point(28, 270);
            this.cbSKAy.Name = "cbSKAy";
            this.cbSKAy.Size = new System.Drawing.Size(70, 25);
            this.cbSKAy.TabIndex = 6;
            // 
            // cbSKYil
            // 
            this.cbSKYil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSKYil.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbSKYil.FormattingEnabled = true;
            this.cbSKYil.Location = new System.Drawing.Point(104, 270);
            this.cbSKYil.Name = "cbSKYil";
            this.cbSKYil.Size = new System.Drawing.Size(90, 25);
            this.cbSKYil.TabIndex = 7;
            // 
            // lblCvc
            // 
            this.lblCvc.AutoSize = true;
            this.lblCvc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblCvc.Location = new System.Drawing.Point(250, 250);
            this.lblCvc.Name = "lblCvc";
            this.lblCvc.Size = new System.Drawing.Size(77, 17);
            this.lblCvc.TabIndex = 8;
            this.lblCvc.Text = "CVC / CVV:";
            // 
            // tbCvc
            // 
            this.tbCvc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tbCvc.Location = new System.Drawing.Point(253, 270);
            this.tbCvc.MaxLength = 4;
            this.tbCvc.Name = "tbCvc";
            this.tbCvc.Size = new System.Drawing.Size(183, 25);
            this.tbCvc.TabIndex = 9;
            // 
            // btnTestKarti
            // 
            this.btnTestKarti.BackColor = System.Drawing.Color.LightYellow;
            this.btnTestKarti.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestKarti.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnTestKarti.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btnTestKarti.Location = new System.Drawing.Point(28, 312);
            this.btnTestKarti.Name = "btnTestKarti";
            this.btnTestKarti.Size = new System.Drawing.Size(408, 30);
            this.btnTestKarti.TabIndex = 10;
            this.btnTestKarti.Text = "⚡ İyzico Sandbox Test Kartı Doldur (5528...)";
            this.btnTestKarti.UseVisualStyleBackColor = false;
            this.btnTestKarti.Click += new System.EventHandler(this.btnTestKarti_Click);
            // 
            // btnOdemeYap
            // 
            this.btnOdemeYap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnOdemeYap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOdemeYap.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnOdemeYap.ForeColor = System.Drawing.Color.White;
            this.btnOdemeYap.Location = new System.Drawing.Point(28, 352);
            this.btnOdemeYap.Name = "btnOdemeYap";
            this.btnOdemeYap.Size = new System.Drawing.Size(408, 48);
            this.btnOdemeYap.TabIndex = 11;
            this.btnOdemeYap.Text = "💳 Ödemeyi Onayla ve Kirala";
            this.btnOdemeYap.UseVisualStyleBackColor = false;
            this.btnOdemeYap.Click += new System.EventHandler(this.btnOdemeYap_Click);
            // 
            // OdemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(464, 421);
            this.Controls.Add(this.btnOdemeYap);
            this.Controls.Add(this.btnTestKarti);
            this.Controls.Add(this.tbCvc);
            this.Controls.Add(this.lblCvc);
            this.Controls.Add(this.cbSKYil);
            this.Controls.Add(this.cbSKAy);
            this.Controls.Add(this.lblSKT);
            this.Controls.Add(this.tbKartNumarasi);
            this.Controls.Add(this.lblKartNo);
            this.Controls.Add(this.tbKartSahibi);
            this.Controls.Add(this.lblKartSahibi);
            this.Controls.Add(this.panelKart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OdemeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "iyzico Sandbox Güvenli Ödeme Ekranı";
            this.Load += new System.EventHandler(this.OdemeForm_Load);
            this.panelKart.ResumeLayout(false);
            this.panelKart.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelKart;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblAracModeli;
        private System.Windows.Forms.Label lblKiralamaSirketi;
        private System.Windows.Forms.Label lblOzellikler;
        private System.Windows.Forms.Label lblFiyat;
        private System.Windows.Forms.Label lblKartSahibi;
        private System.Windows.Forms.TextBox tbKartSahibi;
        private System.Windows.Forms.Label lblKartNo;
        private System.Windows.Forms.TextBox tbKartNumarasi;
        private System.Windows.Forms.Label lblSKT;
        private System.Windows.Forms.ComboBox cbSKAy;
        private System.Windows.Forms.ComboBox cbSKYil;
        private System.Windows.Forms.Label lblCvc;
        private System.Windows.Forms.TextBox tbCvc;
        private System.Windows.Forms.Button btnTestKarti;
        private System.Windows.Forms.Button btnOdemeYap;
    }
}

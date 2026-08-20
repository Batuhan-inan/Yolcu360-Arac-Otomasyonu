namespace Arac_Kiralama
{
    partial class Anaform
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
            this.dtpAlisTarihi = new System.Windows.Forms.DateTimePicker();
            this.cbYakit = new System.Windows.Forms.ComboBox();
            this.cbVites = new System.Windows.Forms.ComboBox();
            this.tbAlısyeri = new System.Windows.Forms.TextBox();
            this.dtpVerisTarihi = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbAlısSaati = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbDonusSaati = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dgbVeriEkranı = new System.Windows.Forms.DataGridView();
            this.btnAra = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.Panel();
            this.btnKoleksiyonKaydet = new System.Windows.Forms.Button();
            this.tbKoleksiyonAdi = new System.Windows.Forms.TextBox();
            this.btnGecmisKoleksiyonlar = new System.Windows.Forms.Button();
            this.btnGecmisOdemeler = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgbVeriEkranı)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpAlisTarihi
            // 
            this.dtpAlisTarihi.Location = new System.Drawing.Point(313, 39);
            this.dtpAlisTarihi.Name = "dtpAlisTarihi";
            this.dtpAlisTarihi.Size = new System.Drawing.Size(200, 20);
            this.dtpAlisTarihi.TabIndex = 0;
            this.dtpAlisTarihi.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // cbYakit
            // 
            this.cbYakit.FormattingEnabled = true;
            this.cbYakit.Location = new System.Drawing.Point(117, 165);
            this.cbYakit.Name = "cbYakit";
            this.cbYakit.Size = new System.Drawing.Size(121, 21);
            this.cbYakit.TabIndex = 1;
            // 
            // cbVites
            // 
            this.cbVites.FormattingEnabled = true;
            this.cbVites.Location = new System.Drawing.Point(349, 165);
            this.cbVites.Name = "cbVites";
            this.cbVites.Size = new System.Drawing.Size(121, 21);
            this.cbVites.TabIndex = 2;
            // 
            // tbAlısyeri
            // 
            this.tbAlısyeri.Location = new System.Drawing.Point(138, 36);
            this.tbAlısyeri.Name = "tbAlısyeri";
            this.tbAlısyeri.Size = new System.Drawing.Size(100, 20);
            this.tbAlısyeri.TabIndex = 3;
            // 
            // dtpVerisTarihi
            // 
            this.dtpVerisTarihi.Location = new System.Drawing.Point(313, 104);
            this.dtpVerisTarihi.Name = "dtpVerisTarihi";
            this.dtpVerisTarihi.Size = new System.Drawing.Size(200, 20);
            this.dtpVerisTarihi.TabIndex = 4;
            this.dtpVerisTarihi.ValueChanged += new System.EventHandler(this.dtpVerisTarihi_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Aracı almak istediğin yer";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Alış Tarihi";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(539, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Alış Saati";
            // 
            // cbAlısSaati
            // 
            this.cbAlısSaati.FormattingEnabled = true;
            this.cbAlısSaati.Location = new System.Drawing.Point(613, 42);
            this.cbAlısSaati.Name = "cbAlısSaati";
            this.cbAlısSaati.Size = new System.Drawing.Size(121, 21);
            this.cbAlısSaati.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(242, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Dönüş Tarhi";
            // 
            // cbDonusSaati
            // 
            this.cbDonusSaati.FormattingEnabled = true;
            this.cbDonusSaati.Location = new System.Drawing.Point(613, 108);
            this.cbDonusSaati.Name = "cbDonusSaati";
            this.cbDonusSaati.Size = new System.Drawing.Size(121, 21);
            this.cbDonusSaati.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(539, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Dönüş Saati";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Yakıt Tipi";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(272, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Vites Tipi";
            // 
            // dgbVeriEkranı
            // 
            this.dgbVeriEkranı.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgbVeriEkranı.Location = new System.Drawing.Point(15, 223);
            this.dgbVeriEkranı.Name = "dgbVeriEkranı";
            this.dgbVeriEkranı.Size = new System.Drawing.Size(680, 559);
            this.dgbVeriEkranı.TabIndex = 14;
            // 
            // btnAra
            // 
            this.btnAra.Location = new System.Drawing.Point(853, 54);
            this.btnAra.Name = "btnAra";
            this.btnAra.Size = new System.Drawing.Size(141, 51);
            this.btnAra.TabIndex = 15;
            this.btnAra.Text = "Ara";
            this.btnAra.UseVisualStyleBackColor = true;
            this.btnAra.Click += new System.EventHandler(this.btnAra_Click);
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(701, 223);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(947, 559);
            this.panel.TabIndex = 16;
            // 
            // btnKoleksiyonKaydet
            // 
            this.btnKoleksiyonKaydet.Location = new System.Drawing.Point(1364, 153);
            this.btnKoleksiyonKaydet.Name = "btnKoleksiyonKaydet";
            this.btnKoleksiyonKaydet.Size = new System.Drawing.Size(120, 53);
            this.btnKoleksiyonKaydet.TabIndex = 17;
            this.btnKoleksiyonKaydet.Text = "Koleksiyon Kaydet";
            this.btnKoleksiyonKaydet.UseVisualStyleBackColor = true;
            this.btnKoleksiyonKaydet.Click += new System.EventHandler(this.btnKoleksiyonKaydet_Click);
            // 
            // tbKoleksiyonAdi
            // 
            this.tbKoleksiyonAdi.Location = new System.Drawing.Point(1151, 166);
            this.tbKoleksiyonAdi.Name = "tbKoleksiyonAdi";
            this.tbKoleksiyonAdi.Size = new System.Drawing.Size(145, 20);
            this.tbKoleksiyonAdi.TabIndex = 18;
            // 
            // btnGecmisKoleksiyonlar
            // 
            this.btnGecmisKoleksiyonlar.Location = new System.Drawing.Point(1509, 29);
            this.btnGecmisKoleksiyonlar.Name = "btnGecmisKoleksiyonlar";
            this.btnGecmisKoleksiyonlar.Size = new System.Drawing.Size(130, 34);
            this.btnGecmisKoleksiyonlar.TabIndex = 19;
            this.btnGecmisKoleksiyonlar.Text = "Geçmiş Koleksiyonlar";
            this.btnGecmisKoleksiyonlar.UseVisualStyleBackColor = true;
            this.btnGecmisKoleksiyonlar.Click += new System.EventHandler(this.btnGecmisKoleksiyonlar_Click);
            // 
            // btnGecmisOdemeler
            // 
            this.btnGecmisOdemeler.Location = new System.Drawing.Point(1357, 33);
            this.btnGecmisOdemeler.Name = "btnGecmisOdemeler";
            this.btnGecmisOdemeler.Size = new System.Drawing.Size(127, 30);
            this.btnGecmisOdemeler.TabIndex = 20;
            this.btnGecmisOdemeler.Text = "Geçmiş Ödemeler";
            this.btnGecmisOdemeler.UseVisualStyleBackColor = true;
            this.btnGecmisOdemeler.Click += new System.EventHandler(this.btnGecmisOdemeler_Click);
            // 
            // Anaform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1660, 794);
            this.Controls.Add(this.btnGecmisOdemeler);
            this.Controls.Add(this.btnGecmisKoleksiyonlar);
            this.Controls.Add(this.tbKoleksiyonAdi);
            this.Controls.Add(this.btnKoleksiyonKaydet);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnAra);
            this.Controls.Add(this.dgbVeriEkranı);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbDonusSaati);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbAlısSaati);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpVerisTarihi);
            this.Controls.Add(this.tbAlısyeri);
            this.Controls.Add(this.cbVites);
            this.Controls.Add(this.cbYakit);
            this.Controls.Add(this.dtpAlisTarihi);
            this.Name = "Anaform";
            this.Text = "AnaForm";
            this.Load += new System.EventHandler(this.Anaform_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgbVeriEkranı)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpAlisTarihi;
        private System.Windows.Forms.ComboBox cbYakit;
        private System.Windows.Forms.ComboBox cbVites;
        private System.Windows.Forms.TextBox tbAlısyeri;
        private System.Windows.Forms.DateTimePicker dtpVerisTarihi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbAlısSaati;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbDonusSaati;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dgbVeriEkranı;
        private System.Windows.Forms.Button btnAra;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button btnKoleksiyonKaydet;
        private System.Windows.Forms.TextBox tbKoleksiyonAdi;
        private System.Windows.Forms.Button btnGecmisKoleksiyonlar;
        private System.Windows.Forms.Button btnGecmisOdemeler;
    }
}


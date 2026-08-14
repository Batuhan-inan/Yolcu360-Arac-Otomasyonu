namespace Arac_Kiralama
{
    partial class GecmisIslemlerForm
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
            this.tabControlGecmis = new System.Windows.Forms.TabControl();
            this.tabPageKoleksiyonlar = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvKoleksiyonlar = new System.Windows.Forms.DataGridView();
            this.lblKoleksiyonlar = new System.Windows.Forms.Label();
            this.btnKoleksiyonSil = new System.Windows.Forms.Button();
            this.btnPngKaydet = new System.Windows.Forms.Button();
            this.dgvKoleksiyonDetay = new System.Windows.Forms.DataGridView();
            this.lblKoleksiyonDetay = new System.Windows.Forms.Label();
            this.tabPageOdemeler = new System.Windows.Forms.TabPage();
            this.dgvOdemeler = new System.Windows.Forms.DataGridView();
            this.lblOdemeler = new System.Windows.Forms.Label();
            this.tabControlGecmis.SuspendLayout();
            this.tabPageKoleksiyonlar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKoleksiyonlar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKoleksiyonDetay)).BeginInit();
            this.tabPageOdemeler.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOdemeler)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlGecmis
            // 
            this.tabControlGecmis.Controls.Add(this.tabPageKoleksiyonlar);
            this.tabControlGecmis.Controls.Add(this.tabPageOdemeler);
            this.tabControlGecmis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlGecmis.Location = new System.Drawing.Point(0, 0);
            this.tabControlGecmis.Name = "tabControlGecmis";
            this.tabControlGecmis.SelectedIndex = 0;
            this.tabControlGecmis.Size = new System.Drawing.Size(984, 611);
            this.tabControlGecmis.TabIndex = 0;
            this.tabControlGecmis.SelectedIndexChanged += new System.EventHandler(this.tabControlGecmis_SelectedIndexChanged);
            // 
            // tabPageKoleksiyonlar
            // 
            this.tabPageKoleksiyonlar.Controls.Add(this.splitContainer1);
            this.tabPageKoleksiyonlar.Location = new System.Drawing.Point(4, 22);
            this.tabPageKoleksiyonlar.Name = "tabPageKoleksiyonlar";
            this.tabPageKoleksiyonlar.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageKoleksiyonlar.Size = new System.Drawing.Size(976, 585);
            this.tabPageKoleksiyonlar.TabIndex = 0;
            this.tabPageKoleksiyonlar.Text = "📂 Geçmiş Koleksiyonlar";
            this.tabPageKoleksiyonlar.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvKoleksiyonlar);
            this.splitContainer1.Panel1.Controls.Add(this.lblKoleksiyonlar);
            this.splitContainer1.Panel1.Controls.Add(this.btnKoleksiyonSil);
            this.splitContainer1.Panel1.Controls.Add(this.btnPngKaydet);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvKoleksiyonDetay);
            this.splitContainer1.Panel2.Controls.Add(this.lblKoleksiyonDetay);
            this.splitContainer1.Size = new System.Drawing.Size(970, 579);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.TabIndex = 0;
            // 
            // dgvKoleksiyonlar
            // 
            this.dgvKoleksiyonlar.AllowUserToAddRows = false;
            this.dgvKoleksiyonlar.AllowUserToDeleteRows = false;
            this.dgvKoleksiyonlar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvKoleksiyonlar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKoleksiyonlar.Location = new System.Drawing.Point(5, 30);
            this.dgvKoleksiyonlar.MultiSelect = false;
            this.dgvKoleksiyonlar.Name = "dgvKoleksiyonlar";
            this.dgvKoleksiyonlar.ReadOnly = true;
            this.dgvKoleksiyonlar.RowHeadersVisible = false;
            this.dgvKoleksiyonlar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKoleksiyonlar.Size = new System.Drawing.Size(340, 490);
            this.dgvKoleksiyonlar.TabIndex = 0;
            this.dgvKoleksiyonlar.SelectionChanged += new System.EventHandler(this.dgvKoleksiyonlar_SelectionChanged);
            // 
            // lblKoleksiyonlar
            // 
            this.lblKoleksiyonlar.AutoSize = true;
            this.lblKoleksiyonlar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKoleksiyonlar.Location = new System.Drawing.Point(5, 8);
            this.lblKoleksiyonlar.Name = "lblKoleksiyonlar";
            this.lblKoleksiyonlar.Size = new System.Drawing.Size(183, 16);
            this.lblKoleksiyonlar.TabIndex = 1;
            this.lblKoleksiyonlar.Text = "Kaydedilen Koleksiyonlar:";
            // 
            // btnKoleksiyonSil
            // 
            this.btnKoleksiyonSil.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKoleksiyonSil.BackColor = System.Drawing.Color.MistyRose;
            this.btnKoleksiyonSil.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKoleksiyonSil.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnKoleksiyonSil.ForeColor = System.Drawing.Color.DarkRed;
            this.btnKoleksiyonSil.Location = new System.Drawing.Point(5, 528);
            this.btnKoleksiyonSil.Name = "btnKoleksiyonSil";
            this.btnKoleksiyonSil.Size = new System.Drawing.Size(165, 45);
            this.btnKoleksiyonSil.TabIndex = 2;
            this.btnKoleksiyonSil.Text = "🗑️ Koleksiyonu Sil";
            this.btnKoleksiyonSil.UseVisualStyleBackColor = false;
            this.btnKoleksiyonSil.Click += new System.EventHandler(this.btnKoleksiyonSil_Click);
            // 
            // btnPngKaydet
            // 
            this.btnPngKaydet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPngKaydet.BackColor = System.Drawing.Color.AliceBlue;
            this.btnPngKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPngKaydet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnPngKaydet.ForeColor = System.Drawing.Color.MidnightBlue;
            this.btnPngKaydet.Location = new System.Drawing.Point(175, 528);
            this.btnPngKaydet.Name = "btnPngKaydet";
            this.btnPngKaydet.Size = new System.Drawing.Size(170, 45);
            this.btnPngKaydet.TabIndex = 3;
            this.btnPngKaydet.Text = "🖼️ PNG İndir / Kaydet";
            this.btnPngKaydet.UseVisualStyleBackColor = false;
            this.btnPngKaydet.Click += new System.EventHandler(this.btnPngKaydet_Click);
            // 
            // dgvKoleksiyonDetay
            // 
            this.dgvKoleksiyonDetay.AllowUserToAddRows = false;
            this.dgvKoleksiyonDetay.AllowUserToDeleteRows = false;
            this.dgvKoleksiyonDetay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvKoleksiyonDetay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvKoleksiyonDetay.Location = new System.Drawing.Point(5, 30);
            this.dgvKoleksiyonDetay.Name = "dgvKoleksiyonDetay";
            this.dgvKoleksiyonDetay.ReadOnly = true;
            this.dgvKoleksiyonDetay.RowHeadersVisible = false;
            this.dgvKoleksiyonDetay.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvKoleksiyonDetay.Size = new System.Drawing.Size(605, 543);
            this.dgvKoleksiyonDetay.TabIndex = 0;
            this.dgvKoleksiyonDetay.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvKoleksiyonDetay_CellDoubleClick);
            // 
            // lblKoleksiyonDetay
            // 
            this.lblKoleksiyonDetay.AutoSize = true;
            this.lblKoleksiyonDetay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblKoleksiyonDetay.Location = new System.Drawing.Point(5, 8);
            this.lblKoleksiyonDetay.Name = "lblKoleksiyonDetay";
            this.lblKoleksiyonDetay.Size = new System.Drawing.Size(193, 16);
            this.lblKoleksiyonDetay.TabIndex = 1;
            this.lblKoleksiyonDetay.Text = "Koleksiyon İçeriği (Araçlar):";
            // 
            // tabPageOdemeler
            // 
            this.tabPageOdemeler.Controls.Add(this.dgvOdemeler);
            this.tabPageOdemeler.Controls.Add(this.lblOdemeler);
            this.tabPageOdemeler.Location = new System.Drawing.Point(4, 22);
            this.tabPageOdemeler.Name = "tabPageOdemeler";
            this.tabPageOdemeler.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOdemeler.Size = new System.Drawing.Size(976, 585);
            this.tabPageOdemeler.TabIndex = 1;
            this.tabPageOdemeler.Text = "💳 Geçmiş Ödemeler";
            this.tabPageOdemeler.UseVisualStyleBackColor = true;
            // 
            // dgvOdemeler
            // 
            this.dgvOdemeler.AllowUserToAddRows = false;
            this.dgvOdemeler.AllowUserToDeleteRows = false;
            this.dgvOdemeler.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOdemeler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOdemeler.Location = new System.Drawing.Point(8, 32);
            this.dgvOdemeler.Name = "dgvOdemeler";
            this.dgvOdemeler.ReadOnly = true;
            this.dgvOdemeler.RowHeadersVisible = false;
            this.dgvOdemeler.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOdemeler.Size = new System.Drawing.Size(960, 545);
            this.dgvOdemeler.TabIndex = 0;
            // 
            // lblOdemeler
            // 
            this.lblOdemeler.AutoSize = true;
            this.lblOdemeler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblOdemeler.Location = new System.Drawing.Point(8, 10);
            this.lblOdemeler.Name = "lblOdemeler";
            this.lblOdemeler.Size = new System.Drawing.Size(176, 16);
            this.lblOdemeler.TabIndex = 1;
            this.lblOdemeler.Text = "Tamamlanan Ödemeler:";
            // 
            // GecmisIslemlerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this.tabControlGecmis);
            this.Name = "GecmisIslemlerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Geçmiş İşlemler & Koleksiyonlar";
            this.Load += new System.EventHandler(this.GecmisIslemlerForm_Load);
            this.tabControlGecmis.ResumeLayout(false);
            this.tabPageKoleksiyonlar.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKoleksiyonlar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKoleksiyonDetay)).EndInit();
            this.tabPageOdemeler.ResumeLayout(false);
            this.tabPageOdemeler.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOdemeler)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlGecmis;
        private System.Windows.Forms.TabPage tabPageKoleksiyonlar;
        private System.Windows.Forms.TabPage tabPageOdemeler;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvKoleksiyonlar;
        private System.Windows.Forms.Label lblKoleksiyonlar;
        private System.Windows.Forms.Button btnKoleksiyonSil;
        private System.Windows.Forms.Button btnPngKaydet;
        private System.Windows.Forms.DataGridView dgvKoleksiyonDetay;
        private System.Windows.Forms.Label lblKoleksiyonDetay;
        private System.Windows.Forms.DataGridView dgvOdemeler;
        private System.Windows.Forms.Label lblOdemeler;
    }
}

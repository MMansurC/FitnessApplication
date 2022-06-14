
namespace FitnessApplication
{
    partial class Giris
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Giris));
            this.myBorder = new System.Windows.Forms.Panel();
            this.VersiyonPaneli = new System.Windows.Forms.Panel();
            this.lblVersiyon = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.GirisPaneli = new System.Windows.Forms.Panel();
            this.lblBekle = new System.Windows.Forms.Label();
            this.SifreBox = new System.Windows.Forms.TextBox();
            this.KullaniciAdiBox = new System.Windows.Forms.TextBox();
            this.btnGiris = new System.Windows.Forms.Button();
            this.lblSifre = new System.Windows.Forms.Label();
            this.lblKullaniciAdi = new System.Windows.Forms.Label();
            this.bekletici = new System.Windows.Forms.Timer(this.components);
            this.lblAciklama = new System.Windows.Forms.Label();
            this.myBorder.SuspendLayout();
            this.VersiyonPaneli.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.GirisPaneli.SuspendLayout();
            this.SuspendLayout();
            // 
            // myBorder
            // 
            this.myBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.myBorder.Controls.Add(this.VersiyonPaneli);
            this.myBorder.Controls.Add(this.pictureBox1);
            this.myBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.myBorder.Location = new System.Drawing.Point(0, 0);
            this.myBorder.Name = "myBorder";
            this.myBorder.Size = new System.Drawing.Size(800, 35);
            this.myBorder.TabIndex = 1;
            // 
            // VersiyonPaneli
            // 
            this.VersiyonPaneli.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(104)))), ((int)(((byte)(110)))));
            this.VersiyonPaneli.Controls.Add(this.lblVersiyon);
            this.VersiyonPaneli.ForeColor = System.Drawing.SystemColors.ControlText;
            this.VersiyonPaneli.Location = new System.Drawing.Point(0, 0);
            this.VersiyonPaneli.Name = "VersiyonPaneli";
            this.VersiyonPaneli.Size = new System.Drawing.Size(270, 35);
            this.VersiyonPaneli.TabIndex = 3;
            // 
            // lblVersiyon
            // 
            this.lblVersiyon.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblVersiyon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblVersiyon.Location = new System.Drawing.Point(12, 0);
            this.lblVersiyon.Name = "lblVersiyon";
            this.lblVersiyon.Size = new System.Drawing.Size(130, 35);
            this.lblVersiyon.TabIndex = 5;
            this.lblVersiyon.Text = "Versiyon";
            this.lblVersiyon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(767, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // GirisPaneli
            // 
            this.GirisPaneli.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.GirisPaneli.Controls.Add(this.lblBekle);
            this.GirisPaneli.Controls.Add(this.SifreBox);
            this.GirisPaneli.Controls.Add(this.KullaniciAdiBox);
            this.GirisPaneli.Controls.Add(this.btnGiris);
            this.GirisPaneli.Controls.Add(this.lblSifre);
            this.GirisPaneli.Controls.Add(this.lblKullaniciAdi);
            this.GirisPaneli.Dock = System.Windows.Forms.DockStyle.Left;
            this.GirisPaneli.Location = new System.Drawing.Point(0, 35);
            this.GirisPaneli.Name = "GirisPaneli";
            this.GirisPaneli.Size = new System.Drawing.Size(270, 415);
            this.GirisPaneli.TabIndex = 2;
            // 
            // lblBekle
            // 
            this.lblBekle.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBekle.ForeColor = System.Drawing.Color.White;
            this.lblBekle.Location = new System.Drawing.Point(70, 220);
            this.lblBekle.Name = "lblBekle";
            this.lblBekle.Size = new System.Drawing.Size(130, 50);
            this.lblBekle.TabIndex = 4;
            this.lblBekle.Text = "Lütfen Bekleyin...";
            this.lblBekle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblBekle.Visible = false;
            // 
            // SifreBox
            // 
            this.SifreBox.Location = new System.Drawing.Point(118, 150);
            this.SifreBox.MaxLength = 15;
            this.SifreBox.Name = "SifreBox";
            this.SifreBox.PasswordChar = '*';
            this.SifreBox.Size = new System.Drawing.Size(129, 27);
            this.SifreBox.TabIndex = 2;
            this.SifreBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SifreBox_KeyPress);
            // 
            // KullaniciAdiBox
            // 
            this.KullaniciAdiBox.Location = new System.Drawing.Point(118, 117);
            this.KullaniciAdiBox.MaxLength = 15;
            this.KullaniciAdiBox.Name = "KullaniciAdiBox";
            this.KullaniciAdiBox.Size = new System.Drawing.Size(129, 27);
            this.KullaniciAdiBox.TabIndex = 1;
            this.KullaniciAdiBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KullaniciAdiBox_KeyPress);
            // 
            // btnGiris
            // 
            this.btnGiris.BackColor = System.Drawing.Color.Silver;
            this.btnGiris.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGiris.Location = new System.Drawing.Point(70, 220);
            this.btnGiris.Name = "btnGiris";
            this.btnGiris.Size = new System.Drawing.Size(130, 50);
            this.btnGiris.TabIndex = 3;
            this.btnGiris.Text = "Giriş Yap";
            this.btnGiris.UseVisualStyleBackColor = false;
            this.btnGiris.Click += new System.EventHandler(this.btnGiris_Click);
            // 
            // lblSifre
            // 
            this.lblSifre.ForeColor = System.Drawing.Color.White;
            this.lblSifre.Location = new System.Drawing.Point(12, 153);
            this.lblSifre.Name = "lblSifre";
            this.lblSifre.Size = new System.Drawing.Size(100, 19);
            this.lblSifre.TabIndex = 0;
            this.lblSifre.Text = "Şifre:";
            this.lblSifre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblKullaniciAdi
            // 
            this.lblKullaniciAdi.ForeColor = System.Drawing.Color.White;
            this.lblKullaniciAdi.Location = new System.Drawing.Point(12, 120);
            this.lblKullaniciAdi.Name = "lblKullaniciAdi";
            this.lblKullaniciAdi.Size = new System.Drawing.Size(100, 19);
            this.lblKullaniciAdi.TabIndex = 0;
            this.lblKullaniciAdi.Text = "Kullanıcı Adı:";
            this.lblKullaniciAdi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bekletici
            // 
            this.bekletici.Interval = 10;
            this.bekletici.Tick += new System.EventHandler(this.bekletici_Tick);
            // 
            // lblAciklama
            // 
            this.lblAciklama.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAciklama.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblAciklama.Location = new System.Drawing.Point(282, 41);
            this.lblAciklama.Name = "lblAciklama";
            this.lblAciklama.Size = new System.Drawing.Size(506, 120);
            this.lblAciklama.TabIndex = 6;
            this.lblAciklama.Text = "AÇIKLAMA";
            this.lblAciklama.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Giris
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblAciklama);
            this.Controls.Add(this.GirisPaneli);
            this.Controls.Add(this.myBorder);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Giris";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Giris";
            this.Load += new System.EventHandler(this.Giris_Load);
            this.myBorder.ResumeLayout(false);
            this.VersiyonPaneli.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.GirisPaneli.ResumeLayout(false);
            this.GirisPaneli.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel myBorder;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel GirisPaneli;
        private System.Windows.Forms.TextBox SifreBox;
        private System.Windows.Forms.TextBox KullaniciAdiBox;
        private System.Windows.Forms.Button btnGiris;
        private System.Windows.Forms.Label lblSifre;
        private System.Windows.Forms.Label lblKullaniciAdi;
        private System.Windows.Forms.Panel VersiyonPaneli;
        private System.Windows.Forms.Label lblBekle;
        private System.Windows.Forms.Timer bekletici;
        public System.Windows.Forms.Label lblVersiyon;
        public System.Windows.Forms.Label lblAciklama;
    }
}


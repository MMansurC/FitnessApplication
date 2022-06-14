using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace FitnessApplication
{
    public partial class PersonelAnaSayfa : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public PersonelAnaSayfa()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        TimeSpan sifiribul = DateTime.Today - DateTime.Today;

        public bool sahipmi = false, sifremi = false, ucsaatuyarmi = false, akaktiflik = false, herkesidndr = false;
        public int girisYapanınSeriNumarası, silSuresi;
        public string SahipAdi, Acilis, Kapanis;

        public DataTable TumMustDT = new DataTable();
        public DataTable SalonDT = new DataTable();
        public DataTable PersDT = new DataTable();
        public DataTable GecmisDT = new DataTable();

        /*baglanti.Open();
for(sayii = 16; sayii < 81; sayii++)
{
    NpgsqlCommand dugme = new NpgsqlCommand("INSERT INTO dolaplar (dolap) VALUES ('dolap" + Convert.ToString(sayii) + "')", baglanti);
    dugme.ExecuteNonQuery();
}
baglanti.Close();
MessageBox.Show("Başarılı");*/

        private void PersonelAnaSayfa_Load(object sender, EventArgs e)
        {
            if (ucsaatuyarmi == true)
            {
                dakikabasi.Start();
            }
            tumUyelerTabloDoldur();
            salonTabloDoldur();
            gecmisTabloDoldur();
            if (girisYapanınSeriNumarası != 1)
            {
                (dgwGecmis.DataSource as DataTable).DefaultView.RowFilter = String.Format("[İşlem] NOT like '%PERSONEL GÜNCELLE%'");
            }
            lblTarih.Text = Convert.ToString(DateTime.Now);
            SaatTakip.Start();
            YUCinsiyetBox.SelectedIndex = 0;
            if(sahipmi == true)
            {
                btnPersonelListele.Visible = btnGecmisIslemler.Visible = btnDigerIslemler.Visible = true;
                personelTabloDoldur();
            }
            PanelleriDuzenle();
            UcretHesapla();
            DolapOlustur();
        }

        NpgsqlDataAdapter da;
        NpgsqlCommandBuilder comb;
        DataTable tumUyelerTabloDoldur()
        {
            TumMustDT.Clear();
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            baglanti.Open();
            NpgsqlCommand TumUyelerTablo = new NpgsqlCommand("SELECT adsoyad,tckimlik,cinsiyet,telefon,ogrenci,gunluk,seanslik FROM musteri order by serino", baglanti);
            da = new NpgsqlDataAdapter(TumUyelerTablo);
            da.Fill(TumMustDT);
            TumMustDT.Columns[0].ColumnName = "Ad Soyad";
            TumMustDT.Columns[1].ColumnName = "T.C Kimlik";
            TumMustDT.Columns[2].ColumnName = "Cinsiyet";
            TumMustDT.Columns[3].ColumnName = "Telefon";
            TumMustDT.Columns[4].ColumnName = "Öğrenci";
            TumMustDT.Columns[5].ColumnName = "Günlük";
            TumMustDT.Columns[6].ColumnName = "Seanslık";
            baglanti.Close();
            dgwTumUyeler.DataSource = null;
            dgwTumUyeler.DataSource = TumMustDT;
            dgwTumUyeler.Columns[0].Width = 130;
            dgwTumUyeler.Columns[1].Width = 90;
            dgwTumUyeler.Columns[2].Width = 50;
            dgwTumUyeler.Columns[3].Width = 80;
            dgwTumUyeler.Columns[4].Width = 60;
            dgwTumUyeler.Columns[5].Width = 40;
            dgwTumUyeler.Columns[6].Width = 60;
            for (int iks = 0; iks < dgwTumUyeler.Rows.Count; iks++)
            {
                if (Convert.ToInt32(dgwTumUyeler.Rows[iks].Cells[5].Value) < 0)
                {
                    dgwTumUyeler.Rows[iks].Cells[5].Value = "0";
                }
            }
            return TumMustDT;
        }

        DataTable salonTabloDoldur()
        {
            SalonDT.Clear();
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            baglanti.Open();
            NpgsqlCommand SalondakilerTablo = new NpgsqlCommand("SELECT musteri.adsoyad,musteri.cinsiyet,dolaplar.dolap,dolaplar.tarih FROM musteri INNER JOIN dolaplar ON musteri.serino=dolaplar.sahip WHERE musteri.aktif=true ORDER BY serino", baglanti);
            da = new NpgsqlDataAdapter(SalondakilerTablo);
            baglanti.Close();
            da.Fill(SalonDT);
            SalonDT.Columns[0].ColumnName = "Ad Soyad";
            SalonDT.Columns[1].ColumnName = "Cinsiyet";
            SalonDT.Columns[2].ColumnName = "Dolap Adı";
            SalonDT.Columns[3].ColumnName = "Tarih";
            dgwSalondakiler.DataSource = SalonDT;
            return SalonDT;
        }

        DataTable personelTabloDoldur()
        {
            int PLSahipGeldi = (girisYapanınSeriNumarası == 1) ? 0 : 1;
            PersDT.Clear();
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            baglanti.Open();
            NpgsqlCommand PersoneldekilerTablo = new NpgsqlCommand("SELECT adsoyad FROM personel WHERE personelserino>'" + PLSahipGeldi + "' ORDER BY personelserino", baglanti);
            da = new NpgsqlDataAdapter(PersoneldekilerTablo);
            baglanti.Close();
            da.Fill(PersDT);
            PersDT.Columns[0].ColumnName = "Ad Soyad";
            dgwPersoneller.DataSource = PersDT;
            return PersDT;
        }

        DataTable gecmisTabloDoldur()
        {
            GecmisDT.Clear();
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            baglanti.Open();
            NpgsqlCommand GecmistekilerTablo = new NpgsqlCommand("SELECT mstr,prsnl,islem,tarih,prsnl2 FROM (SELECT * FROM gecmis ORDER BY tarih DESC LIMIT 50) sub ORDER BY tarih DESC", baglanti);
            da = new NpgsqlDataAdapter(GecmistekilerTablo);
            baglanti.Close();
            da.Fill(GecmisDT);
            GecmisDT.Columns[2].ColumnName = "İşlem";
            GecmisDT.Columns[3].ColumnName = "Tarih";
            dgwGecmis.DataSource = GecmisDT;
            dgwGecmis.Columns[0].Visible = dgwGecmis.Columns[1].Visible = dgwGecmis.Columns[4].Visible = false;
            dgwGecmis.Columns[3].Width = 150;
            return GecmisDT;
        }

        void tumTabloGuncelle()
        {
            TumMustDT.Clear();
            TumMustDT.Columns[0].ColumnName = "adsoyad";
            TumMustDT.Columns[1].ColumnName = "tckimlik";
            TumMustDT.Columns[2].ColumnName = "cinsiyet";
            TumMustDT.Columns[3].ColumnName = "telefon";
            TumMustDT.Columns[4].ColumnName = "ogrenci";
            TumMustDT.Columns[5].ColumnName = "gunluk";
            TumMustDT.Columns[6].ColumnName = "seanslik";
            comb = new NpgsqlCommandBuilder(da);
            tumUyelerTabloDoldur();
        }

        void salonTabloGuncelle()
        {
            SalonDT.Clear();
            SalonDT.Columns[0].ColumnName = "adsoyad";
            SalonDT.Columns[1].ColumnName = "cinsiyet";
            SalonDT.Columns[2].ColumnName = "dolap";
            SalonDT.Columns[3].ColumnName = "tarih";
            comb = new NpgsqlCommandBuilder(da);
            salonTabloDoldur();
        }

        void PersonelTabloGuncelle()
        {
            PersDT.Clear();
            PersDT.Columns[0].ColumnName = "adsoyad";
            comb = new NpgsqlCommandBuilder(da);
            personelTabloDoldur();
        }

        void GecmisTabloGuncelle()
        {
            GecmisDT.Clear();
            GecmisDT.Columns[2].ColumnName = "islem";
            GecmisDT.Columns[3].ColumnName = "tarih";
            comb = new NpgsqlCommandBuilder(da);
            gecmisTabloDoldur();
        }

        void UIGetir()
        {
            PanelGizle();
            pnlUyelikIslemleri.Visible = true;
            UIbtnGirisIzni.Visible = (UIAdSoyadBox.Text != "") ? true : false;
            UIbtnGirisIzni.Text = (UIAktif == false) ? "GİRİŞ İZNİ VER" : "ÇIKIŞ İZNİ VER";
            if (UIbtnGirisIzni.Text == "GİRİŞ İZNİ VER" && UIAdSoyadBox.Text != "")
            {
                pnlVarsaDolap.Visible = false;
                pnlGunSeans.Enabled = true;
                pnlDolapSifresi.Visible = (sifremi == true) ? true : false;
            }
            else if (UIbtnGirisIzni.Text == "ÇIKIŞ İZNİ VER" && UIAdSoyadBox.Text != "")
            {
                pnlVarsaDolap.Visible = true;
                gecenSureHesapla();
                pnlDolapSifresi.Visible = false;
                pnlGunSeans.Enabled = false;
            }
            else
            {
                pnlVarsaDolap.Visible = pnlDolapSifresi.Visible = false;
                pnlGunSeans.Enabled = false;
            }
            UIbtnGirisIzni.BackColor = (UIAktif == false) ? Color.FromArgb(80, 150, 80) : Color.FromArgb(150, 80, 80);
            if (UIGunlukBox.Text != "")
            {
                if (Convert.ToInt32(UIGunlukBox.Text) > 0)
                {
                    lblGunSeans.Text = "Gün Ekle";
                }
                else if (Convert.ToInt32(UISeanslikBox.Text) > 0)
                {
                    lblGunSeans.Text = "Seans Ekle";
                }
                else
                {
                    lblGunSeans.Text = "Gün/Seans Ekle";
                }
                chbGun.Visible = chbSeans.Visible = (lblGunSeans.Text == "Gün/Seans Ekle") ? true : false;
            }
            ButonlarEnable();
            btnUyeIslemleri.Enabled = false;
            btnUyeIslemleri.BackColor = Color.FromArgb(200, 200, 200);
            GunSeans();
        }

        void GunSeans()
        {
            if (UIOgrenciBox.Text == "Öğrenci")
            {
                if (Convert.ToInt32(UIGunlukBox.Text) > 0)
                {
                    UcretBox2.Text = UpDown2.Value * 5 + " TL";
                }
                else if (Convert.ToInt32(UISeanslikBox.Text) > 0)
                {
                    UcretBox2.Text = UpDown2.Value * 7 + " TL";
                }
                else
                {
                    UcretBox2.Text = (chbGun.Checked == true) ? UpDown2.Value * 5 + " TL" : UpDown2.Value * 7 + " TL";
                }
            }
            else if (UIOgrenciBox.Text == "Öğrenci Değil")
            {
                if (Convert.ToInt32(UIGunlukBox.Text) > 0)
                {
                    UcretBox2.Text = UpDown2.Value * 6 + " TL";
                }
                else if (Convert.ToInt32(UISeanslikBox.Text) > 0)
                {
                    UcretBox2.Text = UpDown2.Value * 8 + " TL";
                }
                else
                {
                    UcretBox2.Text = (chbGun.Checked == true) ? UpDown2.Value * 6 + " TL" : UpDown2.Value * 8 + " TL";
                }
            }
            pnlUyelikIslemleri.Focus();
        }

        void PanelleriDuzenle()
        {
            pnlYeniUye.Location = pnlUyelikIslemleri.Location = pnlTumUyeler.Location = pnlSalondakiler.Location = pnlDolapRezervasyonu.Location = pnlPersoneller.Location = pnlGecmisIslemler.Location = pnlDigerIslemler.Location = new Point(270, 35);
            pnlYeniUye.Size = pnlUyelikIslemleri.Size = pnlTumUyeler.Size = pnlSalondakiler.Size = pnlDolapRezervasyonu.Size = pnlPersoneller.Size = pnlGecmisIslemler.Size = pnlDigerIslemler.Size = new Size(930, 665);
        }
        void PanelGizle()
        {
            pnlYeniUye.Visible = pnlUyelikIslemleri.Visible = pnlTumUyeler.Visible = pnlDolapRezervasyonu.Visible = pnlSalondakiler.Visible = pnlPersoneller.Visible = pnlGecmisIslemler.Visible = pnlDigerIslemler.Visible = false;
        }

        void ButonlarEnable()
        {
            btnUyeListele.Enabled = btnUyeIslemleri.Enabled = btnDolap.Enabled = btnSalondakiler.Enabled = btnYeniUye.Enabled = btnPersonelListele.Enabled = btnGecmisIslemler.Enabled = btnDigerIslemler.Enabled = true;
            btnUyeListele.BackColor = btnUyeIslemleri.BackColor = btnDolap.BackColor = btnSalondakiler.BackColor = btnYeniUye.BackColor = btnGecmisIslemler.BackColor = Color.FromArgb(50, 50, 50);
            btnPersonelListele.BackColor = btnGecmisIslemler.BackColor = btnDigerIslemler.BackColor = Color.FromArgb(100, 50, 50);
        }

        bool[] checkdolap = new bool[81];
        void DolapOlustur()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");

            int i, j, dolapno = 1, ekasagi = 0, eksol=0;
            for (i = 1; i <= 4; i++)
            {
                for (j = 1; j <= 20; j++)
                {
                    baglanti.Open();
                    Button dolap = new Button();
                    dolap.Font = new Font(dolap.Font.Name, 9.75f);
                    NpgsqlCommand DolapSorgu = new NpgsqlCommand("SELECT tarih FROM dolaplar WHERE dolapserino=" + dolapno + "", baglanti);
                    NpgsqlDataReader dr = DolapSorgu.ExecuteReader();
                    if (dr.Read())
                    {
                        if (Convert.ToString(dr[0]).Length >= 1)
                        {
                            if (-(Convert.ToDateTime(dr[0]) - DateTime.Now).TotalHours <= 3 || ucsaatuyarmi == false)
                            {
                                dolap.BackColor = Color.FromArgb(255, 70, 70);
                                dolap.Text = Convert.ToString(dolapno);
                                checkdolap[dolapno] = true;
                            }
                            else
                            {
                                dolap.BackColor = Color.FromArgb(120, 80, 120);
                                dolap.Text = Convert.ToString(dolapno);
                                checkdolap[dolapno] = true;
                            }
                        }
                        else
                        {
                            dolap.BackColor = Color.FromArgb(150, 255, 150);
                            dolap.Text = Convert.ToString(dolapno);
                        }
                    }
                    if (dolapno == 41)
                    {
                        ekasagi = 60;
                    }
                    if (dolapno == 11 || dolapno == 31 || dolapno == 51|| dolapno==71)
                    {
                        eksol = 50;
                    }
                    if(dolapno==21 || dolapno == 41 || dolapno == 61)
                    {
                        eksol = 0;
                    }
                    dolap.Name = "dolap" + dolapno;
                    dolap.Height = dolap.Width = 40;
                    dolap.Top = 60 + (i * 40)+ekasagi;
                    dolap.Left = (j * 40)+eksol;
                    
                    pnlDolapRezervasyonu.Controls.Add(dolap);
                    dolap.Click += new EventHandler(this.dolap_click);
                    dolap.TabStop = true;
                    dolapno++;
                    baglanti.Close();
                }
            }
            
        }

        double farkdakika;
        void gecenSureHesapla()
        {
            string gecicitarih = "";
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand tarihgetir = new NpgsqlCommand("select tarih,dolap from dolaplar where sahip='" + serinoDR + "'", baglanti);
            if (serinoDR != 0 && checkdolap[serinoDR] == true)
            {
                baglanti.Open();
                NpgsqlDataReader dr = tarihgetir.ExecuteReader();
                if (dr.Read())
                {gecicitarih = Convert.ToString(dr[0]);
                    UIDolapAdiBox.Text = (string)dr[1];
                    double farkdakika = -(Convert.ToDateTime(gecicitarih) - DateTime.Now).TotalMinutes;
                    farkdakika = (farkdakika < 1) ? farkdakika + 1 : farkdakika;
                    DRGecenSureBox.Text = (farkdakika >= 60) ? Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika / 60))) + " saat " + Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika" : Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika";
                }
                else
                {
                    dakikabasi.Stop();
                    MessageBox.Show("Beklenmeik hata #002");
                }
                baglanti.Close();
                
            }
            else if (serino != 0 && pnlVarsaDolap.Visible == true)
            {
                NpgsqlCommand tarihgetir2 = new NpgsqlCommand("select tarih,dolap from dolaplar where sahip='" + serino + "'", baglanti);
                baglanti.Open();
                NpgsqlDataReader dr = tarihgetir2.ExecuteReader();
                if (dr.Read())
                {
                    gecicitarih = Convert.ToString(dr[0]);
                    UIDolapAdiBox.Text = (string)dr[1];
                    farkdakika = -(Convert.ToDateTime(gecicitarih) - DateTime.Now).TotalMinutes;
                    farkdakika = (farkdakika < 1) ? farkdakika + 1 : farkdakika;
                    UIGecenSureBox.Text = (farkdakika >= 60) ? Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika / 60))) + " saat " + Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika" : Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika";
                }
                else
                {
                    dakikabasi.Stop();
                    MessageBox.Show("Beklenmeik hata #003");
                }
                baglanti.Close();
                
            }
        }

        int serinoDR = 0, lastClickedDolap = 0;
        void dolap_click(Object sender, EventArgs e)
        {
            Button mybutton = (Button)sender;
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand sahipgetir = new NpgsqlCommand("select dolaplar.sahip,dolaplar.tarih,musteri.adsoyad from dolaplar inner join musteri on dolaplar.sahip=musteri.serino where dolaplar.dolap='" + mybutton.Name + "'", baglanti);
            if(mybutton.BackColor== Color.FromArgb(255, 70, 70)|| mybutton.BackColor == Color.FromArgb(120, 80, 120))
            {
                lastClickedDolap = Convert.ToInt32(mybutton.Text);
                baglanti.Open();
                NpgsqlDataReader dr = sahipgetir.ExecuteReader();
                if (dr.Read())
                {
                    DRlblAdSoyad.Visible = DRlblGecenSure.Visible = DRlblRezervasyonTarihi.Visible = true;
                    serinoDR = (int)dr[0];
                    DRRezervasyonTarihiBox.Text = Convert.ToString(dr[1]);
                    DRAdSoyadBox.Text = Convert.ToString(dr[2]);
                }
                baglanti.Close();
                double farkdakika = -(Convert.ToDateTime(DRRezervasyonTarihiBox.Text) - DateTime.Now).TotalMinutes;
                farkdakika = (farkdakika < 1) ? farkdakika + 1 : farkdakika;
                DRGecenSureBox.Text = (farkdakika >= 60) ? Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika / 60))) + " saat " + Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika" : Convert.ToString(Convert.ToInt32(Math.Floor(farkdakika % 60))) + " dakika";
                DRUyari.Visible = (mybutton.BackColor == Color.FromArgb(120, 80, 120)) ? true : false;
            }
            else
            {
                serinoDR = lastClickedDolap = 0;
                DRRezervasyonTarihiBox.Text = DRAdSoyadBox.Text = DRGecenSureBox.Text = "";
                DRlblAdSoyad.Visible = DRlblGecenSure.Visible = DRlblRezervasyonTarihi.Visible = DRUyari.Visible = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SaatTakip_Tick(object sender, EventArgs e)
        {
            lblTarih.Text = Convert.ToString(DateTime.Now);
        }

















        //YENİ ÜYE
        private void btnYeniUye_Click(object sender, EventArgs e)
        {
            PanelGizle();
            pnlYeniUye.Visible = true;
            ButonlarEnable();
            btnYeniUye.Enabled = false;
            btnYeniUye.BackColor = Color.FromArgb(200, 200, 200);
        }

        private void UpDown_ValueChanged(object sender, EventArgs e)
        {
            UcretHesapla();
            pnlYeniUye.Focus();
        }

        private void UpDown_Click(object sender, EventArgs e)
        {
            pnlYeniUye.Focus();
        }

        private void rbnOgrenci_Click(object sender, EventArgs e)
        {
            UcretHesapla();
        }
        void UcretHesapla()
        {
            if (chbOgrenci.Checked == true)
            {
                if (chbGunluk.Checked == true)
                {
                    UcretBox.Text = UpDown.Value * 5 + " TL";
                }
                else
                {
                    UcretBox.Text = UpDown.Value * 7 + " TL";
                }
            }
            else
            {
                if (chbGunluk.Checked == true)
                {
                    UcretBox.Text = UpDown.Value * 6 + " TL";
                }
                else
                {
                    UcretBox.Text = UpDown.Value * 8 + " TL";
                }
            }
        }

        private void chbGunluk_Click(object sender, EventArgs e)
        {
            chbGunluk.Checked = true;
            chbSeanslik.Checked = false;
            UpDown.Location = new Point(368, 382);
            UpDown.RightToLeft = RightToLeft.No;
            UcretHesapla();
        }

        private void chbSeanslik_Click(object sender, EventArgs e)
        {
            chbSeanslik.Checked = true;
            chbGunluk.Checked = false;
            UpDown.Location = new Point(474, 382);
            UpDown.RightToLeft = RightToLeft.Yes;
            UcretHesapla();
        }

        void YURemoveAllFonk()
        {
            YUAdSoyadBox.Text = YUTCBox.Text = YUTelefonBox.Text = YUAdresBox.Text = "";
            YUCinsiyetBox.SelectedIndex = 0;
            chbOgrenci.Checked = false;
            chbGunluk.Checked = true;
            chbSeanslik.Checked = false;
            UpDown.Value = 15;
            UpDown.Location = new Point(368, 382);
            UcretHesapla();
        }
        private void YURemoveAll_Click(object sender, EventArgs e)
        {
            YURemoveAllFonk();
        }

        private void btnYUOlustur_Click(object sender, EventArgs e)
        {
            int TelefonDigits = YUTelefonBox.Text.Count(c => Char.IsDigit(c));
            if (YUAdSoyadBox.Text == "" || YUTCBox.Text == "" || YUTelefonBox.Text == "(___) ___-____" || YUCinsiyetBox.SelectedIndex == 0 || YUCinsiyetBox.SelectedIndex == -1)
            {
                lblYUUyari1.Visible = true;
                UyariGizleyici.Start();
            }
            else if (YUTCBox.Text.Length != 11)
            {
                lblYUUyari2.Visible = true;
                UyariGizleyici.Start();
            }
            else if (TelefonDigits < 10)
            {
                lblYUUyari3.Visible = true;
                UyariGizleyici.Start();
            }
            else
            {

                string GmiSmi = (chbGunluk.Checked == true && chbSeanslik.Checked == false) ? "gunluk" : "seanslik";
                string GmiSmi2 = (chbGunluk.Checked == true && chbSeanslik.Checked == false) ? "GÜNLÜK" : "SEANSLIK";
                NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                NpgsqlCommand YUOlustur = new NpgsqlCommand("INSERT INTO musteri (adsoyad,tckimlik,cinsiyet,telefon,ogrenci,adres,gunluk,seanslik,aktif) VALUES ('" + YUAdSoyadBox.Text + "','" + YUTCBox.Text + "','" + YUCinsiyetBox.SelectedItem + "','" + YUTelefonBox.Text + "'," + chbOgrenci.Checked + ",'" + YUAdresBox.Text + "',0,0,FALSE)", baglanti);
                NpgsqlCommand YUGSEkle = new NpgsqlCommand("UPDATE musteri SET " + GmiSmi + "='" + UpDown.Value + "' WHERE tckimlik='" + YUTCBox.Text + "'", baglanti);
                NpgsqlCommand GecmisYU = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES ('" + YUAdSoyadBox.Text + "','" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", YENİ ÜYE " + YUAdSoyadBox.Text + " " + UpDown.Value + " " + GmiSmi2 + " ekledi. " + UcretBox.Text + "','" + DateTime.Now + "',null)", baglanti);
                baglanti.Open();
                YUOlustur.ExecuteNonQuery();
                YUGSEkle.ExecuteNonQuery();
                GecmisYU.ExecuteNonQuery();
                baglanti.Close();
                GecmisTabloGuncelle();
                tumTabloGuncelle();
                YURemoveAllFonk();
                lblYUBasarili.Visible = true;
                UyariGizleyici.Start();
            }
        }























        //ÜYELİK İŞLEMLERİ
        private void btnUyelikIslemleri_Click(object sender, EventArgs e)
        {
            UIGetir();
        }

        private void YUTCBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar)|| e.KeyChar == (char)Keys.Back);
        }

        private void YUAdSoyadBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsDigit(e.KeyChar));
        }

        void GirisIzniVer()
        {
            string TMPdlpsifresi = (sifremi == false) ? "null" :  UISifreBox.Text[0].ToString() + UISifreBox.Text[2].ToString() + UISifreBox.Text[4].ToString() + UISifreBox.Text[6].ToString();
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand dolaplaraEkle = new NpgsqlCommand("UPDATE dolaplar SET sahip=" + serino + ",sifre=" + TMPdlpsifresi + ",tarih='" + DateTime.Now + "' WHERE dolapserino='" + dolaprand + "'", baglanti);
            string gunsifirla = (Convert.ToInt32(UISeanslikBox.Text) > 0) ? ",gunluk=0" : "";
            NpgsqlCommand aktifet = new NpgsqlCommand("UPDATE musteri SET aktif=true" + gunsifirla + ",eskisifre=" + TMPdlpsifresi + " WHERE serino='" + serino + "'", baglanti);
            baglanti.Open();
            dolaplaraEkle.ExecuteNonQuery();
            aktifet.ExecuteNonQuery();
            baglanti.Close();
            UIDolapSifresiBox.Text = UISifreBox.Text[0].ToString() + UISifreBox.Text[2].ToString() + UISifreBox.Text[4].ToString() + UISifreBox.Text[6].ToString();
            UISifreBox.ResetText();
            salonTabloGuncelle();

            checkdolap[dolaprand] = true;
            try
            {
                foreach (Control DolapButon in pnlDolapRezervasyonu.Controls)
                {
                    if (DolapButon.Name == "dolap" + dolaprand)
                    {
                        DolapButon.BackColor = Color.FromArgb(255, 70, 70);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            UIAktif = true;
            pnlUyelikIslemleri.Visible = false;
            UIGetir();
        }

        bool hatami = false;
        Random rastgele= new Random();
        private void UIbtnGirisIzni_Click(object sender, EventArgs e)
        {
            int SifreDigits = UISifreBox.Text.Count(c => Char.IsDigit(c));
            int toplam = 0;
            if (UIbtnGirisIzni.Text=="GİRİŞ İZNİ VER")
            {
                if (akaktiflik == true && Convert.ToDateTime(Acilis) - Convert.ToDateTime(lblTarih.Text) > sifiribul || akaktiflik == true && Convert.ToDateTime(Kapanis) - Convert.ToDateTime(lblTarih.Text) < sifiribul)
                {
                    MessageBox.Show(Acilis + " ve " + Kapanis + " saatleri arasında olmadığınız için bu işlem gerçekleştirilemiyor.\n\nBu özelliği deaktif etmek için Diğer Tüm İşlemler sekmesini kullanabilirsiniz.");
                }
                else if (herkesidndr == true)
                {
                    MessageBox.Show("Herkes dondurulmuşken salona kimse giremez.");
                }
                else
                {
                    if (sifremi == true && SifreDigits < 4)
                    {
                        lblUIUyari1.Visible = true;
                        UyariGizleyici.Start();
                        UISifreBox.Focus();
                        UISifreBox.SelectAll();
                    }
                    else
                    {
                        if (UICinsiyetBox.Text == "Erkek")
                        {
                            for (int sayac = 1; sayac < 41; sayac++)
                            {
                                if (checkdolap[sayac] == true)
                                {
                                    toplam++;
                                }
                            }
                            if (toplam < 40)
                            {
                                do
                                {
                                    dolaprand = rastgele.Next(1, 41);
                                }
                                while (checkdolap[dolaprand] == true);
                                GirisIzniVer();
                            }
                            else
                                MessageBox.Show("Erkeklere ait boş dolap kalmadı.");
                        }
                        else if (UICinsiyetBox.Text == "Kadın")
                        {
                            for (int sayac = 41; sayac < 81; sayac++)
                            {
                                if (checkdolap[sayac] == true)
                                {
                                    toplam++;
                                }
                            }
                            if (toplam < 40)
                            {
                                do
                                {
                                    dolaprand = rastgele.Next(41, 81);
                                }
                                while (checkdolap[dolaprand] == true);
                                GirisIzniVer();
                            }
                            else
                                MessageBox.Show("Kadınlara ait boş dolap kalmadı.");
                        }
                    }
                }
            }
            else if(UIbtnGirisIzni.Text == "ÇIKIŞ İZNİ VER")
            {
                NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                NpgsqlCommand dolapbul = new NpgsqlCommand("SELECT dolapserino FROM dolaplar WHERE sahip='" + serino + "'", baglanti);
                baglanti.Open();
                NpgsqlDataReader dr = dolapbul.ExecuteReader();
                if (dr.Read())
                {
                    dolaprand = (int)dr[0];
                    checkdolap[dolaprand] = false;
                }
                else
                {
                    MessageBox.Show("KRİTİK HATA #001");
                    hatami = true;
                }
                baglanti.Close();
                if (hatami == false)
                {
                    DRlblAdSoyad.Visible = DRlblGecenSure.Visible = DRlblRezervasyonTarihi.Visible = DRUyari.Visible = false;
                    DRAdSoyadBox.Text = DRGecenSureBox.Text = DRRezervasyonTarihiBox.Text = "";
                    try
                    {
                        foreach (Control DolapButon in pnlDolapRezervasyonu.Controls)
                        {
                            if (DolapButon.Name == "dolap" + dolaprand)
                            {
                                DolapButon.BackColor = Color.FromArgb(150, 255, 150);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    string seansazalt = (Convert.ToInt32(UISeanslikBox.Text) > 0 && farkdakika > 15) ? ",seanslik=seanslik-1" : "";
                    UISeanslikBox.Text = (Convert.ToInt32(UISeanslikBox.Text) > 0) ? Convert.ToString(Convert.ToInt32(UISeanslikBox.Text) - 1) : UISeanslikBox.Text;
                    NpgsqlCommand dolaplardanSil = new NpgsqlCommand("UPDATE dolaplar SET sahip=null,sifre=null,tarih=null WHERE sahip='" + serino + "'", baglanti);
                    NpgsqlCommand deaktifet = new NpgsqlCommand("UPDATE musteri SET aktif=false" + seansazalt + " WHERE serino='" + serino + "'", baglanti);
                    baglanti.Open();
                    dolaplardanSil.ExecuteNonQuery();
                    deaktifet.ExecuteNonQuery();
                    baglanti.Close();
                    if (seansazalt != "")
                    {
                        NpgsqlCommand GecmisUI1 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES ('" + UIAdSoyadBox.Text + "','" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", SEANS AZALTMA " + UIAdSoyadBox.Text + " isimli üyeden.','"+DateTime.Now+"',null)", baglanti);
                        baglanti.Open();
                        GecmisUI1.ExecuteNonQuery();
                        baglanti.Close();
                        GecmisTabloGuncelle();
                    }
                    salonTabloGuncelle();
                    tumTabloGuncelle();
                    UIAktif = false;
                    //serinoDR = 0;
                }
                pnlUyelikIslemleri.Visible = false;
                UIGetir();
            }
        }






















        //TÜM ÜYELERİ LİSTELE
        private void btnUyeListele_Click(object sender, EventArgs e)
        {
            ButonlarEnable();
            btnUyeListele.Enabled = false;
            btnUyeListele.BackColor = Color.FromArgb(200, 200, 200);
            PanelGizle();
            pnlTumUyeler.Visible = true;
            dgwTumUyeler.Focus();
        }

        private void dgwTumUyeler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            TULSecilenUye.Text = (dgwTumUyeler.SelectedCells.Count > 1) ? Convert.ToString(dgwTumUyeler.SelectedRows[0].Cells[0].Value) : "";
        }

        private void TULTelefonBox_Click_1(object sender, EventArgs e)
        {
            TULTelefonBox.SelectAll();
        }

        private void TULTelefonBox_DoubleClick(object sender, EventArgs e)
        {
            TULTelefonBox.SelectAll();
        }

        private void TULTCBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void TULAdsoyadBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsDigit(e.KeyChar));
        }

        private void TULAdsoyadBox_Click(object sender, EventArgs e)
        {
            TULAdsoyadBox.SelectAll();
        }

        private void TULTCBox_Click(object sender, EventArgs e)
        {
            TULTCBox.SelectAll();
        }

        void isimcinsiyet()
        {
            (dgwTumUyeler.DataSource as DataTable).DefaultView.RowFilter = String.Format("[Ad Soyad] like '%" + TULAdsoyadBox.Text + "%' and Cinsiyet  like '%" + TULCinsiyetBox.SelectedItem + "%'");
        }

        void isimcinsiyet2()
        {
            (dgwSalondakiler.DataSource as DataTable).DefaultView.RowFilter = String.Format("[Ad Soyad] like '%" + SAdSoyadBox.Text + "%' and Cinsiyet  like '%" + SCinsiyetBox.SelectedItem + "%'");
        }

        private void TULAdsoyadBox_TextChanged(object sender, EventArgs e)
        {
            if (TULAdsoyadBox.Focused)
            {
                TULTCBox.Text = TULTelefonBox.Text = "";
                isimcinsiyet();
            }
        }

        private void TULTCBox_TextChanged(object sender, EventArgs e)
        {
            if (TULTCBox.Focused)
            {
                TULAdsoyadBox.Text = TULTelefonBox.Text = "";
                TULCinsiyetBox.SelectedIndex = 0;
                (dgwTumUyeler.DataSource as DataTable).DefaultView.RowFilter = String.Format("[T.C Kimlik] like '" + TULTCBox.Text + "%'");
            }
        }

        private void TULTelefonBox_TextChanged(object sender, EventArgs e)
        {
            int TelefonDigits = TULTelefonBox.Text.Count(c => Char.IsDigit(c));
            if (TULTelefonBox.Focused)
            {
                TULAdsoyadBox.Text = TULTCBox.Text = "";
                TULCinsiyetBox.SelectedIndex = 0;
                if (TelefonDigits >= 6)
                {
                    (dgwTumUyeler.DataSource as DataTable).DefaultView.RowFilter = String.Format("Telefon like '%" + TULTelefonBox.Text + "%'");
                }
                else
                {
                    (dgwTumUyeler.DataSource as DataTable).DefaultView.RowFilter = String.Format("");
                }
            }
        }

        private void TULCinsiyetBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            isimcinsiyet();
        }

        private void dgwTumUyeler_Sorted(object sender, EventArgs e)
        {
            TULSecilenUye.ResetText();
        }

        string secilen = "";
        int serino, dolaprand;
        bool UIAktif;

        void secilenAktarma()
        {
            if (TULSecilenUye.Text != "" && pnlTumUyeler.Visible == true)
            {
                secilen = TULSecilenUye.Text;
            }
            else if (SSecilenUye.Text != "" && pnlSalondakiler.Visible == true)
            {
                secilen = SSecilenUye.Text;
            }
            else if (SecilenMBox.Text != "" && pnlGecmisIslemler.Visible == true)
            {
                secilen = SecilenMBox.Text;
            }
            else if (DRSecilenBox.Text != "" && pnlDolapRezervasyonu.Visible == true)
            {
                secilen = DRSecilenBox.Text;
            }
            if (TULSecilenUye.Text != "" && pnlTumUyeler.Visible == true || SSecilenUye.Text != "" && pnlSalondakiler.Visible == true || SecilenMBox.Text != "" && pnlGecmisIslemler.Visible == true || DRSecilenBox.Text != "" && pnlDolapRezervasyonu.Visible == true)
            {
                NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                baglanti.Open();
                NpgsqlCommand SecileniBul = new NpgsqlCommand("SELECT serino,adsoyad,tckimlik,cinsiyet,telefon,ogrenci,adres,gunluk,seanslik,aktif,eskisifre,dondurulmus FROM musteri WHERE adsoyad='" + secilen + "'", baglanti);
                NpgsqlDataReader dr = SecileniBul.ExecuteReader();
                if (dr.Read())
                {
                    serino = Convert.ToInt32(dr[0]);
                    UIAdSoyadBox.Text = dr[1].ToString();
                    UITCBox.Text = dr[2].ToString();
                    UICinsiyetBox.Text = dr[3].ToString();
                    pnlDolapSifresi.BackColor = UISifreBox.BackColor = (UICinsiyetBox.Text == "Erkek") ? Color.FromArgb(120, 120, 255) : Color.FromArgb(255, 150, 150);
                    UITelefonBox.Text = dr[4].ToString();
                    UIOgrenciBox.Text = ((bool)dr[5] == true) ? "Öğrenci" : "Öğrenci Değil";
                    UIAdresBox.Text = (dr[6].ToString().Count(c => Char.IsLetterOrDigit(c)) == 0) ? "Adres Bilgisi Girilmemiş" : dr[6].ToString();
                    UIGunlukBox.Text = Convert.ToString(dr[7]);
                    UISeanslikBox.Text = Convert.ToString(dr[8]);
                    UIAktif = (bool)dr[9];
                    UISifreBox.Text = (Convert.ToString(dr[10]).Length == 4) ? Convert.ToString(dr[10])[0].ToString() + " " + Convert.ToString(dr[10])[1].ToString() + " " + Convert.ToString(dr[10])[2].ToString() + " " + Convert.ToString(dr[10])[3].ToString() : "";
                    btnUyeyiDondur.Text = (Convert.ToBoolean(dr[11]) == true) ? "Geri Döndür" : "Üyeyi Dondur";
                }
                baglanti.Close();
                baglanti.Open();
                NpgsqlCommand SifreBul = new NpgsqlCommand("SELECT sifre FROM dolaplar WHERE sahip='" + serino + "'", baglanti);
                dr = SifreBul.ExecuteReader();
                if (dr.Read())
                {
                    UIDolapSifresiBox.Text = dr[0].ToString();
                }
                baglanti.Close();
                TULSecilenUye.Text = SSecilenUye.Text = "";
                if (UIGunlukBox.Text.Count(c => Char.IsDigit(c)) > 0)
                {
                    lblUIUyari2.Text = (Convert.ToInt32(UIGunlukBox.Text) < 0 || Convert.ToInt32(UIGunlukBox.Text) < -7 && Convert.ToInt32(UISeanslikBox.Text) > 0) ? "Bu kişi " + Convert.ToString(silSuresi + Convert.ToInt32(UIGunlukBox.Text)) + " gün içinde salona giriş yapmadığı takdirde üyeliği silinecek." : "";
                    UIGunlukBox.Text = (Convert.ToInt32(UIGunlukBox.Text) < 0) ? "0" : UIGunlukBox.Text;
                }
                UIGetir();
                girisKontrol();
            }
        }
        private void TULbtnAktar_Click(object sender, EventArgs e)
        {
            secilenAktarma();
        }

        private void TULCinsiyetBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (TULCinsiyetBox.Focused == true)
            {
                dgwTumUyeler.Focus();
            }
        }























        //DOLAP REZERVASYONU
        private void btnDolap_Click(object sender, EventArgs e)
        {
            PanelGizle();
            pnlDolapRezervasyonu.Visible = true;
            ButonlarEnable();
            btnDolap.Enabled = false;
            btnDolap.BackColor = Color.FromArgb(200, 200, 200);
            dolabiFocusla();

        }

        void dolabiFocusla()
        {
            if (lastClickedDolap != 0)
            {
                try
                {
                    foreach (Control DolapButon in pnlDolapRezervasyonu.Controls)
                    {
                        if (DolapButon.Name == "dolap" + lastClickedDolap)
                        {
                            DolapButon.Focus();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void UpDown2_ValueChanged(object sender, EventArgs e)
        {
            GunSeans();
        }

        private void UpDown2_Click(object sender, EventArgs e)
        {
            if (UpDown2.Focused == true)
            {
                pnlUyelikIslemleri.Focus();
            }
        }

        private void chbGun_Click(object sender, EventArgs e)
        {
            chbGun.Checked = true;
            chbSeans.Checked = false;
            GunSeans();
        }

        private void chbSeans_Click(object sender, EventArgs e)
        {
            chbGun.Checked = false;
            chbSeans.Checked = true;
            GunSeans();
        }






























        //SALONDAKİLER
        private void btnSalondakiler_Click(object sender, EventArgs e)
        {
            ButonlarEnable();
            btnSalondakiler.Enabled = false;
            btnSalondakiler.BackColor = Color.FromArgb(200, 200, 200);
            PanelGizle();
            pnlSalondakiler.Visible = true;
        }

        private void dgwSalondakiler_Sorted(object sender, EventArgs e)
        {
            SSecilenUye.ResetText();
        }

        private void SCinsiyetBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (SCinsiyetBox.Focused == true)
            {
                dgwSalondakiler.Focus();
            }
        }

        private void SAdSoyadBox_Click(object sender, EventArgs e)
        {
            SAdSoyadBox.SelectAll();
        }

        private void SAdSoyadBox_TextChanged(object sender, EventArgs e)
        {
            if (SAdSoyadBox.Focused)
            {
                SDolapAdiBox.ResetText();
                isimcinsiyet2();
            }
        }

        private void SAdSoyadBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsDigit(e.KeyChar));
        }

        private void SDolapAdiBox_TextChanged(object sender, EventArgs e)
        {
            if (SDolapAdiBox.Focused)
            {
                SAdSoyadBox.ResetText();
                SCinsiyetBox.SelectedIndex = 0;
                (dgwSalondakiler.DataSource as DataTable).DefaultView.RowFilter = String.Format("[Dolap Adı] like '%" + SDolapAdiBox.Text + "%'");
            }
        }

        private void SDolapAdiBox_Click(object sender, EventArgs e)
        {
            SDolapAdiBox.SelectAll();
        }

        private void SDolapAdiBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void lblDolap_Click(object sender, EventArgs e)
        {
            SDolapAdiBox.SelectAll();
            SDolapAdiBox.Focus();
        }

        private void dakikabasi_Tick(object sender, EventArgs e)
        {
            ReloadUyari.Visible = true;
            try
            {
                foreach (Control DolapButon in pnlDolapRezervasyonu.Controls)
                {
                    if (DolapButon.BackColor == Color.FromArgb(255, 70, 70))
                    {
                        NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                        NpgsqlCommand sorgu = new NpgsqlCommand("SELECT tarih FROM dolaplar WHERE dolapserino='" + Convert.ToInt32(DolapButon.Text) + "'", baglanti);
                        baglanti.Open();
                        NpgsqlDataReader dr = sorgu.ExecuteReader();
                        if (dr.Read())
                        {
                            if (-(Convert.ToDateTime(dr[0]) - DateTime.Now).TotalHours > 3)
                            {
                                DolapButon.BackColor = Color.FromArgb(120, 80, 120);
                            }
                        }
                        baglanti.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (serinoDR != 0)
            {
                gecenSureHesapla();
            }
            UyariGizleyici.Start();
        }

        private void dgwSalondakiler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SSecilenUye.Text = (dgwSalondakiler.SelectedCells.Count > 1) ? Convert.ToString(dgwSalondakiler.SelectedRows[0].Cells[0].Value) : "";
        }
















        void PLsifirla()
        {
            Padsoyad = Pkullaniciadi = Psifre = Ptelefon = PLAdSoyadBox.Text = PLTelefonBox.Text = PLKullaniciAdiBox.Text = PLSifreBox.Text = "";
            PLAdminlikBox.Enabled = true;
            PLAdminlikBox.SelectedIndex = 0;
            Pserino = Padmin = 0;
            PLEkle();
        }
        private void btnPersonelListele_Click(object sender, EventArgs e)
        {
            PanelGizle();
            PLsifirla();
            pnlPersoneller.Visible = true;
            ButonlarEnable();
            btnPersonelListele.BackColor = Color.FromArgb(250, 200, 200);
            btnPersonelListele.Enabled = false;
        }

        void PLguncelleIptal()
        {
            PLbtnEkleGuncelle.Visible = true;
            PLbtnEkleGuncelle.Text = "PERSONEL GÜNCELLE";
            PLbtnEkleGuncelle.Location = new Point(292, 550);
            PLbtnSilIptal.Enabled = true;
            PLbtnSilIptal.Text = "GERİ AL";
            PLbtnSilIptal.Location = new Point(468, 550);
        }

        void PLEkle()
        {
            PLbtnEkleGuncelle.Visible = true;
            PLbtnEkleGuncelle.Text = "PERSONEL EKLE";
            PLbtnEkleGuncelle.Location = new Point(380, 550);
            PLbtnSilIptal.Visible = false;
        }

        void PLSil()
        {
            PLbtnEkleGuncelle.Visible = false;
            PLbtnSilIptal.Text = "PERSONEL SİL";
            PLbtnSilIptal.Location = new Point(380, 550);
            PLbtnSilIptal.Visible = true;
            PLbtnSilIptal.Enabled = (Pserino == 1 || Pserino == girisYapanınSeriNumarası) ? false : true;
        }
        void PLVeritabanıGuncelle()
        {
            string admTmp1 = (Padmin == 1) ? "admin" : "personel";
            string admTmp2 = (PLAdminlikBox.SelectedIndex == 1) ? "admin " : "personel";
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand personelGuncelle = new NpgsqlCommand("UPDATE personel SET adsoyad='" + PLAdSoyadBox.Text + "',kullaniciadi='" + PLKullaniciAdiBox.Text + "',sifre='" + PLSifreBox.Text + "',sahip='" + adminlik + "',telefon='" + PLTelefonBox.Text + "' WHERE personelserino='" + Pserino + "'", baglanti);
            NpgsqlCommand GecmisPL3 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES (null,'" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", PERSONEL GÜNCELLE " + Padsoyad + ">" + PLAdSoyadBox.Text + ", " + Ptelefon + ">" + PLTelefonBox.Text + ", " + Pkullaniciadi + ">" + PLKullaniciAdiBox.Text + ", " + Psifre + ">" + PLSifreBox.Text + ", " + admTmp1 + ">" + admTmp2 + "','" + DateTime.Now + "','" + PLAdSoyadBox.Text + "')", baglanti);
            if (Pserino == 1)
            {
                baglanti.Open();
                personelGuncelle.ExecuteNonQuery();
                baglanti.Close();
                lblAdSoyad.Text = SahipAdi = PLAdSoyadBox.Text;
            }
            else
            {
                baglanti.Open();
                personelGuncelle.ExecuteNonQuery();
                GecmisPL3.ExecuteNonQuery();
                baglanti.Close();
            }
            PersonelTabloGuncelle();
            GecmisTabloGuncelle();
            Padsoyad = PLAdSoyadBox.Text;
            Pkullaniciadi = PLKullaniciAdiBox.Text;
            Psifre = PLSifreBox.Text;
            Padmin = PLAdminlikBox.SelectedIndex;
            Ptelefon = PLTelefonBox.Text;
            PLBasarili.Visible = true;
            UyariGizleyici.Start();
            PLSil();
        }

        void PLVeritabanıEkle()
        {
            int saglama = 0;
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand personelKontrol = new NpgsqlCommand("SELECT personelserino FROM personel WHERE adsoyad='" + PLAdSoyadBox.Text + "' OR kullaniciadi='" + PLKullaniciAdiBox.Text + "' OR telefon='" + PLTelefonBox.Text + "'", baglanti);
            baglanti.Open();
            NpgsqlDataReader dr = personelKontrol.ExecuteReader();
            if (dr.Read())
            {
                saglama = 1;
                PLUyari4.Visible = true;
                UyariGizleyici.Start();
            }
            baglanti.Close();
            if(saglama==1)
            {
                NpgsqlCommand personelEkle = new NpgsqlCommand("INSERT INTO personel (adsoyad,kullaniciadi,sifre,sahip,telefon) VALUES ('" + PLAdSoyadBox.Text + "','" + PLKullaniciAdiBox.Text + "','" + PLSifreBox.Text + "','" + adminlik + "','" + PLTelefonBox.Text + "')", baglanti);
                NpgsqlCommand GecmisPL1 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES (null,'" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", YENİ PERSONEL " + PLAdSoyadBox.Text + " " + PLAdminlikBox.SelectedItem + " olarak ekledi.','" + DateTime.Now + "','" + PLAdSoyadBox.Text + "')", baglanti);
                baglanti.Open();
                personelEkle.ExecuteNonQuery();
                GecmisPL1.ExecuteNonQuery();
                GecmisTabloGuncelle();
                baglanti.Close();
                PLsifirla();
                PersonelTabloGuncelle();
                PLBasarili.Visible = true;
                UyariGizleyici.Start();
                PLEkle();
            }
        }

        void degisiklikAlgila()
        {
            if (Pserino != 0)
            {
                if (Padsoyad != PLAdSoyadBox.Text)
                {
                    PLguncelleIptal();
                }
                else if (Ptelefon != PLTelefonBox.Text)
                {
                    PLguncelleIptal();
                }
                else if (Pkullaniciadi != PLKullaniciAdiBox.Text)
                {
                    PLguncelleIptal();
                }
                else if (Psifre != PLSifreBox.Text)
                {
                    PLguncelleIptal();
                }
                else if (Padmin != PLAdminlikBox.SelectedIndex)
                {
                    PLguncelleIptal();
                }
                else
                {
                    PLSil();
                }
            }
        }

        string Padsoyad, Pkullaniciadi, Psifre, Ptelefon;
        int Pserino = 0, Padmin = 0;
        private void dgwPersoneller_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand personeliGetir = new NpgsqlCommand("SELECT personelserino,adsoyad,kullaniciadi,sifre,sahip,telefon FROM personel WHERE adsoyad='" + Convert.ToString(dgwPersoneller.SelectedRows[0].Cells[0].Value) + "'", baglanti);
            baglanti.Open();
            NpgsqlDataReader dr = personeliGetir.ExecuteReader();
            if (dr.Read())
            {
                Pserino = 0;
                Padsoyad = PLAdSoyadBox.Text = dr[1].ToString();
                Pkullaniciadi = PLKullaniciAdiBox.Text = dr[2].ToString();
                Psifre = PLSifreBox.Text = dr[3].ToString();
                Padmin = PLAdminlikBox.SelectedIndex = (Convert.ToBoolean(dr[4]) == true) ? 1 : 0;
                Ptelefon = PLTelefonBox.Text = dr[5].ToString();
                Pserino = (int)dr[0];
                PLAdminlikBox.Enabled = (Pserino == 1 || girisYapanınSeriNumarası == Pserino) ? false : true;
                PLAdSoyadBox.Enabled = PLTelefonBox.Enabled = (girisYapanınSeriNumarası == Pserino && girisYapanınSeriNumarası != 1) ? false : true;
                PLSifreBox.PasswordChar = PLKullaniciAdiBox.PasswordChar = (Pserino == 1 && girisYapanınSeriNumarası != 1) ? '*' : '\0';
                if (Pserino == 1)
                {
                    PLUyari.Visible = false;
                }
                if (girisYapanınSeriNumarası == Pserino)
                {
                    PLAdminlikBox.Enabled = false;
                }
                PLSil();
            }
            baglanti.Close();
        }

        private void PLAdSoyadBox_TextChanged(object sender, EventArgs e)
        {
            degisiklikAlgila();
        }

        private void PLTelefonBox_TextChanged(object sender, EventArgs e)
        {
            degisiklikAlgila();
        }

        private void PLKullaniciAdiBox_TextChanged(object sender, EventArgs e)
        {
            degisiklikAlgila();
        }

        private void PLSifreBox_TextChanged(object sender, EventArgs e)
        {
            degisiklikAlgila();
        }

        private void PLbtnSilIptal_Click(object sender, EventArgs e)
        {
            if(PLbtnSilIptal.Text == "PERSONEL SİL")
            {
                DialogResult SilOnay = MessageBox.Show("Bu kişi kalıcı olarak silinecektir.\nOnaylıyor musunuz?", "", MessageBoxButtons.YesNo);
                if (SilOnay == DialogResult.Yes)
                {
                    NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                    NpgsqlCommand personelSil = new NpgsqlCommand("DELETE FROM personel WHERE personelserino='" + Pserino + "'", baglanti);
                    NpgsqlCommand GecmisPL2 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES (null,'" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", PERSONEL SİLME " + Padsoyad + ", Kullanıcı Adı= " + Pkullaniciadi + ", Şifre= " + Psifre + ", Telefon= " + Ptelefon + "','" + DateTime.Now + "',null)", baglanti);
                    baglanti.Open();
                    personelSil.ExecuteNonQuery();
                    GecmisPL2.ExecuteNonQuery();
                    baglanti.Close();
                    PLsifirla();
                    PersonelTabloGuncelle();
                }
            }
            else if(PLbtnSilIptal.Text=="GERİ AL")
            {
                PLAdSoyadBox.Text = Padsoyad;
                PLTelefonBox.Text = Ptelefon;
                PLKullaniciAdiBox.Text = Pkullaniciadi;
                PLSifreBox.Text = Psifre;
                PLAdminlikBox.SelectedIndex = Padmin;
                PLSil();
            }
        }

        bool adminlik;
        private void PLbtnEkleGuncelle_Click(object sender, EventArgs e)
        {
            adminlik = (PLAdminlikBox.SelectedIndex == 1) ? true : false;
            int PersonelTelefonDigits = PLTelefonBox.Text.Count(c => Char.IsDigit(c));
            if (PLAdSoyadBox.Text == "" || PLTelefonBox.Text == "(___) ___-____" || PLKullaniciAdiBox.Text == "" || PLSifreBox.Text == "")
            {
                PLUyari1.Visible = true;
                UyariGizleyici.Start();
            }
            else if (PersonelTelefonDigits < 10)
            {
                PLUyari2.Visible = true;
                UyariGizleyici.Start();
            }
            else if (PLKullaniciAdiBox.Text.Length < 5 || PLSifreBox.Text.Length < 5)
            {
                PLUyari3.Visible = true;
                UyariGizleyici.Start();
            }
            else
            {
                if (PLbtnEkleGuncelle.Text == "PERSONEL EKLE")
                {
                    if (PLAdminlikBox.SelectedIndex == 1)
                    {
                        DialogResult EkleOnay = MessageBox.Show("Admin yetkilerinin kapsamı çok fazladır.\nBu kişinin admin yetkilerine sahip olmasını istiyor musunuz?", "", MessageBoxButtons.YesNo);
                        if (EkleOnay == DialogResult.Yes)
                        {
                            PLVeritabanıEkle();
                        }
                    }
                    else
                    {
                        PLVeritabanıEkle();
                    }
                }
                else if (PLbtnEkleGuncelle.Text == "PERSONEL GÜNCELLE")
                {
                    if (PLAdminlikBox.SelectedIndex == 1 && Padmin != 1)
                    {
                        DialogResult GuncelleOnay = MessageBox.Show("Admin yetkilerinin kapsamı çok fazladır.\nBu kişinin admin yetkilerine sahip olmasını istiyor musunuz?", "", MessageBoxButtons.YesNo);
                        if (GuncelleOnay == DialogResult.Yes)
                        {
                            PLVeritabanıGuncelle();
                        }
                    }
                    else
                    {
                        PLVeritabanıGuncelle();
                    }
                }
            }
        }

        private void PLReset_Click(object sender, EventArgs e)
        {
            PLsifirla();
        }

        private void btnGecmisIslemler_Click(object sender, EventArgs e)
        {
            PanelGizle();
            pnlGecmisIslemler.Visible = true;
            ButonlarEnable();
            btnGecmisIslemler.BackColor = Color.FromArgb(250, 200, 200);
            btnGecmisIslemler.Enabled = false;
            if (GIIslembox.Focused == true)
                dgwGecmis.Focus();
        }

        private void UyariGizleyici_Tick(object sender, EventArgs e)
        {
            lblUIUyari1.Visible = lblYUUyari1.Visible = lblYUUyari2.Visible = lblYUUyari3.Visible = lblYUBasarili.Visible = PLUyari1.Visible = PLUyari2.Visible = PLUyari3.Visible = PLUyari4.Visible = PLBasarili.Visible = ReloadUyari.Visible = false;
            UyariGizleyici.Stop();
        }

        private void btnPAktar_Click(object sender, EventArgs e)
        {
            p1mip2mi = SecilenPBox.Text;
            personelgetir();
        }

        private void SecilenMBox_TextChanged(object sender, EventArgs e)
        {
            GIlblSecilenM.Visible = SecilenMBox.Visible = btnMAktar.Visible = (SecilenMBox.Text.Length > 1) ? true : false;
        }

        private void SecilenPBox_TextChanged(object sender, EventArgs e)
        {
            GIlblSecilenP.Visible = SecilenPBox.Visible = btnPAktar.Visible = (SecilenPBox.Text.Length > 1) ? true : false;
            if (SecilenPBox.Text == SahipAdi)
            {
                btnPAktar.Enabled = (lblAdSoyad.Text == SahipAdi) ? true : false;
            }
            else
            {
                btnPAktar.Enabled = true;
            }
        }

        private void dgwGecmis_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            GIIslembox.Visible = GIReset.Visible = GIlblIslem.Visible = true;
            SecilenMBox.Text = Convert.ToString(dgwGecmis.SelectedRows[0].Cells[0].Value);
            SecilenPBox.Text = Convert.ToString(dgwGecmis.SelectedRows[0].Cells[1].Value);
            GIIslembox.Text = Convert.ToString(dgwGecmis.SelectedRows[0].Cells[2].Value);
            SecilenPBox2.Text = Convert.ToString(dgwGecmis.SelectedRows[0].Cells[4].Value);
        }

        private void GIReset_Click(object sender, EventArgs e)
        {
            SecilenMBox.Text = SecilenPBox.Text = GIIslembox.Text = "";
            GIIslembox.Visible = GIlblIslem.Visible = GIReset.Visible = false;
        }

        private void btnMAktar_Click(object sender, EventArgs e)
        {
            secilenAktarma();
        }

        private void DRAdSoyadBox_TextChanged(object sender, EventArgs e)
        {
            DRSecilenBox.Text = DRAdSoyadBox.Text;
            DRSecilenBox.Visible = btnDRAktar.Visible = (DRAdSoyadBox.Text != "") ? true : false;
        }

        private void SecilenPBox2_TextChanged(object sender, EventArgs e)
        {
            GIlblSecilenP2.Visible = SecilenPBox2.Visible = btnPAktar2.Visible = (SecilenPBox2.Text.Length > 1) ? true : false;
            if (SecilenPBox2.Text == SahipAdi)
            {
                btnPAktar2.Enabled = (lblAdSoyad.Text == SahipAdi) ? true : false;
            }
            else
            {
                btnPAktar2.Enabled = true;
            }
        }

        private void PLKullaniciAdiBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == (char)Keys.Space);
        }

        private void PLSifreBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == (char)Keys.Space);
        }

        private void btnPAktar2_Click(object sender, EventArgs e)
        {
            p1mip2mi = SecilenPBox2.Text;
            personelgetir();
        }

        private void btnGoster1_Click(object sender, EventArgs e)
        {
            UIDolapSifresiBox.PasswordChar = (UIDolapSifresiBox.PasswordChar == '*') ? '\0' : '*';
            btnGoster1.Text = (btnGoster1.Text == "göster") ? "gizle" : "göster";
        }

        private void UIDolapSifresiBox_TextChanged(object sender, EventArgs e)
        {
            btnGoster1.Visible = (UIDolapSifresiBox.TextLength == 4) ? true : false;
        }

        private void pnlDolapSifresi_Click(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        private void lblDolapSifresi_Click(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        private void pnlDolapSifresi_DoubleClick(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        private void lblDolapSifresi_DoubleClick(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        private void UISifreBox_Click(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        private void UISifreBox_DoubleClick(object sender, EventArgs e)
        {
            UISifreBox.Focus();
            UISifreBox.SelectAll();
        }

        void DigerIslemlerReset()
        {
            chbSifreli.Checked = (sifremi == true) ? true : false;
            chbAnahtarlı.Checked = (sifremi == true) ? false : true;
            chbIslemYapma.Checked = (ucsaatuyarmi == true) ? false : true;
            chbUcSaatiGecerseUyar.Checked = (ucsaatuyarmi == true) ? true : false;
            chbAcilisKapanisAktif.Checked = (akaktiflik == true) ? true : false;
            btnUyelikleriDondur.Text = (herkesidndr == true) ? "ÜYELİKLERİ GERİ AÇ" : "ÜYELİKLERİ DONDUR";
            AcilisSaat.Text = Convert.ToString(Acilis[0]) + Convert.ToString(Acilis[1]);
            AcilisDakika.Text = Convert.ToString(Acilis[3]) + Convert.ToString(Acilis[4]);
            KapanisSaat.Text = Convert.ToString(Kapanis[0]) + Convert.ToString(Kapanis[1]);
            KapanisDakika.Text = Convert.ToString(Kapanis[3]) + Convert.ToString(Kapanis[4]);
            UyeSilinmeSuresiBox.Text = Convert.ToString(silSuresi);
            DTIbtnKaydet.Enabled = true;
        }

        private void btnDigerIslemler_Click(object sender, EventArgs e)
        {
            DigerIslemlerReset();
            ButonlarEnable();
            PanelGizle();
            pnlDigerIslemler.Visible = true;
            btnDigerIslemler.Enabled = false;
            btnDigerIslemler.BackColor = Color.FromArgb(250, 200, 200);
        }

        private void chbSifreli_Click(object sender, EventArgs e)
        {
            chbSifreli.Checked = true;
            chbAnahtarlı.Checked = false;
        }

        private void DTIbtnKaydet_Click(object sender, EventArgs e)
        {
            if (UyeSilinmeSuresiBox.Text.Count(c => Char.IsDigit(c)) >= 1)
            {
                UyeSilinmeSuresiBox.Text = (Convert.ToInt32(UyeSilinmeSuresiBox.Text) < 90) ? "90" : UyeSilinmeSuresiBox.Text;
            }
            if(AcilisSaat.Text.Count(c => Char.IsDigit(c)) == 1)
            {
                AcilisSaat.Text = (Convert.ToInt32(AcilisSaat.Text) < 6) ? AcilisSaat.Text = "06" : AcilisSaat.Text = "0" + AcilisSaat.Text;
            }
            if (AcilisSaat.Text == "12")
            {
                AcilisDakika.Text = "00";
            }
            AcilisDakika.Text = (AcilisDakika.Text.Count(c => Char.IsDigit(c)) == 1) ? AcilisDakika.Text = "0" + AcilisDakika.Text : AcilisDakika.Text;
            KapanisDakika.Text = (KapanisDakika.Text.Count(c => Char.IsDigit(c)) == 1) ? KapanisDakika.Text = "0" + KapanisDakika.Text : KapanisDakika.Text;
            
            if (AcilisSaat.Text.Count(c => Char.IsDigit(c)) == 2 && KapanisSaat.Text.Count(c => Char.IsDigit(c)) == 2 && AcilisDakika.Text.Count(c => Char.IsDigit(c)) == 2 && KapanisDakika.Text.Count(c => Char.IsDigit(c)) == 2 && UyeSilinmeSuresiBox.Text.Count(c => Char.IsDigit(c)) > 1)
            {
                DialogResult kaydetmek = MessageBox.Show("Bu işlemin gerçekleşebilmesi için uygulamanın yeniden başlaması gerekmektedir.\nEmin misiniz?", "", MessageBoxButtons.YesNo);
                if (kaydetmek == DialogResult.Yes)
                {
                    string acilisYeni = AcilisSaat.Text + ":" + AcilisDakika.Text;
                    string kapanisYeni = KapanisSaat.Text + ":" + KapanisDakika.Text;
                    string aciklamaYeni = (DTUAdminAciklamaBox.Text.Count(c => Char.IsLetterOrDigit(c)) <= 0) ? "Şimdilik Hiçbir açıklama yok." : DTUAdminAciklamaBox.Text;
                    bool sifremiYeni = (chbSifreli.Checked == true) ? true : false;
                    bool ucsaatuyarmiYeni = (chbIslemYapma.Checked == true) ? false : true;
                    bool akaktiflikYeni = (chbAcilisKapanisAktif.Checked == true) ? true : false;
                    string silsuresiYeni = UyeSilinmeSuresiBox.Text;
                    NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                    NpgsqlCommand kaydet = new NpgsqlCommand("UPDATE uygulama SET adminaciklama='" + aciklamaYeni + "',sifremi='" + sifremiYeni + "',ucsaatigecerseuyar='" + ucsaatuyarmiYeni + "',acilis='" + acilisYeni + "',kapanis='" + kapanisYeni + "',silsuresi='" + silsuresiYeni + "',akaktiflik='" + akaktiflikYeni + "'", baglanti);
                    NpgsqlCommand GecmisDTI1 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES (null,'" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", UYGULAMA AYARLARI Açıklama= " + aciklamaYeni + ", ŞifreMi= " + Convert.ToString(sifremiYeni) + ", Üç Saat Uyarısı= " + Convert.ToString(ucsaatuyarmiYeni) + ", Açılış Kapanış Aktiflik= " + Convert.ToString(akaktiflikYeni) + ", Açılış Saati= " + acilisYeni + ", Kapanış Saati= " + kapanisYeni + ", Silinme Süresi= " + silsuresiYeni + ".','" + DateTime.Now + "',null)", baglanti);
                    baglanti.Open();
                    kaydet.ExecuteNonQuery();
                    GecmisDTI1.ExecuteNonQuery();
                    baglanti.Close();
                    Application.Exit();
                }
            }
            else
            {
                lblAcilis.ForeColor = lblKapanis.ForeColor = lblUyeSilinmeSuresi.ForeColor = Color.Red;
            }
        }

        private void AcilisSaat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void AcilisDakika_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void KapanisSaat_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void KapanisDakika_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void AcilisSaat_TextChanged(object sender, EventArgs e)
        {
            if (AcilisSaat.Text.Count(c => Char.IsDigit(c)) > 1)
            {
                if (Convert.ToInt32(AcilisSaat.Text) < 6)
                {
                    AcilisSaat.Text = "06";
                }
                if (Convert.ToInt32(AcilisSaat.Text) > 12)
                {
                    AcilisSaat.Text = "12";
                }
            }
        }

        private void AcilisDakika_TextChanged(object sender, EventArgs e)
        {
            if (AcilisDakika.Text.Count(c => Char.IsDigit(c)) > 0)
            {
                if (AcilisSaat.Text.Count(c => Char.IsDigit(c)) == 12)
                {
                    AcilisDakika.Text = "00";
                }
                else if (Convert.ToInt32(AcilisDakika.Text) > 59)
                {
                    AcilisDakika.Text = "59";
                }
            }
        }

        private void KapanisSaat_TextChanged(object sender, EventArgs e)
        {
            if (KapanisSaat.Text.Count(c => Char.IsDigit(c)) > 1)
            {
                if (Convert.ToInt32(KapanisSaat.Text) < 15)
                {
                    KapanisSaat.Text = "15";
                }
                else if(Convert.ToInt32(KapanisSaat.Text) > 23)
                {
                    KapanisSaat.Text = "23";
                    
                }
            }
        }

        private void UyeSilinmeSuresiBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void UyeSilinmeSuresiBox_TextChanged(object sender, EventArgs e)
        {
            if (UyeSilinmeSuresiBox.Text.Count(c => Char.IsDigit(c)) == 3)
            {
                if (Convert.ToInt32(UyeSilinmeSuresiBox.Text)>360)
                {
                    UyeSilinmeSuresiBox.Text = "360";
                }
            }
        }

        private void btnGunSeansEkle_Click(object sender, EventArgs e)
        {
            int GSSayisi;
            string GmiSmi3 = (lblGunSeans.Text == "Gün/Seans Ekle" && chbGun.Checked == true || lblGunSeans.Text == "Gün Ekle") ? "gunluk" : "seanslik";
            string GmiSmi4 = (GmiSmi3 == "gunluk") ? "GÜNLÜK" : "SEANSLIK";
            if(GmiSmi3 == "gunluk")
            {
                GSSayisi = (Convert.ToInt32(UIGunlukBox.Text) <= 0) ? Convert.ToInt32(UpDown2.Value) : (Convert.ToInt32(UIGunlukBox.Text) + Convert.ToInt32(UpDown2.Value));
            }
            else
            {
                GSSayisi = Convert.ToInt32(UISeanslikBox.Text) + Convert.ToInt32(UpDown2.Value);
            }
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand UIGSEkle = new NpgsqlCommand("UPDATE musteri SET " + GmiSmi3 + "='" + GSSayisi + "' WHERE tckimlik='" + UITCBox.Text + "'", baglanti);
            NpgsqlCommand GecmisYU1 = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES ('" + UIAdSoyadBox.Text + "','" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", GÜN/SEANS EKLEME " + UIAdSoyadBox.Text + " " + UpDown2.Value + " kadar " + GmiSmi4 + " ekledi ve toplam " + Convert.ToString(GSSayisi) + " oldu. " + UcretBox2.Text + "','" + DateTime.Now + "',null)", baglanti);
            baglanti.Open();
            UIGSEkle.ExecuteNonQuery();
            GecmisYU1.ExecuteNonQuery();
            baglanti.Close();
            if (GmiSmi3 == "gunluk")
            {
                UIGunlukBox.Text = Convert.ToString(GSSayisi);
            }
            else
            {
                UISeanslikBox.Text = Convert.ToString(GSSayisi);
            }
            tumTabloGuncelle();
            girisKontrol();
            lblUIUyari2.Text = "";
        }

        private void pnlGunSeans_EnabledChanged(object sender, EventArgs e)
        {
            lblGunSeansUyari.Visible = (pnlGunSeans.Enabled == true) ? false : true;
        }

        private void UIAdSoyadBox_TextChanged(object sender, EventArgs e)
        {
            lblGunSeansUyari.Text = (UIAdSoyadBox.Text.Count(c => Char.IsLetterOrDigit(c)) > 0) ? "Üyenin salonda olmaması gerekiyor." : "Tüm Üyeleri Listele'den üye seçmelisiniz.";
        }

        private void btnUyeyiDondur_TextChanged(object sender, EventArgs e)
        {
            btnUyeyiDondur.Visible = (girisYapanınSeriNumarası == 1 || btnUyeyiDondur.Text == "Geri Döndür") ? true : false;
            btnUyeyiDondur.BackColor = (btnUyeyiDondur.Text == "Geri Döndür") ? Color.FromArgb(255, 0, 0) : Color.FromArgb(50, 20, 70);
        }

        private void btnUyeyiDondur_Click(object sender, EventArgs e)
        {
            string dialogRes = (btnUyeyiDondur.Text == "Üyeyi Dondur") ? "Bu işlem " + UIAdSoyadBox.Text + " adlı üyenin günlük haklarının azalmasını engeller.\nÜyeliği dondurmak istediğinize emin misiniz?" : "Bu işlem " + UIAdSoyadBox.Text + " adlı üyenin günlük haklarının azalmaya devam etmesini sağlar.\nÜyelikleri dondurmak istediğinize emin misiniz?";
            DialogResult uyeyiDondur = MessageBox.Show(dialogRes, "", MessageBoxButtons.YesNo);
            if (uyeyiDondur == DialogResult.Yes)
            {
                string truefalse = (btnUyeyiDondur.Text == "Üyeyi Dondur") ? "true" : "false";
                string deMi = (btnUyeyiDondur.Text == "Üyeyi Dondur") ? "adlı üyeyi dondurdu" : "adlı üyenin hesabını geri açtı";
                btnUyeyiDondur.Text = (btnUyeyiDondur.Text == "Üyeyi Dondur") ? "Geri Döndür" : "Üyeyi Dondur";
                NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                NpgsqlCommand uyeyidondurcommand = new NpgsqlCommand("UPDATE musteri SET dondurulmus=" + truefalse + " WHERE adsoyad='" + UIAdSoyadBox.Text + "'", baglanti);
                NpgsqlCommand GecmisUD = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES ('" + UIAdSoyadBox.Text + "','" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", ÜYEYİ DONDUR " + UIAdSoyadBox.Text + " " + deMi + ".','" + DateTime.Now + "',null)", baglanti);
                baglanti.Open();
                uyeyidondurcommand.ExecuteNonQuery();
                GecmisUD.ExecuteNonQuery();
                baglanti.Close();
                GecmisTabloGuncelle();
            }
        }

        void girisKontrol()
        {
            if (UIGunlukBox.Text.Count(c => Char.IsDigit(c)) > 0 && UISeanslikBox.Text.Count(c => Char.IsDigit(c)) > 0)
            {
                if (Convert.ToInt32(UIGunlukBox.Text) <= 0 && Convert.ToInt32(UISeanslikBox.Text) <= 0)
                {
                    UIbtnGirisIzni.Enabled = (UIbtnGirisIzni.Text == "GİRİŞ İZNİ VER") ? false : true;
                }
                else
                {
                    UIbtnGirisIzni.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("beceremedic");
            }
        }
        private void UIbtnGirisIzni_TextChanged(object sender, EventArgs e)
        {
            girisKontrol();
        }

        private void btnUyelikleriDondur_Click(object sender, EventArgs e)
        {
            string dialogRes = (btnUyelikleriDondur.Text == "ÜYELİKLERİ DONDUR") ? "Bu işlem üyelerin günlük haklarının azalmasını engeller.\nÜyelikleri dondurmak istediğinize emin misiniz?" : "Bu işlem üyelerin günlük haklarının azalmaya devam etmesini sağlar.\nÜyelikleri dondurmak istediğinize emin misiniz?";
            DialogResult herkesidondur = MessageBox.Show(dialogRes, "", MessageBoxButtons.YesNo);
            if (herkesidondur == DialogResult.Yes)
            {
                string truefalse = (btnUyelikleriDondur.Text == "ÜYELİKLERİ DONDUR") ? "true" : "false";
                string deMi = (btnUyelikleriDondur.Text == "ÜYELİKLERİ DONDUR") ? "" : "de";
                btnUyelikleriDondur.Text = (btnUyelikleriDondur.Text == "ÜYELİKLERİ DONDUR") ? "ÜYELİKLERİ GERİ AÇ" : "ÜYELİKLERİ DONDUR";
                herkesidndr = (btnUyelikleriDondur.Text == "ÜYELİKLERİ DONDUR") ? true : false;
                NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
                NpgsqlCommand herkesidondurcommand = new NpgsqlCommand("UPDATE uygulama SET herkesidondur=" + truefalse + "", baglanti);
                NpgsqlCommand GecmisHD = new NpgsqlCommand("INSERT INTO gecmis (mstr,prsnl,islem,tarih,prsnl2) VALUES (null,'" + lblAdSoyad.Text + "','" + lblAdSoyad.Text + ", HERKESİ DONDUR " + deMi + "aktifledi.','" + DateTime.Now + "',null)", baglanti);
                baglanti.Open();
                herkesidondurcommand.ExecuteNonQuery();
                GecmisHD.ExecuteNonQuery();
                baglanti.Close();
                GecmisTabloGuncelle();
            }
        }

        private void chbAcilisKapanisAktif_CheckedChanged(object sender, EventArgs e)
        {
            chbAcilisKapanisAktif.Text = (chbAcilisKapanisAktif.Checked == true) ? "= Özellik Aktif" : "= Özellik Deaktif";
        }

        private void KapanisDakika_TextChanged(object sender, EventArgs e)
        {
            if (KapanisDakika.Text.Count(c => Char.IsDigit(c)) > 0)
            {
                if (Convert.ToInt32(KapanisDakika.Text) > 59)
                {
                    KapanisDakika.Text = "59";
                }
            }
        }

        private void chbAnahtarlı_Click(object sender, EventArgs e)
        {
            chbSifreli.Checked = false;
            chbAnahtarlı.Checked = true;
        }

        private void chbIslemYapma_Click(object sender, EventArgs e)
        {
            chbIslemYapma.Checked = true;
            chbUcSaatiGecerseUyar.Checked = false;
        }

        private void chbUcSaatiGecerseUyar_Click(object sender, EventArgs e)
        {
            chbIslemYapma.Checked = false;
            chbUcSaatiGecerseUyar.Checked = true;
        }

        private void btnDRAktar_Click(object sender, EventArgs e)
        {
            secilenAktarma();
        }

        private void PLAdSoyadBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsDigit(e.KeyChar));
        }

        private void PLAdminlikBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Pserino != 1)
            {
                PLUyari.Visible = (PLAdminlikBox.SelectedIndex == 1) ? true : false;
            }
            degisiklikAlgila();
        }

        private void SbtnAktar_Click(object sender, EventArgs e)
        {
            secilenAktarma();
        }

        private void SCinsiyetBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            isimcinsiyet2();
        }

        string p1mip2mi="";
        void personelgetir()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand pgetirs = new NpgsqlCommand("SELECT personelserino,adsoyad,kullaniciadi,sifre,sahip,telefon FROM personel WHERE adsoyad='" + p1mip2mi + "'", baglanti);
            baglanti.Open();
            NpgsqlDataReader dr = pgetirs.ExecuteReader();
            if (dr.Read())
            {
                Pserino = 0;
                Pserino = (int)dr[0];
                PLAdSoyadBox.Text = dr[1].ToString();
                PLKullaniciAdiBox.Text = dr[2].ToString();
                PLSifreBox.Text = dr[3].ToString();
                PLAdminlikBox.SelectedIndex = (Convert.ToBoolean(dr[4]) == true) ? 1 : 0;
                PLTelefonBox.Text = dr[5].ToString();
                PLAdminlikBox.Enabled = (Pserino == 1 || girisYapanınSeriNumarası == Pserino) ? false : true;
                PLAdSoyadBox.Enabled = PLTelefonBox.Enabled = (girisYapanınSeriNumarası == Pserino && girisYapanınSeriNumarası != 1) ? false : true;
                if (Pserino == 1)
                {
                    PLUyari.Visible = false;
                }
                if (girisYapanınSeriNumarası == Pserino)
                {
                    PLAdminlikBox.Enabled = false;
                }
                PLSil();
                PanelGizle();
                pnlPersoneller.Visible = true;
                ButonlarEnable();
                btnPersonelListele.BackColor = Color.FromArgb(250, 200, 200);
                btnPersonelListele.Enabled = false;
            }
            baglanti.Close();
        }

    }
}
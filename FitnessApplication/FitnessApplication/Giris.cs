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
    public partial class Giris : Form
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

        public Giris()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        void GirisTikla()
        {
            if (KullaniciAdiBox.Text == "")
            {
                MessageBox.Show("Kullanıcı adı alanı boş bırakılamaz.");
            }
            else if (SifreBox.Text == "")
            {
                MessageBox.Show("Şifre alanı boş bırakılamaz.");
            }
            else
            {
                lblBekle.Visible = true;
                btnGiris.Visible = false;
                bekletici.Start();
            }
        }
        
        private void Giris_Load(object sender, EventArgs e)
        {
            UygulamaAyariGiris();
            SahipAdiNe();
            
        }
        string sa, ac, kapa;
        void SahipAdiNe()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand adBul = new NpgsqlCommand("SELECT adsoyad FROM personel WHERE personelserino=1", baglanti);
            baglanti.Open();
            NpgsqlDataReader dr = adBul.ExecuteReader();
            if (dr.Read())
            {
                sa = dr[0].ToString();
            }
            baglanti.Close();
        }

        bool sifreMi,UcSaat,akakt, hrksdndr;
        int eksilecekGun, silsure;

        void UygulamaAyariGiris()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand baslangicbaglantisi = new NpgsqlCommand("SELECT versiyon,adminaciklama,sifremi,songiris,ucsaatigecerseuyar,acilis,kapanis,silsuresi,akaktiflik,herkesidondur FROM uygulama WHERE serino=1", baglanti);
            baglanti.Open();
            NpgsqlDataReader dr = baslangicbaglantisi.ExecuteReader();
            if (dr.Read())
            {
                lblVersiyon.Text = dr[0].ToString();
                lblAciklama.Text = dr[1].ToString();
                sifreMi = Convert.ToBoolean(dr[2]);

                eksilecekGun = Convert.ToInt32(-Math.Floor((Convert.ToDateTime(dr[3]) - DateTime.Now).TotalDays + 1));
                eksilecekGun = (eksilecekGun > 1) ? eksilecekGun - 1 : eksilecekGun;

                UcSaat = Convert.ToBoolean(dr[4]);
                ac = dr[5].ToString();
                kapa = dr[6].ToString();
                silsure = Convert.ToInt32(dr[7]);
                akakt = Convert.ToBoolean(dr[8]);
                eksilecekGun = (Convert.ToBoolean(dr[9]) == false) ? eksilecekGun : 0;
                hrksdndr = Convert.ToBoolean(dr[9]);

            }
            baglanti.Close();
            gunleriAzaltveGuncelle();
        }
        void gunleriAzaltveGuncelle()
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand gunAzalt = new NpgsqlCommand("UPDATE musteri SET gunluk=gunluk-" + eksilecekGun + " WHERE dondurulmus=false", baglanti);
            NpgsqlCommand gunGuncelle = new NpgsqlCommand("UPDATE uygulama SET songiris='" + DateTime.Now + "'", baglanti);
            NpgsqlCommand vaktiGecti = new NpgsqlCommand("DELETE FROM musteri WHERE gunluk<='" + -silsure + "'", baglanti);
            baglanti.Open();
            gunAzalt.ExecuteNonQuery();
            gunGuncelle.ExecuteNonQuery();
            vaktiGecti.ExecuteNonQuery();
            baglanti.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void KullaniciAdiBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == (char)Keys.Space);
        }

        private void SifreBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (e.KeyChar == (char)Keys.Space);
            if (e.KeyChar == (char)Keys.Enter)
            {
                GirisTikla();
            }
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            GirisTikla();
        }

        private void bekletici_Tick(object sender, EventArgs e)
        {
            NpgsqlConnection baglanti = new NpgsqlConnection("server=localhost; port=5432; database=FitnessDB; user ID=postgres; password=postgres");
            NpgsqlCommand personelbul = new NpgsqlCommand("SELECT personelserino,adsoyad,sahip FROM personel WHERE kullaniciadi='" + KullaniciAdiBox.Text + "' AND sifre='" + SifreBox.Text + "'", baglanti);
            NpgsqlDataReader dr;
            baglanti.Open();
            dr = personelbul.ExecuteReader();
            if (dr.Read())
            {
                bekletici.Stop();
                PersonelAnaSayfa personelAnaSayfa = new PersonelAnaSayfa();
                personelAnaSayfa.lblVersiyon.Text = lblVersiyon.Text;
                personelAnaSayfa.girisYapanınSeriNumarası = (int)dr[0];
                personelAnaSayfa.lblAdSoyad.Text = dr[1].ToString();
                personelAnaSayfa.sahipmi = (bool)dr[2];
                personelAnaSayfa.SahipAdi = sa;
                personelAnaSayfa.sifremi = sifreMi;
                personelAnaSayfa.ucsaatuyarmi = UcSaat;
                personelAnaSayfa.Acilis = ac;
                personelAnaSayfa.Kapanis = kapa;
                personelAnaSayfa.silSuresi = silsure;
                personelAnaSayfa.akaktiflik = akakt;
                personelAnaSayfa.herkesidndr = hrksdndr;
                baglanti.Close();
                this.Hide();
                personelAnaSayfa.Show();
            }
            else
            {
                bekletici.Stop();
                MessageBox.Show("Kullanıcı adı veya şifre yanlış.");
                lblBekle.Visible = false;
                btnGiris.Visible = true;
                SifreBox.SelectAll();
            }
            baglanti.Close();
        }
    }
}

using System;
using System.Windows.Forms;
using Npgsql;

namespace YurtVeriTabani
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlBaglantisi bgl = new SqlBaglantisi();

        private void login_Click(object sender, EventArgs e)
        {
            try
            {
                // "Bu kullanýcý adý ve þifre var mý?" KONTROLÜ

                NpgsqlCommand komut = new NpgsqlCommand("Select * from Yoneticiler where kullaniciadi=@p1 and sifre=@p2", bgl.baglanti());

                // Parametreleri kutucuklardan alýyoruz (Güvenlik için)
                komut.Parameters.AddWithValue("@p1", kullaniciAdi.Text);
                komut.Parameters.AddWithValue("@p2", sifre.Text);


                NpgsqlDataReader dr = komut.ExecuteReader();

                if (dr.Read()) // Giriþ baþarýlýysa
                {
                    // ANA MENÜYÜ OLUÞTURMA ÝÞLEMLERÝ
                    AnaMenu fr = new AnaMenu();
                    fr.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanýcý adý veya þifre hatalý!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                bgl.baglanti().Close(); // Ýþlem bitince baðlantýyý kapatýyorum
            }
            catch (Exception ex)
            {
                MessageBox.Show("Baðlantý Hatasý: " + ex.Message);
            }
        }

        private void demo_Click(object sender, EventArgs e)
        {
            AnaMenu fr = new AnaMenu();
            fr.Show();
            this.Hide();
        }
    }
}

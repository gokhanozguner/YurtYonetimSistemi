using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace YurtVeriTabani
{
    public partial class AnaMenu : Form
    {
        string panelHeaderText = "Sakarya Üniversitesi Yurt Yönetim Sistemi";
        public AnaMenu()
        {
            InitializeComponent();
        }

        private void formPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void formuGetir(Form frm)
        {
            // Menü sayfalarının ekrana getirilmesi
            forumPanel.Controls.Clear();
            frm.TopLevel = false; // Formun "pencere" özelliğini kapatıyorum (Panelin içine girebilsin diye)
            frm.FormBorderStyle = FormBorderStyle.None; // Çerçevesini kaldırıyorum
            frm.Dock = DockStyle.Fill; // Paneli tamamen dolduracak şekilde ayarlıyorum
            forumPanel.Controls.Add(frm); // Paneli ekliyorum
            forumPanel.Tag = frm; // Tag olarak tutuyorum
            frm.Show(); // Ekrana getiriyorum
            frm.BringToFront(); // En öne getiriyorum
        }

        private void butonAnaSayfa_Click(object sender, EventArgs e)
        {
            AnaSayfa fr = new AnaSayfa();
            formuGetir(fr);
            labelHeaderText.Text = panelHeaderText + " - Ana Sayfa";

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (sideBar.Width == 60)
            {
                sideBar.Width = 220;
            }
            else
            {
                sideBar.Width = 60;
            }
            iconSet();
        }

        private void butonOgrenci_Click(object sender, EventArgs e)
        {

        }

        private void buton_izin_Click(object sender, EventArgs e)
        {

        }

        private void butonOdemeler_Click(object sender, EventArgs e)
        {

        }

        private void butonOda_ve_Yatak_Click(object sender, EventArgs e)
        {

        }

        private void butonPersonel_Click(object sender, EventArgs e)
        {

        }

        private void butonAriza_Click(object sender, EventArgs e)
        {

        }



        private void butonBolumler_Click(object sender, EventArgs e)
        {
            Bolumler fr = new Bolumler();
            labelHeaderText.Text = panelHeaderText + " - Bölümler";
            formuGetir(fr);

        }

        private void AnaMenu_Load(object sender, EventArgs e)
        {
            AnaSayfa fr = new AnaSayfa();

            formuGetir(fr);
        }

        public void iconSet()
        {
            foreach (Control ctrl in sideBar.Controls)
            {
                if (ctrl is Guna.UI2.WinForms.Guna2Button btn)
                {
                    if (sideBar.Width == 60)
                    {
                        btn.Left -= 7;
                        btn.ForeColor = Color.Transparent;

                    }
                    else
                    {
                        btn.Left += 7;
                        btn.ForeColor = Color.White;

                    }

                }

            }
        }
        private void butonOgrenci_CheckedChanged(object sender, EventArgs e)
        {
            if (butonOgrenci.Checked == true)
            {
                this.Width = 1185;
                OgrenciIslemleri fr = new OgrenciIslemleri();
                labelHeaderText.Text = panelHeaderText + " - Öğrenci İşlemleri";
                formuGetir(fr);

            }
            else
            {
                this.Width = 1185;
            }
        }

        private void butonOda_ve_Yatak_CheckedChanged(object sender, EventArgs e)
        {
            if (butonOda_ve_Yatak.Checked == true)
            {
                OdaVeYatak fr = new OdaVeYatak();
                labelHeaderText.Text = panelHeaderText + " - Oda ve Yatak İşlemleri";
                formuGetir(fr);
            }

        }

        private void buton_izin_CheckedChanged(object sender, EventArgs e)
        {
            if (buton_izin.Checked == true)
            {
                Izin fr = new Izin();
                labelHeaderText.Text = panelHeaderText + " - İzin İşlemleri";
                formuGetir(fr);
            }
        }

        private void butonOdemeler_CheckedChanged(object sender, EventArgs e)
        {
            if (butonOdemeler.Checked == true)
            {
                Odemeleer fr = new Odemeleer();
                labelHeaderText.Text = panelHeaderText + " - Ödeme İşlemleri";
                formuGetir(fr);
            }
        }

        private void butonAriza_CheckedChanged(object sender, EventArgs e)
        {
            if (butonAriza.Checked == true)
            {
                Arizalar fr = new Arizalar();
                labelHeaderText.Text = panelHeaderText + " - Arıza İşlemleri";
                formuGetir(fr);
            }
        }
    }
}


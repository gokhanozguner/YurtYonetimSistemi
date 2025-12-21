using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace YurtVeriTabani
{
    public partial class Arizalar : Form
    {
        int? seciliArizaId = null;
        int? seciliOgrenciId = null;

        SqlBaglantisi bgl = new SqlBaglantisi();
        public Arizalar()
        {
            InitializeComponent();
        }

        void OgrencileriListele()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT k.kisi_id, k.ad || ' ' || k.soyad AS ogrenci " +
                "FROM ogrenci o JOIN kisi k ON k.kisi_id=o.kisi_id",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            dgvOgrenciler.DataSource = dt;
            dgvOgrenciler.Columns["kisi_id"].Visible = false;

            bgl.baglanti().Close();
        }

        void ArizaTipleriniDoldur()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT ariza_tipi_id, ad FROM ariza_tipi",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            cmbArizaTipi.DataSource = dt;
            cmbArizaTipi.DisplayMember = "ad";
            cmbArizaTipi.ValueMember = "ariza_tipi_id";

            bgl.baglanti().Close();
        }

        void ArizalariListele()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT a.ariza_id, " +
                "k.ad || ' ' || k.soyad AS ogrenci, " +
                "t.ad AS ariza_tipi, " +
                "a.aciklama, a.durum, " +
                "a.bildirim_tarihi, a.kapanis_tarihi " +
                "FROM ariza a " +
                "JOIN kisi k ON k.kisi_id=a.ogrenci_kisi_id " +
                "JOIN ariza_tipi t ON t.ariza_tipi_id=a.ariza_tipi_id " +
                "ORDER BY a.bildirim_tarihi DESC",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            dgvArizalar.DataSource = dt;
            dgvArizalar.Columns["ariza_id"].Visible = false;

            bgl.baglanti().Close();
        }

        private void btnArizaEkle_Click(object sender, EventArgs e)
        {
            if (seciliOgrenciId == null)
            {
                MessageBox.Show("Öğrenci seçiniz");
                return;
            }

            NpgsqlCommand cmd = new NpgsqlCommand(
                "INSERT INTO ariza " +
                "(ogrenci_kisi_id, ariza_tipi_id, aciklama, durum) " +
                "VALUES (@p1,@p2,@p3,'Açık')",
                bgl.baglanti()
            );

            cmd.Parameters.AddWithValue("@p1", seciliOgrenciId);
            cmd.Parameters.AddWithValue("@p2", cmbArizaTipi.SelectedValue);
            cmd.Parameters.AddWithValue("@p3", txtAciklama.Text);

            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Arıza bildirildi");
            ArizalariListele();
            dgvArizalar.ClearSelection();
        }

        private void dgvArizalar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            seciliArizaId = Convert.ToInt32(dgvArizalar.CurrentRow.Cells["ariza_id"].Value);

            txtAciklama.Text =
                dgvArizalar.CurrentRow.Cells["aciklama"].Value.ToString();

            cmbDurum.Text =
                dgvArizalar.CurrentRow.Cells["durum"].Value.ToString();
        }

        private void btnOdemeGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliArizaId == null)
            {
                MessageBox.Show("Arıza seçiniz");
                return;
            }

            string sql =
                cmbDurum.Text == "Çözüldü"
                ? "UPDATE ariza SET durum='Çözüldü', kapanis_tarihi=now() WHERE ariza_id=@p1"
                : "UPDATE ariza SET durum='Açık', kapanis_tarihi=NULL WHERE ariza_id=@p1";

            NpgsqlCommand cmd = new NpgsqlCommand(sql, bgl.baglanti());
            cmd.Parameters.AddWithValue("@p1", seciliArizaId);

            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Arıza güncellendi");
            ArizalariListele();
        }

        private void Arizalar_Load(object sender, EventArgs e)
        {
            ArizalariListele();
            OgrencileriListele();
            ArizaTipleriniDoldur();
            dgvOgrenciler.ScrollBars = ScrollBars.Both;
            dgvArizalar.ScrollBars = ScrollBars.Both;
        }

        private void dgvOgrenciler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            seciliOgrenciId =
                Convert.ToInt32(dgvOgrenciler.Rows[e.RowIndex].Cells["kisi_id"].Value);
        }
    }
}

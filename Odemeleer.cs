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
    public partial class Odemeleer : Form
    {
        int? seciliOgrenciId = null;
        int? seciliOdemeId = null;
        SqlBaglantisi bgl = new SqlBaglantisi();
        public Odemeleer()
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

        void OdemeleriListele()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT o.odeme_id, " +
                "k.ad || ' ' || k.soyad AS ogrenci, " +
                "o.tutar, o.odeme_tipi, o.odeme_tarihi " +
                "FROM odeme o " +
                "JOIN kisi k ON k.kisi_id = o.ogrenci_kisi_id " +
                "ORDER BY o.odeme_tarihi DESC",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            dgvOdemeler.DataSource = dt;
            dgvOdemeler.Columns["odeme_id"].Visible = false;

            bgl.baglanti().Close();
        }

        private void dgvOgrenciler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            seciliOgrenciId = Convert.ToInt32(dgvOgrenciler.CurrentRow.Cells["kisi_id"].Value);
        }

        private void btnOdemeEkle_Click(object sender, EventArgs e)
        {
            if (seciliOgrenciId == null)
            {
                MessageBox.Show("Öğrenci seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "CALL odeme_ekle(@p1,@p2,@p3,@p4)",
                    bgl.baglanti()
                );

                cmd.Parameters.AddWithValue("@p1", seciliOgrenciId);
                cmd.Parameters.AddWithValue("@p2", nudTutar.Value);
                cmd.Parameters.AddWithValue("@p3", cmbOdemeTuru.Text);

                cmd.Parameters.Add("@p4", NpgsqlTypes.NpgsqlDbType.Date)
                   .Value = dtOdemeTarihi.Value;

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Ödeme eklendi");
                OdemeleriListele();
                dgvOdemeler.ClearSelection();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void dgvOdemeler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            seciliOdemeId =
        Convert.ToInt32(dgvOdemeler.CurrentRow.Cells["odeme_id"].Value);

            nudTutar.Value =
                Convert.ToDecimal(dgvOdemeler.CurrentRow.Cells["tutar"].Value);

            cmbOdemeTuru.Text =
                dgvOdemeler.CurrentRow.Cells["odeme_tipi"].Value.ToString();

            dtOdemeTarihi.Value =
                ((DateOnly)dgvOdemeler.CurrentRow.Cells["odeme_tarihi"].Value)
                .ToDateTime(TimeOnly.MinValue);
        }

        private void btnOdemeGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliOdemeId == null) { 
                MessageBox.Show("Ödeme seçiniz!");
                return; }

            NpgsqlCommand cmd = new NpgsqlCommand(
                "UPDATE odeme SET " +
                "tutar=@p1, odeme_tipi=@p2, odeme_tarihi=@p3 " +
                "WHERE odeme_id=@p4",
                bgl.baglanti()
            );

            cmd.Parameters.AddWithValue("@p1", nudTutar.Value);
            cmd.Parameters.AddWithValue("@p2", cmbOdemeTuru.Text);

            cmd.Parameters.Add("@p3", NpgsqlTypes.NpgsqlDbType.Date)
               .Value = dtOdemeTarihi.Value;

            cmd.Parameters.AddWithValue("@p4", seciliOdemeId);

            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Ödeme güncellendi");
            OdemeleriListele();
            dgvOdemeler.ClearSelection();
            seciliOdemeId = null;
        }

        private void btnOdemeSil_Click(object sender, EventArgs e)
        {
            if (seciliOdemeId == null)
            {
                MessageBox.Show("Ödeme seçiniz!");
                return;
            }

            NpgsqlCommand cmd = new NpgsqlCommand(
                "DELETE FROM odeme WHERE odeme_id=@p1",
                bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@p1", seciliOdemeId);

            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();

            MessageBox.Show("Ödeme silindi");
            OdemeleriListele();
            seciliOdemeId = null;
        }

        private void Odemeleer_Load(object sender, EventArgs e)
        {
            OgrencileriListele();
            OdemeleriListele();

            dtOdemeTarihi.Value = DateTime.Today;

            cmbOdemeTuru.Items.AddRange(
                new string[] { "Aidat", "Depozito", "Ceza", "Diğer" }
            );
        }
    }
}

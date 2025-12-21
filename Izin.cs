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
    public partial class Izin : Form
    {
        int? seciliOgrenciId = null;
        int? seciliIzinId = null;
        SqlBaglantisi bgl = new SqlBaglantisi();
        public Izin()
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

        void IzinleriListele()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
            "SELECT i.izin_id, " +
            "k.ad || ' ' || k.soyad AS ogrenci, " +
            "i.baslangic_tarihi, " +
            "i.bitis_tarihi, " +
            "i.izin_nedeni " +
            "FROM izin i " +
            "JOIN kisi k ON k.kisi_id = i.ogrenci_kisi_id " +
            "ORDER BY i.baslangic_tarihi DESC",
            bgl.baglanti()
        );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            dgvIzinler.DataSource = dt;
            dgvIzinler.Columns["izin_id"].Visible = false;

            bgl.baglanti().Close();
        }

        private void dgvOgrenciler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            seciliOgrenciId = Convert.ToInt32(dgvOgrenciler.CurrentRow.Cells["kisi_id"].Value);


        }

        private void btnIzinEkle_Click(object sender, EventArgs e)
        {
            if (seciliOgrenciId == null) return;


            NpgsqlCommand cmd = new NpgsqlCommand(
            "CALL izin_ekle(@p1,@p2,@p3,@p4)",
            bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@p1", seciliOgrenciId);
            cmd.Parameters.Add("@p2", NpgsqlTypes.NpgsqlDbType.Date)
   .Value = dtBaslangic.Value.Date;

            cmd.Parameters.Add("@p3", NpgsqlTypes.NpgsqlDbType.Date)
               .Value = dtBitis.Value.Date;
            cmd.Parameters.AddWithValue("@p4", txtIzinSebebi.Text);


            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();


            IzinleriListele();
        }

        private void dgvIzinler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvIzinler.CurrentRow == null) return;

            seciliIzinId =
                Convert.ToInt32(dgvIzinler.CurrentRow.Cells["izin_id"].Value);

            // Tarihleri DateTimePicker'a bas
            var bas = dgvIzinler.CurrentRow.Cells["baslangic_tarihi"].Value;
            var bit = dgvIzinler.CurrentRow.Cells["bitis_tarihi"].Value;

            dtBaslangic.Value =
                ((DateOnly)bas).ToDateTime(TimeOnly.MinValue);

            dtBitis.Value =
                ((DateOnly)bit).ToDateTime(TimeOnly.MinValue);

            // İzin nedeni
            txtIzinSebebi.Text =
                dgvIzinler.CurrentRow.Cells["izin_nedeni"].Value.ToString();
        }

        private void btnIzinGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliIzinId == null)
            {
                MessageBox.Show("Lütfen güncellenecek izni seçiniz");
                return;
            }

            if (dtBitis.Value.Date < dtBaslangic.Value.Date)
            {
                MessageBox.Show("Bitiş tarihi başlangıçtan küçük olamaz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "UPDATE izin SET " +
                    "baslangic_tarihi=@p1, " +
                    "bitis_tarihi=@p2, " +
                    "izin_nedeni=@p3 " +
                    "WHERE izin_id=@p4",
                    bgl.baglanti()
                );

                cmd.Parameters.Add("@p1", NpgsqlTypes.NpgsqlDbType.Date)
                   .Value = dtBaslangic.Value;

                cmd.Parameters.Add("@p2", NpgsqlTypes.NpgsqlDbType.Date)
                   .Value = dtBitis.Value;

                cmd.Parameters.AddWithValue("@p3", txtIzinSebebi.Text);
                cmd.Parameters.AddWithValue("@p4", seciliIzinId);

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("İzin güncellendi");

                IzinleriListele();
                dgvIzinler.ClearSelection();
                seciliIzinId = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnIzinSil_Click(object sender, EventArgs e)
        {
            if (seciliIzinId == null)
            {
                MessageBox.Show("Lütfen silinecek izni seçiniz!");
                return;
            }


            NpgsqlCommand cmd = new NpgsqlCommand(
            "DELETE FROM izin WHERE izin_id=@p1",
            bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@p1", seciliIzinId);


            cmd.ExecuteNonQuery();
            bgl.baglanti().Close();


            IzinleriListele();
            seciliIzinId = null;
            dgvIzinler.ClearSelection();
        }

        private void Izin_Load(object sender, EventArgs e)
        {
            OgrencileriListele();
            IzinleriListele();
            dtBaslangic.Value = DateTime.Today;
            dtBitis.Value = DateTime.Today;
        }

        private void dtBaslangic_ValueChanged(object sender, EventArgs e)
        {
            

        }
    }
}

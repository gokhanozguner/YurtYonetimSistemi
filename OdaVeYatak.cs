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
    public partial class OdaVeYatak : Form
    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        // Global Değişkenler
        int? seciliOdaId = null;
        int? seciliYatakId = null;
        public OdaVeYatak()
        {
            InitializeComponent();
        }


        void OdaDoldur()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT oda_id, kat, oda_no FROM oda ORDER BY oda_no",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            dataOdalar.DataSource = dt;
            dataOdalar.Columns["oda_id"].Visible = false;

            bgl.baglanti().Close();
        }

        void BosYataklariGetir()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT yatak_id, yatak_no FROM yatak WHERE oda_id=@oda AND dolu = false",
                bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@oda", comboOda.SelectedValue);

            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            da.Fill(dt);

            comboYatak.DataSource = dt;
            comboYatak.DisplayMember = "yatak_no";
            comboYatak.ValueMember = "yatak_id";
        }




        void OgrenciDoldur()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT k.kisi_id, k.ad || ' ' || k.soyad AS adsoyad " +
                "FROM ogrenci o JOIN kisi k ON k.kisi_id=o.kisi_id",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            comboOgrenci.DataSource = dt;
            comboOgrenci.DisplayMember = "adsoyad";
            comboOgrenci.ValueMember = "kisi_id";
        }


        void YataklariGetir()
        {
            if (seciliOdaId == null) return;

            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT yatak_id, yatak_no FROM yatak WHERE oda_id=@p1 ORDER BY yatak_no",
                bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@p1", seciliOdaId);

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            comboYatak.DataSource = dt;
            comboYatak.DisplayMember = "yatak_no";
            comboYatak.ValueMember = "yatak_id";

            bgl.baglanti().Close();
        }

        void YataksizOgrencileriGetir()
        {
            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT k.kisi_id, k.ad || ' ' || k.soyad AS adsoyad " +
                "FROM ogrenci o " +
                "JOIN kisi k ON k.kisi_id = o.kisi_id " +
                "LEFT JOIN ogrenci_oda oo ON oo.ogrenci_kisi_id = o.kisi_id " +
                "WHERE oo.ogrenci_kisi_id IS NULL " +
                "ORDER BY adsoyad",
                bgl.baglanti()
            );

            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(cmd).Fill(dt);

            comboOgrenci.DataSource = dt;
            comboOgrenci.DisplayMember = "adsoyad";
            comboOgrenci.ValueMember = "kisi_id";

            bgl.baglanti().Close();
        }


        void SeciliYataktaKimVar()
        {
            if (seciliYatakId == null)
            {
                if (labelOgrenci != null) labelOgrenci.Text = "-";
                return;
            }

            NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT k.ad || ' ' || k.soyad AS adsoyad " +
                "FROM ogrenci_oda oo " +
                "JOIN kisi k ON k.kisi_id = oo.ogrenci_kisi_id " +
                "WHERE oo.yatak_id = @p1",
                bgl.baglanti()
            );
            cmd.Parameters.AddWithValue("@p1", seciliYatakId);

            object sonuc = cmd.ExecuteScalar();
            bgl.baglanti().Close();

            string adSoyad = (sonuc == null) ? "Boş" : sonuc.ToString();

            if (labelOgrenci != null)
                labelOgrenci.Text = adSoyad;
        }



        private void comboOda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboOda.SelectedValue != null)
            {
                BosYataklariGetir();
            }
        }



        private void OdaVeYatak_Load(object sender, EventArgs e)
        {
            OdaDoldur();
            OgrenciDoldur();
            YataksizOgrencileriGetir();
        }



        private void butonOdaEkle_Click(object sender, EventArgs e)
        {
            if (textOdaNo.Text == "")
            {
                MessageBox.Show("Oda numarası giriniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO oda (oda_no, kat) VALUES (@oda, @kat)",
                    bgl.baglanti()
                );
                cmd.Parameters.AddWithValue("@oda", textOdaNo.Text);
                cmd.Parameters.AddWithValue("@kat", int.Parse(textKat.Text));

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Oda eklendi");
                OdaDoldur();
                textOdaNo.Clear();
                textKat.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void dataOdalar_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            seciliOdaId = Convert.ToInt32(dataOdalar.CurrentRow.Cells["oda_id"].Value);
            textOdaNo.Text = dataOdalar.CurrentRow.Cells["oda_no"].Value.ToString();
            textKat.Text = dataOdalar.CurrentRow.Cells["kat"].Value.ToString();
            YataklariGetir();
        }

        private void butonOdaGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliOdaId == null)
            {
                MessageBox.Show("Lütfen bir oda seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "UPDATE oda SET oda_no=@p1, kat=@p2 WHERE oda_id=@p3",
                    bgl.baglanti()
                );

                cmd.Parameters.AddWithValue("@p1", textOdaNo.Text);
                cmd.Parameters.AddWithValue("@p2", int.Parse(textKat.Text));
                cmd.Parameters.AddWithValue("@p3", seciliOdaId);

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Oda güncellendi");

                OdaDoldur();
                textOdaNo.Clear();
                textKat.Clear();
                seciliOdaId = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void butonOdaSil_Click(object sender, EventArgs e)
        {
            if (seciliOdaId == null)
            {
                MessageBox.Show("Lütfen bir oda seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "DELETE FROM oda WHERE oda_id=@p1",
                    bgl.baglanti()
                );

                cmd.Parameters.AddWithValue("@p1", seciliOdaId);
                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Oda silindi");

                OdaDoldur();
                textOdaNo.Clear();
                textKat.Clear();
                seciliOdaId = null;
            }
            catch
            {
                MessageBox.Show(
                    "Bu odaya bağlı yatak(lar) olduğu için silinemez",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void comboYatak_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboYatak.SelectedValue != null)
                seciliYatakId = Convert.ToInt32(comboYatak.SelectedValue);
            SeciliYataktaKimVar();
        }

        private void butonYatakEkle_Click(object sender, EventArgs e)
        {
            if (seciliOdaId == null)
            {
                MessageBox.Show("Lütfen önce bir oda seçiniz");
                return;
            }

            try
            {
                // O odadaki max yatak no
                NpgsqlCommand cmdNo = new NpgsqlCommand(
                    "SELECT COALESCE(MAX(yatak_no::int),0) FROM yatak WHERE oda_id=@p1",
                    bgl.baglanti()
                );
                cmdNo.Parameters.AddWithValue("@p1", seciliOdaId);

                int yeniYatakNo = Convert.ToInt32(cmdNo.ExecuteScalar()) + 1;
                bgl.baglanti().Close();

                // Yatak ekle
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "INSERT INTO yatak (oda_id, yatak_no, dolu) VALUES (@p1,@p2,false)",
                    bgl.baglanti()
                );
                cmd.Parameters.AddWithValue("@p1", seciliOdaId);
                cmd.Parameters.AddWithValue("@p2", yeniYatakNo);

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Yatak eklendi");

                YataklariGetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void butonYatakSil_Click(object sender, EventArgs e)
        {
            if (seciliYatakId == null)
            {
                MessageBox.Show("Lütfen bir yatak seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "DELETE FROM yatak WHERE yatak_id=@p1",
                    bgl.baglanti()
                );
                cmd.Parameters.AddWithValue("@p1", seciliYatakId);

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Yatak silindi");

                YataklariGetir();
                seciliYatakId = null;
            }
            catch
            {
                MessageBox.Show(
                    "Bu yatak dolu olduğu için silinemez.\nÖnce öğrenciyi yataktan çıkarın.",
                    "Uyarı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void butonOgrenciAta_Click(object sender, EventArgs e)
        {
            if (seciliOdaId == null || seciliYatakId == null)
            {
                MessageBox.Show("Lütfen önce oda ve yatak seçiniz");
                return;
            }

            if (comboOgrenci.SelectedValue == null)
            {
                MessageBox.Show("Lütfen yataksız bir öğrenci seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "CALL ogrenci_oda_ata(@p1,@p2)",
                    bgl.baglanti()
                );
                cmd.Parameters.AddWithValue("@p1", comboOgrenci.SelectedValue);
                cmd.Parameters.AddWithValue("@p2", seciliYatakId);

                cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Öğrenci yatağa atandı");

                // Tazele
                YataklariGetir();             // yatak listesi (oda seçili)
                YataksizOgrencileriGetir();   // combo yenilensin
                SeciliYataktaKimVar();        // label güncellensin
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void butonYatakOgrenciSil_Click(object sender, EventArgs e)
        {
            if (seciliYatakId == null)
            {
                MessageBox.Show("Lütfen bir yatak seçiniz");
                return;
            }

            try
            {
                NpgsqlCommand cmd = new NpgsqlCommand(
                    "DELETE FROM ogrenci_oda WHERE yatak_id=@p1",
                    bgl.baglanti()
                );
                cmd.Parameters.AddWithValue("@p1", seciliYatakId);

                int etkilenen = cmd.ExecuteNonQuery();
                bgl.baglanti().Close();

                if (etkilenen == 0)
                {
                    MessageBox.Show("Bu yatakta öğrenci yok (zaten boş).");
                    return;
                }

                MessageBox.Show("Öğrenci yataktan çıkarıldı");

                // Tazele
                YataklariGetir();
                YataksizOgrencileriGetir();
                SeciliYataktaKimVar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}

using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel; // SortOrder için gerekli

namespace YurtVeriTabani
{
    public partial class OgrenciIslemleri : Form
    {
        SqlBaglantisi bgl = new SqlBaglantisi();

        // Seçilen satırın ID'sini tutmak için nullable int
        int? id = null;

        public OgrenciIslemleri()
        {
            InitializeComponent();
        }

        private void OgrenciIslemleri_Load(object sender, EventArgs e)
        {
            BolumCek();
            OgrenciListele();
        }

        void OgrenciListele()
        {
            // 1. Mevcut sıralama bilgisini sakla
            DataGridViewColumn eskiKolon = dataGrid.SortedColumn;
            SortOrder eskiYon = dataGrid.SortOrder;

            DataTable dt = new DataTable();

            // using bloğu iş bitince bağlantıyı otomatik kapatır
            using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(
                @"SELECT 
                    k.kisi_id,
                    k.ad,
                    k.soyad,
                    k.tc_no,
                    k.telefon,
                    k.email,
                    k.dogum_tarihi,
                    k.kan_grubu,
                    o.ogrenci_no,
                    b.bolum_adi
                FROM ogrenci o
                JOIN kisi k ON k.kisi_id = o.kisi_id
                JOIN bolumler b ON b.bolum_id = o.bolum_id
                ORDER BY k.ad", bgl.baglanti()))
            {
                da.Fill(dt);
            }

            dataGrid.DataSource = dt;
            // 1. Başlıkları Güzelleştirme
            dataGrid.Columns["ad"].HeaderText = "Ad";
            dataGrid.Columns["soyad"].HeaderText = "Soyad";
            dataGrid.Columns["tc_no"].HeaderText = "T.C";
            dataGrid.Columns["ogrenci_no"].HeaderText = "Öğrenci No";
            dataGrid.Columns["telefon"].HeaderText = "Telefon";
            dataGrid.Columns["email"].HeaderText = "E-Posta";
            dataGrid.Columns["dogum_tarihi"].HeaderText = "Doğum Tarihi";
            dataGrid.Columns["kan_grubu"].HeaderText = "Kan Grubu";
            dataGrid.Columns["bolum_adi"].HeaderText = "Bölüm";

            // 2. ID Sütununu Gizleme (Kullanıcı görmesin ama arkada dursun)
            // ID genelde kullanıcı için anlamsızdır, kalabalık yapmasın diye gizleriz.
            dataGrid.Columns["kisi_id"].Visible = false;

            // 3. (İsteğe Bağlı) Sütun Genişlikleri
            // Mesela Bölüm adı uzun olabilir, ona daha çok yer verelim
            dataGrid.Columns["ad"].Width = 100;
            dataGrid.Columns["soyad"].Width = 100;
            dataGrid.Columns["tc_no"].Width = 100;
            dataGrid.Columns["ogrenci_no"].Width = 100;
            dataGrid.Columns["telefon"].Width = 120;
            dataGrid.Columns["email"].Width = 150;
            dataGrid.Columns["dogum_tarihi"].Width = 100;
            dataGrid.Columns["kan_grubu"].Width = 80;
            dataGrid.Columns["bolum_adi"].Width = 150;


            // 2. Eski sıralamayı geri yükle (Kullanıcı deneyimi için harika detay)
            if (eskiKolon != null && eskiYon != SortOrder.None)
            {
                var yeniGriddekiKolon = dataGrid.Columns[eskiKolon.Name];
                if (yeniGriddekiKolon != null)
                {
                    ListSortDirection yon = (eskiYon == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    dataGrid.Sort(yeniGriddekiKolon, yon);
                }
            }
        }

        void BolumCek()
        {
            DataTable dt = new DataTable();
            using (NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT bolum_id, bolum_adi FROM bolumler ORDER BY bolum_adi", bgl.baglanti()))
            {
                da.Fill(dt);
            }

            comboBolumler.DisplayMember = "bolum_adi";
            comboBolumler.ValueMember = "bolum_id";
            comboBolumler.DataSource = dt;
            comboBolumler.SelectedIndex = -1; // Açılışta boş gelsin
        }

        void Temizle()
        {
            textAdd.Clear();
            textSoyadd.Clear();
            textTCKNN.Clear();
            textTelefonn.Clear();
            textMaill.Clear();
            textOgrenciNOO.Clear();
            comboKan.SelectedIndex = -1;
            comboBolumler.SelectedIndex = -1;
            dateTime.Value = new DateTime(2000, 1, 1);

            // KRİTİK NOKTA: Hafızadaki ID'yi sıfırlıyoruz.
            id = null;
            dataGrid.ClearSelection();
        }

        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Başlıklara tıklanınca hata vermemesi için
            if (e.RowIndex < 0) return;

            try
            {
                // Null kontrolü yaparak değerleri alıyoruz
                if (dataGrid.CurrentRow.Cells["kisi_id"].Value != DBNull.Value)
                {
                    id = Convert.ToInt32(dataGrid.CurrentRow.Cells["kisi_id"].Value);
                    textAdd.Text = dataGrid.CurrentRow.Cells["ad"].Value.ToString();
                    textSoyadd.Text = dataGrid.CurrentRow.Cells["soyad"].Value.ToString();
                    textTCKNN.Text = dataGrid.CurrentRow.Cells["tc_no"].Value.ToString();
                    textOgrenciNOO.Text = dataGrid.CurrentRow.Cells["ogrenci_no"].Value.ToString();

                    // Telefon ve Mail gridde yoksa veritabanından çekmek gerekebilir
                    // Şimdilik gridde ne varsa onu doldurur.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Seçim hatası: " + ex.Message);
            }
        }

        private void butonOgrenciEkle_Click(object sender, EventArgs e)
        {
            timer1.Start();
            if (string.IsNullOrWhiteSpace(textAdd.Text) ||
                string.IsNullOrWhiteSpace(textSoyadd.Text) ||
                string.IsNullOrWhiteSpace(textTCKNN.Text) ||
                comboKan.SelectedItem == null ||
                comboBolumler.SelectedValue == null)
            {
                MessageBox.Show("Lütfen zorunlu alanları, Bölümü ve Kan Grubunu seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar.Value = 0;
                return;
            }

            // 'using' bloğu: Conn ve Transaction otomatik Dispose edilir.
            using (NpgsqlConnection conn = bgl.baglanti())
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                using (NpgsqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. KİŞİ EKLE
                        NpgsqlCommand cmdKisi = new NpgsqlCommand(
                            "INSERT INTO kisi (ad, soyad, tc_no, telefon, email, dogum_tarihi, kan_grubu) " +
                            "VALUES (@ad, @soyad, @tc, @tel, @mail, @dogum, @kan) RETURNING kisi_id", conn);

                        cmdKisi.Transaction = transaction;
                        cmdKisi.Parameters.AddWithValue("@ad", textAdd.Text);
                        cmdKisi.Parameters.AddWithValue("@soyad", textSoyadd.Text);
                        cmdKisi.Parameters.AddWithValue("@tc", textTCKNN.Text);
                        cmdKisi.Parameters.AddWithValue("@tel", textTelefonn.Text);
                        cmdKisi.Parameters.AddWithValue("@mail", textMaill.Text);
                        cmdKisi.Parameters.AddWithValue("@dogum", dateTime.Value.Date);
                        cmdKisi.Parameters.AddWithValue("@kan", comboKan.SelectedItem.ToString());

                        int kisiId = Convert.ToInt32(cmdKisi.ExecuteScalar());

                        // 2. ÖĞRENCİ EKLE
                        NpgsqlCommand cmdOgr = new NpgsqlCommand(
                            "INSERT INTO ogrenci (kisi_id, ogrenci_no, bolum_id) VALUES (@kisi_id, @ogr_no, @bolum_id)", conn);

                        cmdOgr.Transaction = transaction;
                        cmdOgr.Parameters.AddWithValue("@kisi_id", kisiId);
                        cmdOgr.Parameters.AddWithValue("@ogr_no", textOgrenciNOO.Text);
                        cmdOgr.Parameters.AddWithValue("@bolum_id", (int)comboBolumler.SelectedValue);

                        cmdOgr.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Öğrenci başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        progressBar.Value = 0;

                        OgrenciListele();
                        Temizle();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressBar.Value = 0;
                    }
                }
            }
        }

        private void butonOgrenciSil_Click(object sender, EventArgs e)
        {
            timer1.Start();
            if (id == null || id == 0)
            {
                MessageBox.Show("Lütfen listeden silinecek öğrenciyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar.Value = 0;
                return;
            }

            if (MessageBox.Show("Bu öğrenciyi silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                progressBar.Value = 0;
                return;
            }
            using (NpgsqlConnection conn = bgl.baglanti())
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                using (NpgsqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Önce Çocuk Tablo (Ogrenci)
                        NpgsqlCommand cmdOgr = new NpgsqlCommand("DELETE FROM ogrenci WHERE kisi_id = @id", conn);
                        cmdOgr.Transaction = transaction;
                        cmdOgr.Parameters.AddWithValue("@id", id);
                        cmdOgr.ExecuteNonQuery();

                        // Sonra Ana Tablo (Kisi)
                        NpgsqlCommand cmdKisi = new NpgsqlCommand("DELETE FROM kisi WHERE kisi_id = @id", conn);
                        cmdKisi.Transaction = transaction;
                        cmdKisi.Parameters.AddWithValue("@id", id);
                        cmdKisi.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Kayıt silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        progressBar.Value = 0;

                        OgrenciListele();
                        Temizle();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Silme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressBar.Value = 0;
                    }
                }
            }
        }

        private void butonOgrenciGuncelle_Click(object sender, EventArgs e)
        {
            timer1.Start();
            if (id == null || id == 0)
            {
                MessageBox.Show("Lütfen güncellenecek öğrenciyi seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar.Value = 0;
                return;
            }

            if (string.IsNullOrWhiteSpace(textAdd.Text) || comboKan.SelectedItem == null || comboBolumler.SelectedValue == null)
            {
                MessageBox.Show("Zorunlu alanları doldurunuz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                progressBar.Value = 0;
                return;
            }

            using (NpgsqlConnection conn = bgl.baglanti())
            {
                if (conn.State != ConnectionState.Open) conn.Open();

                using (NpgsqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // KİŞİ GÜNCELLE
                        NpgsqlCommand cmdKisi = new NpgsqlCommand(
                            "UPDATE kisi SET ad=@ad, soyad=@soyad, telefon=@tel, email=@mail, kan_grubu=@kan, dogum_tarihi=@dogum WHERE kisi_id=@id", conn);

                        cmdKisi.Transaction = transaction;
                        cmdKisi.Parameters.AddWithValue("@ad", textAdd.Text);
                        cmdKisi.Parameters.AddWithValue("@soyad", textSoyadd.Text);
                        cmdKisi.Parameters.AddWithValue("@tel", textTelefonn.Text);
                        cmdKisi.Parameters.AddWithValue("@mail", textMaill.Text);
                        cmdKisi.Parameters.AddWithValue("@kan", comboKan.SelectedItem.ToString());
                        cmdKisi.Parameters.AddWithValue("@dogum", dateTime.Value.Date);
                        cmdKisi.Parameters.AddWithValue("@id", id);
                        cmdKisi.ExecuteNonQuery();

                        // ÖĞRENCİ GÜNCELLE
                        NpgsqlCommand cmdOgr = new NpgsqlCommand(
                            "UPDATE ogrenci SET ogrenci_no=@no, bolum_id=@bolum WHERE kisi_id=@id", conn);

                        cmdOgr.Transaction = transaction;
                        cmdOgr.Parameters.AddWithValue("@no", textOgrenciNOO.Text);
                        cmdOgr.Parameters.AddWithValue("@bolum", (int)comboBolumler.SelectedValue);
                        cmdOgr.Parameters.AddWithValue("@id", id);
                        cmdOgr.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Güncelleme başarılı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        progressBar.Value = 0;

                        OgrenciListele();
                        Temizle(); // ID burada null oluyor
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Güncelleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        progressBar.Value = 0;
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar.Value < 100)
            {
                progressBar.Value += 5;
            }
            else
            {
                timer1.Stop();
            }
        }
    }
}
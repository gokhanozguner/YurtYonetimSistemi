using Guna.UI2.WinForms;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using YurtVeriTabani;



namespace YurtVeriTabani
{

    public partial class Bolumler : Form


    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        String id, bolumAd;

        public Bolumler()
        {
            InitializeComponent();
        }

        void BolumListele()
        {
            // Listelemeden önceki sıralama bilgisini kaydediyorum
            DataGridViewColumn eskiKolon = dataGrid.SortedColumn;
            SortOrder eskiYon = dataGrid.SortOrder;


            // Tabloyu temizleyip yeni veriyi çekiyorum
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter($"Select * From Bolumler", bgl.baglanti());
            da.Fill(dt);
            dataGrid.DataSource = dt;

            if (eskiKolon != null && eskiYon != SortOrder.None)
            {
                // Eski kolonun adından yeni griddeki karşılığını buluyoruz
                // (Çünkü DataSource değişince eski kolon nesnesi silinmiş olabilir)
                var yeniGriddekiKolon = dataGrid.Columns[eskiKolon.Name];

                if (yeniGriddekiKolon != null)
                {
                    ListSortDirection yon = (eskiYon == SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;

                    dataGrid.Sort(yeniGriddekiKolon, yon); 
                    // hafızaya attığımız sıralamayı tekrar uyguluyoruz
                }
            }
        }




        private void butonEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBolumler.Text == "")
                {
                    MessageBox.Show("Lütfen Bölüm Adını Giriniz", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    NpgsqlCommand komut = new NpgsqlCommand("insert into Bolumler (bolum_adi) values (@p1)", bgl.baglanti());
                    komut.Parameters.AddWithValue("@p1", textBolumler.Text);
                    komut.ExecuteNonQuery();
                    bgl.baglanti().Close();
                    MessageBox.Show("Bölüm Eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BolumListele();
                    textBolumler.Clear();
                    id = null;
                    bolumAd = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butonGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                int seciliIndex = dataGrid.SelectedCells[0].RowIndex;
                NpgsqlCommand komut = new NpgsqlCommand("update Bolumler set bolum_adi=@p1 where bolum_id=@p2", bgl.baglanti());
                komut.Parameters.AddWithValue("@p1", textBolumler.Text);
                komut.Parameters.AddWithValue("@p2", int.Parse(id));
                komut.ExecuteNonQuery();
                bgl.baglanti().Close();

                MessageBox.Show("Bölüm Güncellendi!");
                string yon = "";
                string tur = "";
                if (dataGrid.SortOrder == SortOrder.Ascending)
                {
                    yon = "Asc";
                }
                else
                {
                    yon = "Desc";
                }

                if(dataGrid.SortedColumn != null)
                {
                    tur = dataGrid.SortedColumn.HeaderText;
                }
                else
                {
                    tur = "Bolumid";
                }

                BolumListele();
                dataGrid.ClearSelection();
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    // O satırdaki ID, bizim güncellediğimiz ID'ye eşit mi?
                    // (Cells[0] senin ID sütunun olduğu için 0 yazdık)
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == id)
                    {
                        // HAH BULDUK!
                        row.Selected = true; // O satırı seç

                        // Oraya otomatik odaklan (Scroll yap)
                        dataGrid.FirstDisplayedScrollingRowIndex = row.Index;

                        // Aradığımızı bulduk, döngüyü bitir, boşuna arama artık
                        break;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen güncellenecek satırı seçiniz.");
            }
        }

        private void butonSil_Click(object sender, EventArgs e)
        {
            try
            {
                // Emin misin diye soralım
                if (MessageBox.Show("Silmek istediğinize emin misiniz?", "Uyarı", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    NpgsqlCommand komut = new NpgsqlCommand("delete from Bolumler where bolum_id=@p1", bgl.baglanti());
                    komut.Parameters.AddWithValue("@p1", int.Parse(id));
                    komut.ExecuteNonQuery();
                    bgl.baglanti().Close();

                    MessageBox.Show("Bölüm Silindi!");
                    BolumListele();
                    textBolumler.Text = "";
                    dataGrid.ClearSelection();
                    id = null;
                    bolumAd = null;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen silinecek satırı seçiniz.", "UYARI");
            }
        }

        private void dataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Tıklanan satırın indeksini al
                int secilen = e.RowIndex;

                // Verileri hafızaya (Global değişkenlere) al
                id = dataGrid.Rows[secilen].Cells[0].Value.ToString();
                bolumAd = dataGrid.Rows[secilen].Cells[1].Value.ToString();

                // İsmi kutuya yaz ki düzeltebilelim
                textBolumler.Text = bolumAd;
            }
            catch (Exception)
            {
                // Başlığa tıklanırsa hata vermesin
            }
        }

        private void Bolumler_Load(object sender, EventArgs e)
        {
            BolumListele();
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
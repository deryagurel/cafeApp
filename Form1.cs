using cafeApp;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace cafeApp
{
    public partial class Form1 : Form
    {
        private List<SepetUrun> sepet = new List<SepetUrun>();

        private int seciliKategoriID = 1;

        private int aktifMusteriID = -1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            UrunleriListele(1);
            KampanyaBilgisiniYukle();
        }

        private void KategoriButon_Click(object sender, EventArgs e)
        {
            Control tiklanan = (Control)sender;
            Button tiklananButon = null;

            if (tiklanan is Button)
            {
                tiklananButon = (Button)tiklanan;
            }
            else if (tiklanan.Parent is Button)
            {
                tiklananButon = (Button)tiklanan.Parent;
            }

            if (tiklananButon != null)
            {
                btnSicakIcecekler.BackColor = Color.LightBlue;
                btnSogukIcecekler.BackColor = Color.LightBlue;
                btnTatlilar.BackColor = Color.LightBlue;

                tiklananButon.BackColor = Color.LightGreen;

                if (tiklananButon.Name == "btnSicakIcecekler") seciliKategoriID = 1;
                else if (tiklananButon.Name == "btnSogukIcecekler") seciliKategoriID = 2;
                else if (tiklananButon.Name == "btnTatlilar") seciliKategoriID = 3;
                else if (tiklananButon.Tag != null)
                {
                    seciliKategoriID = int.Parse(tiklananButon.Tag.ToString());
                }

                UrunleriListele(seciliKategoriID);
            }
        }

        private void UrunleriListele(int kategoriID)
        {
            panelUrunler.Controls.Clear();

            string sorgu = "SELECT * FROM Urunler WHERE KategoriID = @kategoriID";
            SqlParameter[] p = { new SqlParameter("@kategoriID", kategoriID) };
            DataTable dt = Veritabani.VeriGetir(sorgu, p);

            foreach (DataRow row in dt.Rows)
            {
                Button btnUrun = new Button();

                // Ürün adını ve fiyatını buton yazısı yapıyoruz (\n\n\n ile resmi yukarıya, yazıyı aşağıya itiyoruz)
                btnUrun.Text = $"\n\n\n{row["UrunAdi"]}\n{row["Fiyat"]:#,##0.00} TL";
                btnUrun.Size = new Size(130, 150);
                btnUrun.BackColor = Color.White;
                btnUrun.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                // RESMİ KÜÇÜLTÜP BUTONA SIKIŞTIRAN EN ÖNEMLİ ALAN
                if (row["FotografYolu"] != DBNull.Value && !string.IsNullOrEmpty(row["FotografYolu"].ToString()))
                {
                    string tamYol = System.IO.Path.Combine(Application.StartupPath, row["FotografYolu"].ToString());
                    if (System.IO.File.Exists(tamYol))
                    {
                        using (Image orjinalResim = Image.FromFile(tamYol))
                        {
                            // Resmi 110x90 boyutuna küçültüp butonun tam üst ortasına hizalıyoruz
                            Bitmap boyutlandirilmisResim = new Bitmap(orjinalResim, new Size(110, 90));
                            btnUrun.Image = boyutlandirilmisResim;
                            btnUrun.TextImageRelation = TextImageRelation.ImageAboveText;
                            btnUrun.ImageAlign = ContentAlignment.TopCenter;
                        }
                    }
                }

                // Ürüne tıklandığında sepete ekleme tetiklensin
                btnUrun.Click += (s, args) =>
                {
                    SepeteUrunEkle(Convert.ToInt32(row["UrunID"]), row["UrunAdi"].ToString(), Convert.ToDecimal(row["Fiyat"]));
                };

                panelUrunler.Controls.Add(btnUrun);
            }
        }

        // Ürünü sepet listesine ekleyen metot
        private void SepeteUrunEkle(int id, string ad, decimal fiyat)
        {
            SepetUrun varOlan = sepet.Find(x => x.UrunID == id && x.Fiyat == fiyat);
            if (varOlan != null)
            {
                varOlan.Adet++;
            }
            else
            {
                sepet.Add(new SepetUrun { UrunID = id, UrunAdi = ad, Fiyat = fiyat, Adet = 1 });
            }
            SepetGuncelle();
        }

        // Sepetteki verileri ListBox'a yazdıran ve toplam tutarı hesaplayan metot
        private void SepetGuncelle()
        {
            listBoxSepet.Items.Clear();
            decimal toplam = 0;

            foreach (var urun in sepet)
            {
                listBoxSepet.Items.Add(urun);
                toplam += urun.ToplamTutar;
            }

            lblToplamTutar2.Text = $"{toplam:#,##0.00} TL";
        }

        // Seçili ürünü silme metodu
        private void btnSepettenSil_Click(object sender, EventArgs e)
        {
            if (listBoxSepet.SelectedItem != null)
            {
                SepetUrun secili = (SepetUrun)listBoxSepet.SelectedItem;
                if (secili.Adet > 1)
                {
                    secili.Adet--;
                }
                else
                {
                    sepet.Remove(secili);
                }
                SepetGuncelle();
            }
            else
            {
                MessageBox.Show("Lütfen sepetten silmek istediğiniz ürünü seçin!");
            }
        }

        // Müşteri kaydetme veya telefon numarasıyla eski puanı sorgulama metodu
        private void btnMusteriKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTelefon.Text))
            {
                MessageBox.Show("Lütfen bir telefon numarası girin!");
                return;
            }

            string kontrolSorgu = "SELECT MusteriID, KahveHakki FROM Musteriler WHERE Telefon = @tel";
            SqlParameter[] p1 = { new SqlParameter("@tel", txtTelefon.Text) };
            DataTable dt = Veritabani.VeriGetir(kontrolSorgu, p1);

            if (dt.Rows.Count > 0)
            {
                aktifMusteriID = Convert.ToInt32(dt.Rows[0]["MusteriID"]);
                int puan = Convert.ToInt32(dt.Rows[0]["KahveHakki"]);
                MessageBox.Show($"CafeCorner'a Hoş Geldiniz!\nMevcut Kahve Puanınız: {puan}");
            }
            else
            {
                string ekleSorgu = "INSERT INTO Musteriler (Ad, Soyad, Telefon, KahveHakki) OUTPUT INSERTED.MusteriID VALUES (@ad, @soyad, @tel, 0)";
                SqlParameter[] p2 = {
                        new SqlParameter("@ad", txtAd.Text),
                        new SqlParameter("@soyad", txtSoyad.Text),
                        new SqlParameter("@tel", txtTelefon.Text)
                    };

                using (SqlConnection conn = Veritabani.BaglantiAl())
                {
                    SqlCommand cmd = new SqlCommand(ekleSorgu, conn);
                    cmd.Parameters.AddRange(p2);
                    conn.Open();
                    aktifMusteriID = (int)cmd.ExecuteScalar();
                }
                MessageBox.Show("Yeni CafeCorner Sadakat Kartı Oluşturuldu!");
            }
        }

        // Ödeme yapıldığında siparişi kaydeden ana metot
        private void btnOdemeYap_Click(object sender, EventArgs e)
        {
            if (sepet.Count == 0)
            {
                MessageBox.Show("Sepetiniz boş!");
                return;
            }

            string odemeTipi = "";
            if (radioKart.Checked) odemeTipi = "Kart";
            else if (radioNakit.Checked) odemeTipi = "Nakit";
            else
            {
                MessageBox.Show("Lütfen bir ödeme tipi seçin!");
                return;
            }

            // Sepette kaç tane kahve (Kategori 1 veya 2) olduğunu hesapla
            int satinAlinanKahveSayisi = 0;
            foreach (var urun in sepet)
            {
                if (UrunKahveMi(urun.UrunID))
                {
                    satinAlinanKahveSayisi += urun.Adet;
                }
            }

            // SADAKAT SİSTEMİ - HEDİYE SİSTEMİ
            if (aktifMusteriID != -1 && satinAlinanKahveSayisi > 0)
            {
                string puanSorgu = "SELECT KahveHakki FROM Musteriler WHERE MusteriID = @id";
                SqlParameter[] pId = { new SqlParameter("@id", aktifMusteriID) };
                DataTable dtPuan = Veritabani.VeriGetir(puanSorgu, pId);

                if (dtPuan.Rows.Count > 0)
                {
                    int mevcutPuan = Convert.ToInt32(dtPuan.Rows[0]["KahveHakki"]);
                    int toplamPuan = mevcutPuan + satinAlinanKahveSayisi;

                    if (toplamPuan >= 3)
                    {
                        int hediyeKurabiyeSayisi = toplamPuan / 3;
                        int kalanPuan = toplamPuan % 3;

                        // SQL tablonuza göre 'Çikolatalı Cookie' ürününün ID'sini otomatik buluyoruz
                        int cookieID = 16;
                        DataTable dtCookie = Veritabani.VeriGetir("SELECT TOP 1 UrunID FROM Urunler WHERE UrunAdi = N'Çikolatalı Cookie'");
                        if (dtCookie.Rows.Count > 0) cookieID = Convert.ToInt32(dtCookie.Rows[0]["UrunID"]);

                        // Fiyatı 0 TL olacak şekilde sepete hediye ekle
                        SepeteUrunEkle(cookieID, "HEDİYE Çikolatalı Cookie", 0);
                        var hediyeUrun = sepet.Find(x => x.UrunID == cookieID && x.Fiyat == 0);
                        if (hediyeUrun != null)
                        {
                            hediyeUrun.Adet = hediyeKurabiyeSayisi;
                        }

                        MessageBox.Show($"TEBRİKLER!\n3 Kahve Barajını Aştınız! {hediyeKurabiyeSayisi} Adet Çikolatalı Cookie Sepetinize HEDİYE olarak eklenmiştir!\nYeni Puanınız: {kalanPuan}");

                        // Müşterinin yeni puanını güncelle
                        string puanGuncelle = "UPDATE Musteriler SET KahveHakki = @kalan WHERE MusteriID = @id";
                        SqlParameter[] pGuncel = { new SqlParameter("@kalan", kalanPuan), new SqlParameter("@id", aktifMusteriID) };
                        Veritabani.KomutCalistir(puanGuncelle, pGuncel);
                    }
                    else
                    {
                        // 3 barajı geçilmediyse puanları üzerine ekle
                        string puanEkle = "UPDATE Musteriler SET KahveHakki = KahveHakki + @eklenen WHERE MusteriID = @id";
                        SqlParameter[] pEkle = { new SqlParameter("@eklenen", satinAlinanKahveSayisi), new SqlParameter("@id", aktifMusteriID) };
                        Veritabani.KomutCalistir(puanEkle, pEkle);
                    }
                }
            }

            // Hediye eklendikten sonraki son toplam tutarı hesapla
            decimal toplamTutar = 0;
            foreach (var urun in sepet)
            {
                toplamTutar += urun.ToplamTutar;
            }

            // Siparişi Siparisler Tablosuna Yazma ve yeni ID'yi çekme
            string satisSorgu = "INSERT INTO Siparisler (MusteriID, ToplamTutar, OdemeTipi, Tarih) OUTPUT INSERTED.SiparisID VALUES (@mID, @tutar, @tip, GETDATE())";
            SqlParameter[] pSatis = {
                new SqlParameter("@mID", aktifMusteriID == -1 ? (object)DBNull.Value : aktifMusteriID),
                new SqlParameter("@tutar", toplamTutar),
                new SqlParameter("@tip", odemeTipi)
            };

            int yeniSiparisID = -1;
            using (SqlConnection conn = Veritabani.BaglantiAl())
            {
                SqlCommand cmd = new SqlCommand(satisSorgu, conn);
                cmd.Parameters.AddRange(pSatis);
                conn.Open();
                yeniSiparisID = (int)cmd.ExecuteScalar();
            }

            // Sipariş detaylarını SiparisDetay Tablosuna yazma (İlişkisel Veritabanı)
            if (yeniSiparisID != -1)
            {
                foreach (var urun in sepet)
                {
                    string detaySorgu = "INSERT INTO SiparisDetay (SiparisID, UrunID, Adet, BirimFiyat) VALUES (@siparisID, @urunID, @adet, @birimFiyat)";
                    SqlParameter[] pDetay = {
                        new SqlParameter("@siparisID", yeniSiparisID),
                        new SqlParameter("@urunID", urun.UrunID),
                        new SqlParameter("@adet", urun.Adet),
                        new SqlParameter("@birimFiyat", urun.Fiyat)
                    };
                    Veritabani.KomutCalistir(detaySorgu, pDetay);
                }
            }

            MessageBox.Show($"Sipariş Başarıyla Tamamlandı!\nToplam Ödenen: {toplamTutar:#,##0.00} TL\nAfiyet Olsun!");

            sepet.Clear();
            SepetGuncelle();
            MusteriAlaniniTemizle();
        }

        // Ürünün kahve olup olmadığını (KategoriID 1 veya 2 mi) doğrulayan yardımcı metot
        private bool UrunKahveMi(int urunID)
        {
            string sorgu = "SELECT KategoriID FROM Urunler WHERE UrunID = @id";
            SqlParameter[] p = { new SqlParameter("@id", urunID) };
            DataTable dt = Veritabani.VeriGetir(sorgu, p);
            if (dt.Rows.Count > 0)
            {
                int katID = Convert.ToInt32(dt.Rows[0]["KategoriID"]);
                return katID == 1 || katID == 2;
            }
            return false;
        }

        private void KampanyaBilgisiniYukle()
        {
            lblKampanya.Text = "3 Kahveye 1 Çikolatalı Cookie Hediye!";
        }

        private void MusteriAlaniniTemizle()
        {
            txtAd.Text = "";
            txtSoyad.Text = "";
            txtTelefon.Text = "";
            aktifMusteriID = -1;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
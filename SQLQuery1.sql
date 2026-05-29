USE CafeCornerDB;
GO
DELETE FROM SiparisDetay;
DELETE FROM Siparisler;
DELETE FROM Urunler;
DELETE FROM Kategoriler;
DELETE FROM Kampanyalar;

-- Otomatik artan ID'leri sıfırlıyoruz ki her şey 1'den başlasın
DBCC CHECKIDENT ('Kategoriler', RESEED, 0);
DBCC CHECKIDENT ('Urunler', RESEED, 0);

-- 2. KATEGORİLERİ SIRAYLA EKLEME
INSERT INTO Kategoriler (KategoriAdi) VALUES (N'Sıcak İçecekler'); -- KategoriID: 1
INSERT INTO Kategoriler (KategoriAdi) VALUES (N'Soğuk İçecekler'); -- KategoriID: 2
INSERT INTO Kategoriler (KategoriAdi) VALUES (N'Tatlılar');         -- KategoriID: 3

-- 3. ÜRÜNLERİ GÖRSEL SIRANA VE FİYATLARINA GÖRE EKSİKSİZ EKLEME (Tam 15 Satılık Ürün)

-- --- Sıcak İçecekler (KategoriID = 1) ---
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (1, N'Wiener melange', 180.00, 'wiener.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (1, N'latte macchiato', 195.00, 'latte.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (1, N'Türk Kahvesi', 150.00, 'turk_kahvesi.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (1, N'Matcha latte', 220.00, 'matcha.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (1, N'Latte', 150.00, 'latte_normal.jpg');

-- --- Soğuk İçecekler (KategoriID = 2) ---
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (2, N'Ice Latte', 195.00, 'iced_latte.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (2, N'C. Mocha Frappe', 230.00, 'frappe.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (2, N'Gin Tonic', 150.00, 'gin_tonic.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (2, N'Straw. Smoothie', 240.00, 'smoothie.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (2, N'Kiwi Smoothie', 240.00, 'kiwi.jpg');

-- --- Tatlılar (KategoriID = 3) ---
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'tiramisu', 240.00, 'tiramisu.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'5li Cokie', 130.00, 'cookie5.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'chocolate cake', 230.00, 'chocolate_cake.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'Browni', 230.00, 'browni.jpg');
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'magnolia', 180.00, 'magnolia.jpg');

-- --- HEDİYE KURABİYE SİSTEMİ ÜRÜNÜ (16. Ürün) ---
INSERT INTO Urunler (KategoriID, UrunAdi, Fiyat, FotografYolu) VALUES (3, N'HEDİYE Çikolatalı Cookie', 0.00, 'cookie.jpg');

-- 4. KAMPANYA TANIMLAMASI
INSERT INTO Kampanyalar (CampanyaAdi, Aciklama) VALUES (N'3 Kahveye 1 Cookie Hediye', N'Sadakat kartı olan müşterilerimize özel kampanya!');
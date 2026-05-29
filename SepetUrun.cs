using System;

namespace cafeApp
{
    // Sepetteki her bir ürünü, adetini ve toplam tutarını hafızada tutacak modelimiz
    public class SepetUrun
    {
        public int UrunID { get; set; }
        public string UrunAdi { get; set; }
        public decimal Fiyat { get; set; }
        public int Adet { get; set; }

        public decimal ToplamTutar => Fiyat * Adet;
        public override string ToString()
        {
            return $"{UrunAdi} x{Adet} - {ToplamTutar:C2}";
        }
    }
}

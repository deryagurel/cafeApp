using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;

namespace cafeApp
{
    public static class Veritabani
    {
        private static string baglantiCumlesi = @"Server=(localdb)\MSSQLLocalDB;Database=CafeCornerDB;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";

        public static SqlConnection BaglantiAl()
        {
            return new SqlConnection(baglantiCumlesi);
        }

        public static DataTable VeriGetir(string sorgu, SqlParameter[] parametreler = null)
        {
            using (SqlConnection baglanti = BaglantiAl())
            {
                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    if (parametreler != null)
                    {
                        komut.Parameters.Clear();
                        komut.Parameters.AddRange(parametreler);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(komut))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int KomutCalistir(string sorgu, SqlParameter[] parametreler = null)
        {
            using (SqlConnection baglanti = BaglantiAl())
            {
                using (SqlCommand komut = new SqlCommand(sorgu, baglanti))
                {
                    if (parametreler != null)
                    {
                        komut.Parameters.Clear();
                        komut.Parameters.AddRange(parametreler);
                    }

                    baglanti.Open();
                    return komut.ExecuteNonQuery();
                }
            }
        }
    }
}
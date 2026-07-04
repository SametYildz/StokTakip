using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;
using StokTakip.Models;
using System.Globalization;

namespace StokTakip.Data
{
    public class SqliteDataAccess
    {
        private readonly string _dbFile;

        public SqliteDataAccess()
        {
            _dbFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StokTakip.db");
        }

        public void InitializeDatabase()
        {
            var needsCreate = !File.Exists(_dbFile);
            Directory.CreateDirectory(Path.GetDirectoryName(_dbFile) ?? AppDomain.CurrentDomain.BaseDirectory);

            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Urun (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ad TEXT NOT NULL,
    Kategori TEXT,
    Fiyat REAL,
    StokAdedi INTEGER,
    MinimumStok INTEGER
);

CREATE TABLE IF NOT EXISTS Fatura (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FaturaNo TEXT,
    Tedarikci TEXT,
    GelisTarihi TEXT,
    UrunAdi TEXT,
    UrunKategorisi TEXT,
    Miktar INTEGER,
    BirimFiyat REAL,
    IskontoYuzdesi REAL,
    KdvYuzdesi REAL
);";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Urun> GetAllUrunler()
        {
            var list = new List<Urun>();
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Ad, Kategori, Fiyat, StokAdedi, MinimumStok FROM Urun ORDER BY Id";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Urun
                            {
                                Id = r.GetInt32(0),
                                Ad = r.IsDBNull(1) ? "" : r.GetString(1),
                                Kategori = r.IsDBNull(2) ? "" : r.GetString(2),
                                Fiyat = r.IsDBNull(3) ? 0m : Convert.ToDecimal(r.GetDouble(3)),
                                StokAdedi = r.IsDBNull(4) ? 0 : r.GetInt32(4),
                                MinimumStok = r.IsDBNull(5) ? 0 : r.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<Fatura> GetAllFaturalar()
        {
            var list = new List<Fatura>();
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT Id, FaturaNo, Tedarikci, GelisTarihi, UrunAdi, UrunKategorisi, Miktar, BirimFiyat, IskontoYuzdesi, KdvYuzdesi
FROM Fatura ORDER BY Id";
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var gelis = r.IsDBNull(3) ? DateTime.Today : DateTime.ParseExact(r.GetString(3), "o", CultureInfo.InvariantCulture);
                            list.Add(new Fatura
                            {
                                Id = r.GetInt32(0),
                                FaturaNo = r.IsDBNull(1) ? "" : r.GetString(1),
                                Tedarikci = r.IsDBNull(2) ? "" : r.GetString(2),
                                GelisTarihi = gelis,
                                UrunAdi = r.IsDBNull(4) ? "" : r.GetString(4),
                                UrunKategorisi = r.IsDBNull(5) ? "" : r.GetString(5),
                                Miktar = r.IsDBNull(6) ? 0 : r.GetInt32(6),
                                BirimFiyat = r.IsDBNull(7) ? 0m : Convert.ToDecimal(r.GetDouble(7)),
                                IskontoYuzdesi = r.IsDBNull(8) ? 0m : Convert.ToDecimal(r.GetDouble(8)),
                                KdvYuzdesi = r.IsDBNull(9) ? 0m : Convert.ToDecimal(r.GetDouble(9))
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void AddUrun(Urun u)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Urun (Ad, Kategori, Fiyat, StokAdedi, MinimumStok) VALUES (@ad,@kat,@fiyat,@stok,@min); SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("@ad", u.Ad ?? "");
                    cmd.Parameters.AddWithValue("@kat", u.Kategori ?? "");
                    cmd.Parameters.AddWithValue("@fiyat", (double)u.Fiyat);
                    cmd.Parameters.AddWithValue("@stok", u.StokAdedi);
                    cmd.Parameters.AddWithValue("@min", u.MinimumStok);
                    var id = (long)cmd.ExecuteScalar();
                    u.Id = (int)id;
                }
            }
        }

        public void UpdateUrun(Urun u)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Urun SET Ad=@ad, Kategori=@kat, Fiyat=@fiyat, StokAdedi=@stok, MinimumStok=@min WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@ad", u.Ad ?? "");
                    cmd.Parameters.AddWithValue("@kat", u.Kategori ?? "");
                    cmd.Parameters.AddWithValue("@fiyat", (double)u.Fiyat);
                    cmd.Parameters.AddWithValue("@stok", u.StokAdedi);
                    cmd.Parameters.AddWithValue("@min", u.MinimumStok);
                    cmd.Parameters.AddWithValue("@id", u.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUrun(int id)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Urun WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddFatura(Fatura f)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
INSERT INTO Fatura (FaturaNo, Tedarikci, GelisTarihi, UrunAdi, UrunKategorisi, Miktar, BirimFiyat, IskontoYuzdesi, KdvYuzdesi)
VALUES (@no,@ted,@gel,@urun,@kat,@miktar,@birim,@isk,@kdv);
SELECT last_insert_rowid();";
                    cmd.Parameters.AddWithValue("@no", f.FaturaNo ?? "");
                    cmd.Parameters.AddWithValue("@ted", f.Tedarikci ?? "");
                    cmd.Parameters.AddWithValue("@gel", f.GelisTarihi.ToString("o"));
                    cmd.Parameters.AddWithValue("@urun", f.UrunAdi ?? "");
                    cmd.Parameters.AddWithValue("@kat", f.UrunKategorisi ?? "");
                    cmd.Parameters.AddWithValue("@miktar", f.Miktar);
                    cmd.Parameters.AddWithValue("@birim", (double)f.BirimFiyat);
                    cmd.Parameters.AddWithValue("@isk", (double)f.IskontoYuzdesi);
                    cmd.Parameters.AddWithValue("@kdv", (double)f.KdvYuzdesi);
                    var id = (long)cmd.ExecuteScalar();
                    f.Id = (int)id;
                }
            }
        }

        public void UpdateFatura(Fatura f)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE Fatura SET FaturaNo=@no, Tedarikci=@ted, GelisTarihi=@gel, UrunAdi=@urun, UrunKategorisi=@kat, Miktar=@miktar, BirimFiyat=@birim, IskontoYuzdesi=@isk, KdvYuzdesi=@kdv
WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@no", f.FaturaNo ?? "");
                    cmd.Parameters.AddWithValue("@ted", f.Tedarikci ?? "");
                    cmd.Parameters.AddWithValue("@gel", f.GelisTarihi.ToString("o"));
                    cmd.Parameters.AddWithValue("@urun", f.UrunAdi ?? "");
                    cmd.Parameters.AddWithValue("@kat", f.UrunKategorisi ?? "");
                    cmd.Parameters.AddWithValue("@miktar", f.Miktar);
                    cmd.Parameters.AddWithValue("@birim", (double)f.BirimFiyat);
                    cmd.Parameters.AddWithValue("@isk", (double)f.IskontoYuzdesi);
                    cmd.Parameters.AddWithValue("@kdv", (double)f.KdvYuzdesi);
                    cmd.Parameters.AddWithValue("@id", f.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteFatura(int id)
        {
            using (var conn = new SqliteConnection($"Data Source={_dbFile}"))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Fatura WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
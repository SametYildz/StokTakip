using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StokTakip.Models;
using StokTakip.Views;
using StokTakip.Data;

namespace StokTakip
{
    public partial class App : Application
    {
        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.Handled = true;
        }
    }
    public class AylikRapor
    {
        public string AyAdi { get; set; }
        public int FaturaSayisi { get; set; }
        public int ToplamUrunAdedi { get; set; }
        public decimal AraToplam { get; set; }
        public decimal ToplamIskonto { get; set; }
        public decimal ToplamKdv { get; set; }
        public decimal GenelToplam { get; set; }
        public bool BuAyMi { get; set; }
    }

    public class YillikRapor
    {
        public int Yil { get; set; }
        public int FaturaSayisi { get; set; }
        public int ToplamUrunAdedi { get; set; }
        public decimal AraToplam { get; set; }
        public decimal ToplamIskonto { get; set; }
        public decimal ToplamKdv { get; set; }
        public decimal GenelToplam { get; set; }
        public bool BuYilMi { get; set; }
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<Urun> _tumUrunler = new ObservableCollection<Urun>();
        private ObservableCollection<Fatura> _tumFaturalar = new ObservableCollection<Fatura>();
        private SqliteDataAccess _db = new SqliteDataAccess();

        public MainWindow()
        {
            InitializeComponent();

                 _db.InitializeDatabase();

            

            LoadFromDatabase();

            dgFaturalar.ItemsSource = _tumFaturalar;
            YillariDoldur();
            RaporuGuncelle();
        }

        private void LoadFromDatabase()
        {
            _tumUrunler = new ObservableCollection<Urun>(_db.GetAllUrunler());
            dgUrunler.ItemsSource = _tumUrunler;

            _tumFaturalar = new ObservableCollection<Fatura>(_db.GetAllFaturalar());
            dgFaturalar.ItemsSource = _tumFaturalar;

            ListeyiGuncelle();
            YillikRaporuGuncelle();
        }

        private void YillariDoldur()
        {
            int buYil = DateTime.Today.Year;
            cmbYil.Items.Clear();
            for (int y = buYil; y >= buYil - 4; y--)
                cmbYil.Items.Add(y);
            cmbYil.SelectedIndex = 0;
        }

        private void cmbYil_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaporuGuncelle();
        }

        private void RaporuGuncelle()
        {
            if (cmbYil.SelectedItem == null) return;

            int secilenYil = (int)cmbYil.SelectedItem;
            int buAy = DateTime.Today.Month;
            int buYil = DateTime.Today.Year;

            var aylar = new string[]
            {
                "Ocak","Şubat","Mart","Nisan","Mayıs","Haziran",
                "Temmuz","Ağustos","Eylül","Ekim","Kasım","Aralık"
            };

            var raporListesi = new List<AylikRapor>();

            for (int ay = 1; ay <= 12; ay++)
            {
                var ayFaturalari = _tumFaturalar
                    .Where(f => f.GelisTarihi.Year == secilenYil && f.GelisTarihi.Month == ay)
                    .ToList();

                raporListesi.Add(new AylikRapor
                {
                    AyAdi = aylar[ay - 1],
                    FaturaSayisi = ayFaturalari.Count,
                    ToplamUrunAdedi = ayFaturalari.Sum(f => f.Miktar),
                    AraToplam = ayFaturalari.Sum(f => f.AraToplam),
                    ToplamIskonto = ayFaturalari.Sum(f => f.IskontoTutari),
                    ToplamKdv = ayFaturalari.Sum(f => f.KdvTutari),
                    GenelToplam = ayFaturalari.Sum(f => f.GenelToplam),
                    BuAyMi = secilenYil == buYil && ay == buAy
                });
            }

            dgRapor.ItemsSource = raporListesi;

            lblYillikToplam.Text = raporListesi.Sum(r => r.GenelToplam).ToString("N2") + " ₺";
            lblYillikUrun.Text = raporListesi.Sum(r => r.ToplamUrunAdedi).ToString();
            lblYillikFatura.Text = raporListesi.Sum(r => r.FaturaSayisi).ToString();
        }

        private void YillikRaporuGuncelle()
        {
            var yilGruplari = _tumFaturalar
                .GroupBy(f => f.GelisTarihi.Year)
                .OrderByDescending(g => g.Key)
                .Select(g => new YillikRapor
                {
                    Yil = g.Key,
                    FaturaSayisi = g.Count(),
                    ToplamUrunAdedi = g.Sum(f => f.Miktar),
                    AraToplam = g.Sum(f => f.AraToplam),
                    ToplamIskonto = g.Sum(f => f.IskontoTutari),
                    ToplamKdv = g.Sum(f => f.KdvTutari),
                    GenelToplam = g.Sum(f => f.GenelToplam),
                    BuYilMi = g.Key == DateTime.Today.Year
                })
                .ToList();

            dgYillik.ItemsSource = yilGruplari;
        }

        private void ListeyiGuncelle(string filtre = null)
        {
            var liste = _tumUrunler.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filtre))
            {
                liste = liste.Where(u =>
                    u.Ad.ToLower().Contains(filtre.ToLower()) ||
                    u.Kategori.ToLower().Contains(filtre.ToLower()));
            }

            dgUrunler.ItemsSource = liste.ToList();

            lblToplamUrun.Text = "Toplam: " + _tumUrunler.Count + " ürün";
            int dusuk = _tumUrunler.Count(u => u.DusukStokMu);
            lblDusukStok.Text = dusuk > 0 ? "⚠️ " + dusuk + " üründe stok kritik seviyede!" : "";
        }

        private void txtArama_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListeyiGuncelle(txtArama.Text);
        }

        private void Ekle_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UrunDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                _db.AddUrun(dialog.Urun);
                _tumUrunler.Add(dialog.Urun);
                ListeyiGuncelle(txtArama.Text);
            }
        }

        private void Duzenle_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgUrunler.SelectedItem is Urun secili))
            {
                MessageBox.Show("Lütfen düzenlenecek ürünü seçin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dialog = new UrunDialog(secili);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                _db.UpdateUrun(secili);
                ListeyiGuncelle(txtArama.Text);
            }
        }

        private void Sil_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgUrunler.SelectedItem is Urun secili))
            {
                MessageBox.Show("Lütfen silinecek ürünü seçin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var sonuc = MessageBox.Show(
                "'" + secili.Ad + "' ürününü silmek istediğinizden emin misiniz?",
                "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (sonuc == MessageBoxResult.Yes)
            {
                _db.DeleteUrun(secili.Id);
                _tumUrunler.Remove(secili);
                ListeyiGuncelle(txtArama.Text);
            }
        }

        private void FaturaEkle_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FaturaDialog(new List<Urun>(_tumUrunler));
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                // Faturayı DB'ye ekle
                _db.AddFatura(dialog.Fatura);

                // Ürün stoklarını DB ve koleksiyonlarda güncelle
                var mevcutUrun = _tumUrunler.FirstOrDefault(u =>
                    u.Ad.Equals(dialog.Fatura.UrunAdi, StringComparison.OrdinalIgnoreCase));

                if (mevcutUrun != null)
                {
                    mevcutUrun.StokAdedi += dialog.Fatura.Miktar;
                    mevcutUrun.Fiyat = dialog.Fatura.BirimFiyat;
                    mevcutUrun.MinimumStok = dialog.MinimumStok;
                    _db.UpdateUrun(mevcutUrun);
                }
                else
                {
                    var yeniUrun = new Urun
                    {
                        Ad = dialog.Fatura.UrunAdi,
                        Kategori = dialog.Fatura.UrunKategorisi,
                        Fiyat = dialog.Fatura.BirimFiyat,
                        StokAdedi = dialog.Fatura.Miktar,
                        MinimumStok = dialog.MinimumStok
                    };
                    _db.AddUrun(yeniUrun);
                    _tumUrunler.Add(yeniUrun);
                }

                _tumFaturalar.Add(dialog.Fatura);
                ListeyiGuncelle(txtArama.Text);
                RaporuGuncelle();
                YillikRaporuGuncelle();
                tabMain.SelectedIndex = 0;
            }
        }

        private void FaturaDuzenle_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgFaturalar.SelectedItem is Fatura secili))
            {
                MessageBox.Show("Lütfen düzenlenecek faturayı seçin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string eskiUrunAdi = secili.UrunAdi;
            int eskiMiktar = secili.Miktar;

            var dialog = new FaturaDialog(new List<Urun>(_tumUrunler), secili);
            dialog.Owner = this;

            if (dialog.ShowDialog() == true)
            {
                // eski ürün stokunu azalt
                var eskiUrun = _tumUrunler.FirstOrDefault(u =>
                    u.Ad.Equals(eskiUrunAdi, StringComparison.OrdinalIgnoreCase));
                if (eskiUrun != null)
                {
                    eskiUrun.StokAdedi -= eskiMiktar;
                    if (eskiUrun.StokAdedi < 0) eskiUrun.StokAdedi = 0;
                    _db.UpdateUrun(eskiUrun);
                }

                // yeni/aynı ürün stokunu arttır
                var yeniUrun = _tumUrunler.FirstOrDefault(u =>
                    u.Ad.Equals(secili.UrunAdi, StringComparison.OrdinalIgnoreCase));

                if (yeniUrun != null)
                {
                    yeniUrun.StokAdedi += secili.Miktar;
                    yeniUrun.Fiyat = secili.BirimFiyat;
                    yeniUrun.MinimumStok = dialog.MinimumStok;
                    _db.UpdateUrun(yeniUrun);
                }
                else
                {
                    var yeni = new Urun
                    {
                        Ad = secili.UrunAdi,
                        Kategori = secili.UrunKategorisi,
                        Fiyat = secili.BirimFiyat,
                        StokAdedi = secili.Miktar,
                        MinimumStok = dialog.MinimumStok
                    };
                    _db.AddUrun(yeni);
                    _tumUrunler.Add(yeni);
                }

                // faturayı DB'ye kaydet
                _db.UpdateFatura(secili);

                ListeyiGuncelle(txtArama.Text);
                RaporuGuncelle();
                YillikRaporuGuncelle();
            }
        }

        private void FaturaSil_Click(object sender, RoutedEventArgs e)
        {
            if (!(dgFaturalar.SelectedItem is Fatura secili))
            {
                MessageBox.Show("Lütfen silinecek faturayı seçin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sonuc = MessageBox.Show(
                "'" + secili.FaturaNo + "' numaralı faturayı silmek istediğinizden emin misiniz?\n\nÜrün stoku da güncellenecektir.",
                "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (sonuc == MessageBoxResult.Yes)
            {
                var urun = _tumUrunler.FirstOrDefault(u =>
                    u.Ad.Equals(secili.UrunAdi, StringComparison.OrdinalIgnoreCase));
                if (urun != null)
                {
                    urun.StokAdedi -= secili.Miktar;
                    if (urun.StokAdedi < 0)
                        urun.StokAdedi = 0;
                    _db.UpdateUrun(urun);
                }

                _db.DeleteFatura(secili.Id);

                _tumFaturalar.Remove(secili);
                ListeyiGuncelle(txtArama.Text);
                RaporuGuncelle();
                YillikRaporuGuncelle();
            }
        }
    }
}
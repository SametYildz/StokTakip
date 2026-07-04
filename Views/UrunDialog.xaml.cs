using System.Windows;
using StokTakip.Models;

namespace StokTakip.Views
{
    public partial class UrunDialog : Window
    {
        public Urun Urun { get; private set; }

        public UrunDialog(Urun mevcutUrun = null)
        {
            InitializeComponent();

            if (mevcutUrun != null)
            {
                Urun = mevcutUrun;
                txtAd.Text = mevcutUrun.Ad;
                txtKategori.Text = mevcutUrun.Kategori;
                txtFiyat.Text = mevcutUrun.Fiyat.ToString();
                txtStok.Text = mevcutUrun.StokAdedi.ToString();
                txtMinStok.Text = mevcutUrun.MinimumStok.ToString();
            }
            else
            {
                Urun = new Urun();
            }
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAd.Text))
            {
                MessageBox.Show("Ürün adı boş olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtFiyat.Text, out decimal fiyat))
            {
                MessageBox.Show("Geçerli bir fiyat girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtStok.Text, out int stok))
            {
                MessageBox.Show("Geçerli bir stok adedi girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtMinStok.Text, out int minStok))
            {
                MessageBox.Show("Geçerli bir minimum stok girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Urun.Ad = txtAd.Text.Trim();
            Urun.Kategori = txtKategori.Text.Trim();
            Urun.Fiyat = fiyat;
            Urun.StokAdedi = stok;
            Urun.MinimumStok = minStok;

            DialogResult = true;
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
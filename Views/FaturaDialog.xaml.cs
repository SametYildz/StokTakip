using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StokTakip.Models;

namespace StokTakip.Views
{
    public partial class FaturaDialog : Window
    {
        public Fatura Fatura { get; private set; }
        public int MinimumStok { get; private set; }
        private List<Urun> _urunler;

        public FaturaDialog(List<Urun> urunler, Fatura mevcutFatura = null)
        {
            InitializeComponent();
            _urunler = urunler;
            dpTarih.SelectedDate = DateTime.Today;
            cmbKdv.SelectedIndex = 2;

            if (mevcutFatura != null)
            {
                Fatura = mevcutFatura;
                txtFaturaNo.Text = mevcutFatura.FaturaNo;
                txtTedarikci.Text = mevcutFatura.Tedarikci;
                cmbUrun.Text = mevcutFatura.UrunAdi;
                txtKategori.Text = mevcutFatura.UrunKategorisi;
                dpTarih.SelectedDate = mevcutFatura.GelisTarihi;
                txtMiktar.Text = mevcutFatura.Miktar.ToString();
                txtBirimFiyat.Text = mevcutFatura.BirimFiyat.ToString();
                txtIskonto.Text = mevcutFatura.IskontoYuzdesi.ToString();

                var urun = _urunler.FirstOrDefault(u =>
                    u.Ad.ToLower() == mevcutFatura.UrunAdi.ToLower());
                txtMinStok.Text = urun != null ? urun.MinimumStok.ToString() : "5";

                foreach (ComboBoxItem item in cmbKdv.Items)
                {
                    if (item.Content.ToString() == mevcutFatura.KdvYuzdesi.ToString("0"))
                    {
                        cmbKdv.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                Fatura = new Fatura();
            }
        }

        private void cmbUrun_TextChanged(object sender, TextChangedEventArgs e)
        {
            var eslesme = _urunler.FirstOrDefault(u =>
                u.Ad.ToLower() == cmbUrun.Text.ToLower());

            if (eslesme != null)
            {
                txtBirimFiyat.Text = eslesme.Fiyat.ToString();
                txtMinStok.Text = eslesme.MinimumStok.ToString();
                txtKategori.Text = eslesme.Kategori;
            }

            Hesapla(null, null);
        }

        private void txtTedarikci_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hesapla(null, null);
        }

        private void cmbKdv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Hesapla(null, null);
        }

        private void Hesapla(object sender, TextChangedEventArgs e)
        {
            if (lblGenelToplam == null) return;

            decimal.TryParse(txtMiktar.Text, out decimal miktar);
            decimal.TryParse(txtBirimFiyat.Text, out decimal birimFiyat);
            decimal.TryParse(txtIskonto.Text, out decimal iskonto);

            decimal kdv = 0;
            if (cmbKdv.SelectedItem is ComboBoxItem kdvItem)
                decimal.TryParse(kdvItem.Content.ToString(), out kdv);

            decimal araToplam = miktar * birimFiyat;
            decimal iskontoTutari = araToplam * iskonto / 100;
            decimal netTutar = araToplam - iskontoTutari;
            decimal kdvTutari = netTutar * kdv / 100;
            decimal genelToplam = netTutar + kdvTutari;

            lblAraToplam.Text = araToplam.ToString("N2") + " ₺";
            lblIskonto.Text = "-" + iskontoTutari.ToString("N2") + " ₺";
            lblNetTutar.Text = netTutar.ToString("N2") + " ₺";
            lblKdv.Text = kdvTutari.ToString("N2") + " ₺";
            lblGenelToplam.Text = genelToplam.ToString("N2") + " ₺";

            var urun = _urunler.FirstOrDefault(u =>
                u.Ad.ToLower() == cmbUrun.Text.ToLower());
            if (urun != null && miktar > 0 && urun.StokAdedi <= urun.MinimumStok)
            {
                pnlUyari.Visibility = Visibility.Visible;
                lblUyari.Text = "⚠️ Bu ürünün mevcut stoğu minimum seviyeye yakın!";
            }
            else
            {
                pnlUyari.Visibility = Visibility.Collapsed;
            }
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFaturaNo.Text))
            {
                MessageBox.Show("Fatura no boş olamaz.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(cmbUrun.Text))
            {
                MessageBox.Show("Lütfen ürün adı girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtMiktar.Text, out int miktar) || miktar <= 0)
            {
                MessageBox.Show("Geçerli bir miktar girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtBirimFiyat.Text, out decimal birimFiyat) || birimFiyat <= 0)
            {
                MessageBox.Show("Geçerli bir birim fiyat girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(txtIskonto.Text, out decimal iskonto))
                iskonto = 0;
            if (!int.TryParse(txtMinStok.Text, out int minStok) || minStok < 0)
            {
                MessageBox.Show("Geçerli bir minimum stok girin.", "Uyarı",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal kdv = 0;
            if (cmbKdv.SelectedItem is ComboBoxItem kdvItem)
                decimal.TryParse(kdvItem.Content.ToString(), out kdv);

            Fatura.FaturaNo = txtFaturaNo.Text.Trim();
            Fatura.Tedarikci = txtTedarikci.Text.Trim();
            Fatura.GelisTarihi = dpTarih.SelectedDate ?? DateTime.Today;
            Fatura.UrunAdi = cmbUrun.Text.Trim();
            Fatura.UrunKategorisi = txtKategori.Text.Trim();
            Fatura.Miktar = miktar;
            Fatura.BirimFiyat = birimFiyat;
            Fatura.IskontoYuzdesi = iskonto;
            Fatura.KdvYuzdesi = kdv;
            MinimumStok = minStok;

            DialogResult = true;
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
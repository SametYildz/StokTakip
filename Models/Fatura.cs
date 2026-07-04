using System;
using System.ComponentModel;

namespace StokTakip.Models
{
    public class Fatura : INotifyPropertyChanged
    {
        private int _id;
        private string _faturaNo = "";
        private string _tedarikci = "";
        private DateTime _gelisTarihi = DateTime.Today;
        private string _urunAdi = "";
        private string _urunKategorisi = "";
        private int _miktar;
        private decimal _birimFiyat;
        private decimal _iskontoYuzdesi;
        private decimal _kdvYuzdesi;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string FaturaNo
        {
            get => _faturaNo;
            set { _faturaNo = value; OnPropertyChanged(nameof(FaturaNo)); }
        }

        public string Tedarikci
        {
            get => _tedarikci;
            set { _tedarikci = value; OnPropertyChanged(nameof(Tedarikci)); }
        }

        public DateTime GelisTarihi
        {
            get => _gelisTarihi;
            set { _gelisTarihi = value; OnPropertyChanged(nameof(GelisTarihi)); }
        }

        public string UrunAdi
        {
            get => _urunAdi;
            set { _urunAdi = value; OnPropertyChanged(nameof(UrunAdi)); }
        }

        public string UrunKategorisi
        {
            get => _urunKategorisi;
            set { _urunKategorisi = value; OnPropertyChanged(nameof(UrunKategorisi)); }
        }

        public int Miktar
        {
            get => _miktar;
            set
            {
                _miktar = value;
                OnPropertyChanged(nameof(Miktar));
                OnPropertyChanged(nameof(AraToplam));
                OnPropertyChanged(nameof(IskontoTutari));
                OnPropertyChanged(nameof(NetTutar));
                OnPropertyChanged(nameof(KdvTutari));
                OnPropertyChanged(nameof(GenelToplam));
            }
        }

        public decimal BirimFiyat
        {
            get => _birimFiyat;
            set
            {
                _birimFiyat = value;
                OnPropertyChanged(nameof(BirimFiyat));
                OnPropertyChanged(nameof(AraToplam));
                OnPropertyChanged(nameof(IskontoTutari));
                OnPropertyChanged(nameof(NetTutar));
                OnPropertyChanged(nameof(KdvTutari));
                OnPropertyChanged(nameof(GenelToplam));
            }
        }

        public decimal IskontoYuzdesi
        {
            get => _iskontoYuzdesi;
            set
            {
                _iskontoYuzdesi = value;
                OnPropertyChanged(nameof(IskontoYuzdesi));
                OnPropertyChanged(nameof(IskontoTutari));
                OnPropertyChanged(nameof(NetTutar));
                OnPropertyChanged(nameof(KdvTutari));
                OnPropertyChanged(nameof(GenelToplam));
            }
        }

        public decimal KdvYuzdesi
        {
            get => _kdvYuzdesi;
            set
            {
                _kdvYuzdesi = value;
                OnPropertyChanged(nameof(KdvYuzdesi));
                OnPropertyChanged(nameof(KdvTutari));
                OnPropertyChanged(nameof(GenelToplam));
            }
        }

        public decimal AraToplam => Miktar * BirimFiyat;
        public decimal IskontoTutari => AraToplam * IskontoYuzdesi / 100;
        public decimal NetTutar => AraToplam - IskontoTutari;
        public decimal KdvTutari => NetTutar * KdvYuzdesi / 100;
        public decimal GenelToplam => NetTutar + KdvTutari;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
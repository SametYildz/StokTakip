using System.ComponentModel;

namespace StokTakip.Models
{
    public class Urun : INotifyPropertyChanged
    {
        private int _id;
        private string _ad = "";
        private string _kategori = "";
        private decimal _fiyat;
        private int _stokAdedi;
        private int _minimumStok;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Ad
        {
            get => _ad;
            set { _ad = value; OnPropertyChanged(nameof(Ad)); }
        }

        public string Kategori
        {
            get => _kategori;
            set { _kategori = value; OnPropertyChanged(nameof(Kategori)); }
        }

        public decimal Fiyat
        {
            get => _fiyat;
            set { _fiyat = value; OnPropertyChanged(nameof(Fiyat)); }
        }

        public int StokAdedi
        {
            get => _stokAdedi;
            set
            {
                _stokAdedi = value;
                OnPropertyChanged(nameof(StokAdedi));
                OnPropertyChanged(nameof(DusukStokMu));
            }
        }

        public int MinimumStok
        {
            get => _minimumStok;
            set
            {
                _minimumStok = value;
                OnPropertyChanged(nameof(MinimumStok));
                OnPropertyChanged(nameof(DusukStokMu));
            }
        }

        public bool DusukStokMu => StokAdedi <= MinimumStok;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
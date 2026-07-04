using System;
using System.Windows;

namespace StokTakip
{
    /// <summary>
    /// App.xaml etkileşim mantığı
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // System.Data.SQLite kullanıyorsanız özel init gerekmez;
            base.OnStartup(e);
        }
    }
}

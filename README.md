# 📦 Stock Tracking Application

A modern and user-friendly desktop application designed for professional inventory management. Manage product stock control, invoice tracking, and financial reporting all on a single platform.

## 🌟 Features

### 📊 Product Management
- ✅ Add, edit, and delete products
- ✅ Category-based classification
- ✅ Real-time stock level tracking
- ✅ Critical stock level alerts (⚠️ Low Stock)
- ✅ Set minimum stock thresholds
- ✅ Advanced search and filtering

### 🧾 Invoice Operations
- ✅ Create and manage invoices
- ✅ Supplier information tracking
- ✅ Discount and VAT calculations
- ✅ Automatic stock updates
- ✅ Date-based tracking

### 📈 Reports & Statistics
- ✅ **Monthly Reports**: Monthly purchase summary
- ✅ **Yearly Summary**: Year-based financial data
- ✅ Total expenditure analysis
- ✅ Product entry statistics
- ✅ Invoice count tracking
- ✅ Discount and VAT analysis

## 🎯 Quick Start

### Requirements
- **Operating System**: Windows 7 or higher
- **.NET Framework**: 4.8 or higher
- **Database**: SQLite (built-in, no separate installation needed)

### Installation

1. **Download the latest version**: From [Releases](https://github.com/SametYildz/StokTakip/releases) page
2. **Run it**: Double-click the `StokTakip.exe` file
3. **Start using**: The database is automatically created when you first open the application

### Building from Source

```bash
git clone https://github.com/SametYildz/StokTakip.git
cd StokTakip
# Open in Visual Studio and build (Ctrl+Shift+B)
```

## 💻 Technologies

- **Language**: C# (.NET Framework 4.8)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Database**: SQLite
- **Architecture**: MVVM-compatible design

### Used Packages
```xml
- System.Data.SQLite (1.0.119.0)
- System.Memory (4.6.3)
- System.Buffers (4.6.1)
- System.Runtime.CompilerServices.Unsafe (6.1.2)
```

## 📖 User Guide

### 📦 Products Tab
1. **Add Product**: Click "➕ Add" button to create a new product
   - Enter product name, category, unit price, stock quantity, and minimum stock level
2. **Edit Product**: Select a product from the list and click "✏️ Edit"
3. **Delete Product**: Select a product and click "🗑️ Delete"
4. **Search**: Search by product name or category

**Status Indicator**:
- ✅ **Sufficient**: Stock is above minimum level
- ⚠️ **Low**: Critical stock level - warning is displayed

### 🧾 Invoices Tab
1. **Add New Invoice**: Enter invoice number, supplier, product information, and financial details
2. **Automatic Stock Update**: Stock is automatically updated when you add an invoice
3. **Edit Invoice**: Modify previous invoices (stock is automatically adjusted)
4. **Delete Invoice**: Remove an invoice and stock is recalculated

**Financial Calculations**:
- Net Amount = Unit Price × Quantity - Discount
- VAT = Net Amount × VAT%
- Grand Total = Net Amount + VAT

### 📊 Reports Tab

#### Monthly Reports
- Select a year to view monthly details for that year
- The current month is specially highlighted
- Quick statistics panel: Yearly Total, Total Product Entries, Invoice Count

#### Yearly Summary
- View summary information for all years
- Comparative analysis capabilities
- The current year is automatically highlighted

## 📁 Project Structure

```
StokTakip/
├── MainWindow.xaml/cs      # Main screen and business logic
├── Data/
│   └── SqliteDataAccess.cs # Database operations
├── Models/
│   ├── Urun.cs            # Product model
│   └── Fatura.cs          # Invoice model
├── Views/
│   ├── UrunDialog.xaml/cs # Product add/edit window
│   └── FaturaDialog.xaml/cs # Invoice add/edit window
├── StokTakip.db           # SQLite database (created on first run)
└── App.xaml/cs            # Application settings
```

## 🗄️ Database Schema

### Products Table
```sql
CREATE TABLE Urun (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Ad TEXT NOT NULL,
    Kategori TEXT,
    Fiyat REAL,
    StokAdedi INTEGER,
    MinimumStok INTEGER
);
```

### Invoices Table
```sql
CREATE TABLE Fatura (
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
);
```

## ⚙️ Settings

The database file (`StokTakip.db`) is stored at:
```
C:\Users\[UserName]\AppData\Local\[Applications]
```

**For Backup**: Regularly backup the `StokTakip.db` file

## 🐛 Known Issues

- If you encounter database initialization errors, ensure SQLite native components are installed

## 📝 Planned Features

- 📧 Send reports via email
- 📁 Export to Excel/PDF
- 🔔 Low stock notifications
- 💾 Backup and restore
- 🔐 User authentication
- 📱 Mobile application

## 🤝 Contributing

This is an open-source project. To contribute improvements:

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/NewFeature`)
3. Commit your changes (`git commit -m 'Add new feature'`)
4. Push to the branch (`git push origin feature/NewFeature`)
5. Open a Pull Request

## 📄 License

This project is released under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Developer

**Samet Yıldız** - [@SametYildz](https://github.com/SametYildz)

## 📞 Contact & Support

- 🐛 Report bugs: [Issues](https://github.com/SametYildz/StokTakip/issues)
- 💬 Questions or suggestions: Open Discussions or Pull Request
- 📧 Email: Send a message through Issues

## 🙏 Acknowledgments

- WPF and SQLite community
- Everyone supporting open-source software development

---

**⭐ Found this project helpful? Please give it a star!**

*Last Updated: 07.04.2026*

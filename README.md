# TechStore

TechStore, **ASP.NET Core MVC** ve **Entity Framework Core** ile geliştirilmiş, katmanlı mimariye sahip bir e-ticaret portfolyo projesidir. Ürün kataloğu, sepet, kupon, sipariş ve destek talebi yönetimi gibi gerçek bir e-ticaret sitesinde bulunan temel akışların uçtan uca uygulanmasını hedefler.

## Ekran Görüntüleri

<!-- Buraya ana sayfa, ürün detay, sepet ve admin panelinden birkaç ekran görüntüsü ekle -->
| Ana Sayfa | Ürün Detay | Admin Panel |
|---|---|---|
| _ekran görüntüsü_ | _ekran görüntüsü_ | _ekran görüntüsü_ |

## Canlı Demo

<!-- Deploy ettiğinde linki buraya ekle, yoksa bu bölümü kaldır -->
🔗 [Demo linki](#)

## Özellikler

### Müşteri Tarafı
- Kategoriye ve arama terimine göre filtrelenebilen, sayfalanan ürün listesi
- Ürün detay sayfası: galeri görselleri, ortalama puan, ilgili ürünler
- Sepete ekleme/çıkarma, miktar güncelleme — fiyatlar her zaman sunucu tarafında ürün fiyatından senkronize edilir
- Kupon kodu uygulama (yüzde/tutar bazlı indirim, minimum sepet tutarı ve kullanım limiti kontrolü)
- Çok adımlı satın alma akışı: teslimat bilgisi → ödeme → sipariş onayı
- Geçmiş siparişleri listeleme ve sipariş detayı görüntüleme
- İstek listesi (wishlist)
- Satın alınan ürünlere değerlendirme/yorum bırakma
- Destek talebi oluşturma ve talep geçmişini görüntüleme
- Kimlik doğrulama: kayıt, giriş, çıkış, profil güncelleme (ASP.NET Core Identity)

### Yönetici (Admin) Paneli
- Dashboard: sipariş sayısı, toplam ciro gibi özet metrikler
- Ürün yönetimi (CRUD) ve çoklu ürün görseli yükleme/yönetme
- Kategori yönetimi
- Kupon oluşturma ve yönetimi
- Sipariş listesi, arama/filtreleme ve sipariş durumu güncelleme
- Kullanıcı listesi, kullanıcı detayı ve rol atama
- Destek taleplerini görüntüleme ve yanıtlama

## Kullanılan Teknolojiler

- **Backend:** ASP.NET Core 8 MVC (.NET 8)
- **ORM / Veritabanı:** Entity Framework Core 8, SQL Server (LocalDB)
- **Kimlik Doğrulama:** ASP.NET Core Identity (rol tabanlı: Admin / User)
- **Frontend:** Razor Views, Bootstrap, jQuery
- **Diğer:** jQuery Validation (client-side doğrulama)

## Mimari

Proje, sorumlulukların net şekilde ayrıldığı katmanlı bir mimariyle yapılandırılmıştır:

```
Controllers/      → HTTP isteklerini karşılar, servisleri çağırır
Services/          → İş kuralları (Interfaces + Implementations)
Repositories/      → Veri erişim katmanı (Interfaces + Implementations)
Data/              → DbContext ve EF Core migration'ları
Models/            → Veritabanı varlıkları (entities)
DTOs/              → Katmanlar arası veri transfer nesneleri
ViewModels/        → View'lara özel veri modelleri
Views/             → Razor sayfaları
```

Bu ayrım sayesinde, örneğin veri erişim mantığı (Repository) değişse bile iş kuralları (Service) veya controller katmanı etkilenmez.

### Dikkat Edilen Bazı Teknik Noktalar
- **Stok tutarlılığı:** Sipariş oluşturma sırasında stok düşürme işlemi, veritabanı seviyesinde koşullu ve atomik olarak (`WHERE Stock >= quantity`) yapılır; bu sayede eşzamanlı satın alımlarda stok hatası (race condition) engellenir.
- **Fiyat güvenliği:** Sepetteki birim fiyat client'tan alınmaz, her işlemde sunucu tarafında ürünün güncel fiyatıyla senkronize edilir.
- **Yetkilendirme:** Sipariş/sepet gibi kullanıcıya özel veriler, her istekte oturum sahibinin kimliğiyle eşleştirilerek sorgulanır (başka bir kullanıcının verisine ID değiştirerek erişilemez).
- **Dosya yükleme güvenliği:** Ürün görselleri; uzantı, MIME türü ve dosya imzası (magic bytes) olmak üzere üç katmanlı doğrulamadan geçer.

## Kurulum

### Gereksinimler
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (Visual Studio ile birlikte gelir) veya bir SQL Server örneği

### Adımlar

```bash
# 1. Depoyu klonla
git clone https://github.com/<kullanici-adin>/TechStore.git
cd TechStore

# 2. Bağımlılıkları geri yükle
dotnet restore

# 3. Veritabanı bağlantı bilgisini ayarla
# appsettings.json içindeki ConnectionStrings:DefaultConnection değerini
# kendi ortamına göre düzenle (varsayılan LocalDB ile doğrudan çalışır)

# 4. Migration'ları uygula
dotnet ef database update

# 5. Projeyi çalıştır
dotnet run
```

Uygulama varsayılan olarak `https://localhost:xxxx` adresinde ayağa kalkar.

### Admin Hesabı
Bir kullanıcıyı admin yapmak için `appsettings.json` (veya User Secrets) içindeki `SeedData:AdminEmail` alanına, önceden kayıt olmuş bir kullanıcının e-postasını yaz; uygulama başlatıldığında o kullanıcı otomatik olarak `Admin` rolüne atanır.

## Geliştirme Notları

Bu proje aktif olarak geliştirilmektedir. Planlanan/olası geliştirmeler:
- Servis katmanı için unit testler
- Ödeme entegrasyonu (şu an ödeme adımı simülasyon amaçlıdır)

## Lisans

Bu proje kişisel portfolyo amacıyla geliştirilmiştir.

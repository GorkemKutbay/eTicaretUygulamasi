using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class AppDbContext : DbContext
    {
       
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ==================== ROLES ====================
            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity() { Id = 1, Name = "admin", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 2, Name = "seller", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 3, Name = "buyer", CreatedAt = DateTime.UtcNow }
            );

            // ==================== USERS ====================
            modelBuilder.Entity<UserEntity>().HasData(
                // Admin
                new UserEntity() { Id = 1, FirstName = "Admin", LastName = "User", Email = "admin@mail.com", Enabled = true, RoleId = 1, Password = "1234", CreatedAt = DateTime.UtcNow },
                // Sellers
                new UserEntity() { Id = 2, FirstName = "Ahmet", LastName = "Yılmaz", Email = "ahmet@mail.com", Enabled = true, RoleId = 2, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 3, FirstName = "Elif", LastName = "Kaya", Email = "elif@mail.com", Enabled = true, RoleId = 2, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 4, FirstName = "Mehmet", LastName = "Demir", Email = "mehmet@mail.com", Enabled = true, RoleId = 2, Password = "1234", CreatedAt = DateTime.UtcNow },
                // Buyers
                new UserEntity() { Id = 5, FirstName = "Zeynep", LastName = "Çelik", Email = "zeynep@mail.com", Enabled = true, RoleId = 3, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 6, FirstName = "Can", LastName = "Öztürk", Email = "can@mail.com", Enabled = true, RoleId = 3, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 7, FirstName = "Selin", LastName = "Arslan", Email = "selin@mail.com", Enabled = true, RoleId = 3, Password = "1234", CreatedAt = DateTime.UtcNow },
                new UserEntity() { Id = 8, FirstName = "Burak", LastName = "Şahin", Email = "burak@mail.com", Enabled = true, RoleId = 3, Password = "1234", CreatedAt = DateTime.UtcNow },
                // Disabled buyer (onay bekleyen)
                new UserEntity() { Id = 9, FirstName = "Deniz", LastName = "Koç", Email = "deniz@mail.com", Enabled = false, RoleId = 3, Password = "1234", CreatedAt = DateTime.UtcNow }
            );

            // ==================== CATEGORIES ====================
            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity() { Id = 1, Name = "Electronics", Color = "Blue", IconCssClass = "fa fa-fw fa-bolt", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 2, Name = "Clothing", Color = "Red", IconCssClass = "fa fa-fw fa-shopping-bag", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 3, Name = "Home", Color = "Green", IconCssClass = "fa fa-fw fa-home", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 4, Name = "Books", Color = "Orange", IconCssClass = "fa fa-fw fa-book", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 5, Name = "Health", Color = "Purple", IconCssClass = "fa fa-fw fa-heart", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 6, Name = "Sports", Color = "Yellow", IconCssClass = "fa fa-fw fa-trophy", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 7, Name = "Toys", Color = "Pink", IconCssClass = "fa fa-fw fa-child", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 8, Name = "Automotive", Color = "Grey", IconCssClass = "fa fa-fw fa-car", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 9, Name = "Furniture", Color = "Brown", IconCssClass = "fa fa-fw fa-tree", CreatedAt = DateTime.UtcNow },
                new CategoryEntity() { Id = 10, Name = "Food", Color = "Black", IconCssClass = "fa fa-fw fa-cutlery", CreatedAt = DateTime.UtcNow }
            );

            // ==================== PRODUCTS ====================
            modelBuilder.Entity<ProductEntity>().HasData(
                // Electronics (CategoryId: 1)
                new ProductEntity { Id = 1, SellerId = 2, CategoryId = 1, DDName = "Gaming Mouse Pro X", Price = 1499.99m, Details = "RGB aydınlatmalı, 16000 DPI profesyonel oyuncu mouse.", StockAmount = 25, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 2, SellerId = 2, CategoryId = 1, DDName = "Kablosuz Bluetooth Kulaklık", Price = 2599.00m, Details = "Aktif gürültü önleme, 30 saat pil ömrü.", StockAmount = 40, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 3, SellerId = 3, CategoryId = 1, DDName = "Mekanik Klavye RGB", Price = 3299.90m, Details = "Cherry MX Blue switch, tam boyut, alüminyum kasa.", StockAmount = 18, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Clothing (CategoryId: 2)
                new ProductEntity { Id = 4, SellerId = 3, CategoryId = 2, DDName = "Oversize Hoodie", Price = 899.90m, Details = "Unisex, %100 pamuklu oversize hoodie.", StockAmount = 50, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 5, SellerId = 3, CategoryId = 2, DDName = "Slim Fit Jean Pantolon", Price = 1199.00m, Details = "Yüksek bel, esnek kumaş, koyu mavi.", StockAmount = 35, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 6, SellerId = 4, CategoryId = 2, DDName = "Keten Gömlek", Price = 749.90m, Details = "Yazlık keten gömlek, nefes alan kumaş.", StockAmount = 60, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Home (CategoryId: 3)
                new ProductEntity { Id = 7, SellerId = 2, CategoryId = 3, DDName = "Akıllı Robot Süpürge", Price = 8999.00m, Details = "Lazer navigasyon, otomatik çöp boşaltma, uygulama kontrollü.", StockAmount = 12, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 8, SellerId = 4, CategoryId = 3, DDName = "Seramik Yemek Takımı 24 Parça", Price = 2499.90m, Details = "Porselen, bulaşık makinesi uyumlu, 6 kişilik.", StockAmount = 20, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Books (CategoryId: 4)
                new ProductEntity { Id = 9, SellerId = 2, CategoryId = 4, DDName = "Clean Code", Price = 599.00m, Details = "Robert C. Martin - Yazılım geliştiriciler için temiz kod rehberi.", StockAmount = 30, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 10, SellerId = 2, CategoryId = 4, DDName = "Design Patterns", Price = 549.90m, Details = "Gang of Four - Tasarım kalıpları klasiği.", StockAmount = 22, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 11, SellerId = 3, CategoryId = 4, DDName = "Suç ve Ceza", Price = 89.90m, Details = "Fyodor Dostoyevski, Türkçe çeviri, ciltli baskı.", StockAmount = 100, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Health (CategoryId: 5)
                new ProductEntity { Id = 12, SellerId = 4, CategoryId = 5, DDName = "Vitamin D3 1000 IU", Price = 249.90m, Details = "90 tablet, günlük D vitamini takviyesi.", StockAmount = 200, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 13, SellerId = 4, CategoryId = 5, DDName = "Dijital Tansiyon Aleti", Price = 1299.00m, Details = "Koldan ölçüm, LCD ekran, hafıza fonksiyonu.", StockAmount = 15, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Sports (CategoryId: 6)
                new ProductEntity { Id = 14, SellerId = 2, CategoryId = 6, DDName = "Fitness Dumbbell Set", Price = 1299.00m, Details = "Ayarlanabilir ağırlık seti, 2-24 kg arası.", StockAmount = 15, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 15, SellerId = 3, CategoryId = 6, DDName = "Yoga Matı 6mm", Price = 399.90m, Details = "Kaymaz yüzey, taşıma askılı, TPE malzeme.", StockAmount = 80, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 16, SellerId = 3, CategoryId = 6, DDName = "Koşu Bandı Katlanır", Price = 12999.00m, Details = "150 kg kapasite, 18 km/h max hız, Bluetooth hoparlör.", StockAmount = 5, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Toys (CategoryId: 7)
                new ProductEntity { Id = 17, SellerId = 4, CategoryId = 7, DDName = "Lego Teknik Araba Seti", Price = 1899.00m, Details = "1200+ parça, 12 yaş üstü, koleksiyonluk.", StockAmount = 10, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 18, SellerId = 4, CategoryId = 7, DDName = "Peluş Ayıcık 60cm", Price = 349.90m, Details = "Hypoalerjenik dolgu, yıkanabilir.", StockAmount = 45, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Automotive (CategoryId: 8)
                new ProductEntity { Id = 19, SellerId = 2, CategoryId = 8, DDName = "Araç İçi Telefon Tutucu", Price = 199.90m, Details = "360° dönebilir, havalandırma klipsli, tüm telefonlara uyumlu.", StockAmount = 120, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 20, SellerId = 2, CategoryId = 8, DDName = "Araç Kamerası Full HD", Price = 1799.00m, Details = "1080p kayıt, gece görüş, G-sensor, park modu.", StockAmount = 30, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Furniture (CategoryId: 9)
                new ProductEntity { Id = 21, SellerId = 3, CategoryId = 9, DDName = "Ahşap Kitaplık 5 Raflı", Price = 3499.99m, Details = "Modern tasarım, masif ahşap, ceviz renk.", StockAmount = 8, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 22, SellerId = 4, CategoryId = 9, DDName = "Ergonomik Ofis Sandalyesi", Price = 5999.00m, Details = "Bel desteği, ayarlanabilir kol dayama, mesh kumaş.", StockAmount = 14, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Food (CategoryId: 10)
                new ProductEntity { Id = 23, SellerId = 4, CategoryId = 10, DDName = "Organik Zeytinyağı 1L", Price = 449.90m, Details = "Soğuk sıkım, natürel sızma, Ege bölgesi.", StockAmount = 75, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 24, SellerId = 2, CategoryId = 10, DDName = "Filtre Kahve 1kg", Price = 599.00m, Details = "Öğütülmüş, Kolombiya çekirdeği, orta kavrulmuş.", StockAmount = 90, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 25, SellerId = 3, CategoryId = 10, DDName = "Kuru Meyve Paketi 500g", Price = 179.90m, Details = "Karışık kuru meyve, katkısız, doğal kurutma.", StockAmount = 110, CreatedAt = DateTime.UtcNow, Enabled = true },

                // Disabled ürün (admin panelinde test için)
                new ProductEntity { Id = 26, SellerId = 2, CategoryId = 1, DDName = "Eski Model Webcam", Price = 299.90m, Details = "720p, USB bağlantılı. (Satıştan kaldırıldı)", StockAmount = 0, CreatedAt = DateTime.UtcNow, Enabled = false },

                // Admin'e ait ürünler (ProfileController.MyProducts() hardcoded sellerId=1 için)
                new ProductEntity { Id = 27, SellerId = 1, CategoryId = 1, DDName = "Webcam HD 1080p", Price = 599.00m, Details = "Full HD webcam, dahili mikrofon, otomatik odaklama.", StockAmount = 20, CreatedAt = DateTime.UtcNow, Enabled = true },
                new ProductEntity { Id = 28, SellerId = 1, CategoryId = 4, DDName = "C# Programlama Rehberi", Price = 399.90m, Details = "Başlangıçtan ileri seviyeye C# programlama kitabı.", StockAmount = 50, CreatedAt = DateTime.UtcNow, Enabled = true }
            );

            // ==================== PRODUCT IMAGES ====================
            modelBuilder.Entity<ProductImageEntity>().HasData(
                // Electronics
                new ProductImageEntity { Id = 1, ProductId = 1, Url = "/images/products/gaming-mouse-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 2, ProductId = 1, Url = "/images/products/gaming-mouse-2.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 3, ProductId = 2, Url = "/images/products/bluetooth-kulaklik-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 4, ProductId = 3, Url = "/images/products/mekanik-klavye-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 5, ProductId = 3, Url = "/images/products/mekanik-klavye-2.jpg", CreatedAt = DateTime.UtcNow },
                // Clothing
                new ProductImageEntity { Id = 6, ProductId = 4, Url = "/images/products/oversize-hoodie-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 7, ProductId = 5, Url = "/images/products/slim-fit-jean-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 8, ProductId = 6, Url = "/images/products/keten-gomlek-1.jpg", CreatedAt = DateTime.UtcNow },
                // Home
                new ProductImageEntity { Id = 9, ProductId = 7, Url = "/images/products/robot-supurge-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 10, ProductId = 8, Url = "/images/products/yemek-takimi-1.jpg", CreatedAt = DateTime.UtcNow },
                // Books
                new ProductImageEntity { Id = 11, ProductId = 9, Url = "/images/products/clean-code-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 12, ProductId = 10, Url = "/images/products/design-patterns-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 13, ProductId = 11, Url = "/images/products/suc-ve-ceza-1.jpg", CreatedAt = DateTime.UtcNow },
                // Health
                new ProductImageEntity { Id = 14, ProductId = 12, Url = "/images/products/vitamin-d3-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 15, ProductId = 13, Url = "/images/products/tansiyon-aleti-1.jpg", CreatedAt = DateTime.UtcNow },
                // Sports
                new ProductImageEntity { Id = 16, ProductId = 14, Url = "/images/products/dumbbell-set-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 17, ProductId = 15, Url = "/images/products/yoga-mati-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 18, ProductId = 16, Url = "/images/products/kosu-bandi-1.jpg", CreatedAt = DateTime.UtcNow },
                // Toys
                new ProductImageEntity { Id = 19, ProductId = 17, Url = "/images/products/lego-teknik-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 20, ProductId = 18, Url = "/images/products/pelus-ayicik-1.jpg", CreatedAt = DateTime.UtcNow },
                // Automotive
                new ProductImageEntity { Id = 21, ProductId = 19, Url = "/images/products/telefon-tutucu-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 22, ProductId = 20, Url = "/images/products/arac-kamerasi-1.jpg", CreatedAt = DateTime.UtcNow },
                // Furniture
                new ProductImageEntity { Id = 23, ProductId = 21, Url = "/images/products/ahsap-kitaplik-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 24, ProductId = 22, Url = "/images/products/ofis-sandalyesi-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 25, ProductId = 22, Url = "/images/products/ofis-sandalyesi-2.jpg", CreatedAt = DateTime.UtcNow },
                // Food
                new ProductImageEntity { Id = 26, ProductId = 23, Url = "/images/products/zeytinyagi-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 27, ProductId = 24, Url = "/images/products/filtre-kahve-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 28, ProductId = 25, Url = "/images/products/kuru-meyve-1.jpg", CreatedAt = DateTime.UtcNow },
                // Admin ürünleri
                new ProductImageEntity { Id = 29, ProductId = 27, Url = "/images/products/webcam-hd-1.jpg", CreatedAt = DateTime.UtcNow },
                new ProductImageEntity { Id = 30, ProductId = 28, Url = "/images/products/csharp-kitap-1.jpg", CreatedAt = DateTime.UtcNow }
            );

            // ==================== PRODUCT COMMENTS ====================
            modelBuilder.Entity<ProductCommentEntity>().HasData(
                // Gaming Mouse
                new ProductCommentEntity { Id = 1, ProductId = 1, UserId = 5, Text = "Harika bir mouse, DPI ayarları çok hassas. Oyunlarda fark yarattı!", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 2, ProductId = 1, UserId = 6, Text = "Fiyat/performans oranı güzel ama biraz ağır geldi.", StarCount = 4, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Bluetooth Kulaklık
                new ProductCommentEntity { Id = 3, ProductId = 2, UserId = 7, Text = "Gürültü önleme mükemmel, uzun yolculuklarda vazgeçilmez.", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 4, ProductId = 2, UserId = 8, Text = "Ses kalitesi iyi ama kulak üstü baskı yapıyor.", StarCount = 3, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Oversize Hoodie
                new ProductCommentEntity { Id = 5, ProductId = 4, UserId = 5, Text = "Kumaşı çok yumuşak, tam istediğim kalıp!", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 6, ProductId = 4, UserId = 6, Text = "İkinci yıkamada biraz çekti, dikkat edin.", StarCount = 3, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Clean Code
                new ProductCommentEntity { Id = 7, ProductId = 9, UserId = 5, Text = "Her yazılımcının okuması gereken bir kitap. Harika!", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Robot Süpürge
                new ProductCommentEntity { Id = 8, ProductId = 7, UserId = 7, Text = "Ev temizliğini tamamen değiştirdi, navigasyonu çok akıllı.", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 9, ProductId = 7, UserId = 8, Text = "Halı kenarlarında biraz zorlanıyor ama genel olarak memnunum.", StarCount = 4, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Yoga Matı
                new ProductCommentEntity { Id = 10, ProductId = 15, UserId = 5, Text = "Kaymaz yüzeyi gerçekten işe yarıyor, ter de tutmuyor.", StarCount = 4, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Lego Teknik
                new ProductCommentEntity { Id = 11, ProductId = 17, UserId = 6, Text = "Oğluma aldım, birlikte yaptık, harika vakit geçirdik!", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Ergonomik Sandalye
                new ProductCommentEntity { Id = 12, ProductId = 22, UserId = 7, Text = "Bel ağrılarım geçti, ofiste mutlaka olması gereken bir ürün.", StarCount = 5, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Filtre Kahve
                new ProductCommentEntity { Id = 13, ProductId = 24, UserId = 8, Text = "Aroması harika, sabah kahvesi için birebir.", StarCount = 4, IsConfirmed = true, CreatedAt = DateTime.UtcNow },

                // Onay bekleyen yorumlar (admin paneli testi için)
                new ProductCommentEntity { Id = 14, ProductId = 3, UserId = 6, Text = "Switch sesleri biraz fazla gürültülü ofis ortamı için.", StarCount = 3, IsConfirmed = false, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 15, ProductId = 19, UserId = 5, Text = "Klips biraz gevşek, telefon düşebilir dikkat!", StarCount = 2, IsConfirmed = false, CreatedAt = DateTime.UtcNow },
                new ProductCommentEntity { Id = 16, ProductId = 12, UserId = 8, Text = "Düzenli kullanınca fark ettim, enerji seviyem arttı.", StarCount = 4, IsConfirmed = false, CreatedAt = DateTime.UtcNow }
            );

            // ==================== ORDERS ====================
            modelBuilder.Entity<OrderEntity>().HasData(
                // Zeynep'in siparişleri
                new OrderEntity { Id = 1, UserId = 5, TotalPrice = 4098.99m, Status = "Teslim Edildi", OrderCode = "ORD-2025-0001", Address = "Kadıköy Mah. Bağdat Cad. No:45 Daire:3, İstanbul", CreatedAt = DateTime.UtcNow },
                new OrderEntity { Id = 2, UserId = 5, TotalPrice = 899.90m, Status = "Kargoda", OrderCode = "ORD-2025-0002", Address = "Kadıköy Mah. Bağdat Cad. No:45 Daire:3, İstanbul", CreatedAt = DateTime.UtcNow },

                // Can'ın siparişleri
                new OrderEntity { Id = 3, UserId = 6, TotalPrice = 14898.00m, Status = "Teslim Edildi", OrderCode = "ORD-2025-0003", Address = "Çankaya Mah. Atatürk Blv. No:120, Ankara", CreatedAt = DateTime.UtcNow },
                new OrderEntity { Id = 4, UserId = 6, TotalPrice = 1899.00m, Status = "Hazırlanıyor", OrderCode = "ORD-2025-0004", Address = "Çankaya Mah. Atatürk Blv. No:120, Ankara", CreatedAt = DateTime.UtcNow },

                // Selin'in siparişleri
                new OrderEntity { Id = 5, UserId = 7, TotalPrice = 9598.00m, Status = "Teslim Edildi", OrderCode = "ORD-2025-0005", Address = "Alsancak Mah. Kordon Cad. No:78, İzmir", CreatedAt = DateTime.UtcNow },
                new OrderEntity { Id = 6, UserId = 7, TotalPrice = 449.90m, Status = "Kargoda", OrderCode = "ORD-2025-0006", Address = "Alsancak Mah. Kordon Cad. No:78, İzmir", CreatedAt = DateTime.UtcNow },

                // Burak'ın siparişleri
                new OrderEntity { Id = 7, UserId = 8, TotalPrice = 3036.70m, Status = "Hazırlanıyor", OrderCode = "ORD-2025-0007", Address = "Nilüfer Mah. Özlüce Cad. No:15 Daire:7, Bursa", CreatedAt = DateTime.UtcNow },
                new OrderEntity { Id = 8, UserId = 8, TotalPrice = 12999.00m, Status = "İptal Edildi", OrderCode = "ORD-2025-0008", Address = "Nilüfer Mah. Özlüce Cad. No:15 Daire:7, Bursa", CreatedAt = DateTime.UtcNow },

                // Admin'in siparişi (ProfileController.MyOrders() hardcoded userId=1 için)
                new OrderEntity { Id = 9, UserId = 1, TotalPrice = 1499.99m, Status = "Teslim Edildi", OrderCode = "ORD-2025-0009", Address = "Merkez Mah. Admin Cad. No:1, İstanbul", CreatedAt = DateTime.UtcNow },
                new OrderEntity { Id = 10, UserId = 1, TotalPrice = 899.90m, Status = "Kargoda", OrderCode = "ORD-2025-0010", Address = "Merkez Mah. Admin Cad. No:1, İstanbul", CreatedAt = DateTime.UtcNow }
            );

            // ==================== ORDER ITEMS ====================
            modelBuilder.Entity<OrderItemEntity>().HasData(
                // Sipariş 1: Zeynep - Gaming Mouse + Bluetooth Kulaklık = 4098.99
                new OrderItemEntity { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 1499.99m, CreatedAt = DateTime.UtcNow },
                new OrderItemEntity { Id = 2, OrderId = 1, ProductId = 2, Quantity = 1, UnitPrice = 2599.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 2: Zeynep - Oversize Hoodie = 899.90
                new OrderItemEntity { Id = 3, OrderId = 2, ProductId = 4, Quantity = 1, UnitPrice = 899.90m, CreatedAt = DateTime.UtcNow },

                // Sipariş 3: Can - Koşu Bandı + Lego Teknik = 14898.00
                new OrderItemEntity { Id = 4, OrderId = 3, ProductId = 16, Quantity = 1, UnitPrice = 12999.00m, CreatedAt = DateTime.UtcNow },
                new OrderItemEntity { Id = 5, OrderId = 3, ProductId = 17, Quantity = 1, UnitPrice = 1899.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 4: Can - Lego Teknik = 1899.00
                new OrderItemEntity { Id = 6, OrderId = 4, ProductId = 17, Quantity = 1, UnitPrice = 1899.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 5: Selin - Robot Süpürge + Clean Code = 9598.00
                new OrderItemEntity { Id = 7, OrderId = 5, ProductId = 7, Quantity = 1, UnitPrice = 8999.00m, CreatedAt = DateTime.UtcNow },
                new OrderItemEntity { Id = 8, OrderId = 5, ProductId = 9, Quantity = 1, UnitPrice = 599.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 6: Selin - Zeytinyağı = 449.90
                new OrderItemEntity { Id = 9, OrderId = 6, ProductId = 23, Quantity = 1, UnitPrice = 449.90m, CreatedAt = DateTime.UtcNow },

                // Sipariş 7: Burak - Filtre Kahve x2 + Kuru Meyve x3 + Dumbbell = 3036.70
                new OrderItemEntity { Id = 10, OrderId = 7, ProductId = 24, Quantity = 2, UnitPrice = 599.00m, CreatedAt = DateTime.UtcNow },
                new OrderItemEntity { Id = 11, OrderId = 7, ProductId = 25, Quantity = 3, UnitPrice = 179.90m, CreatedAt = DateTime.UtcNow },
                new OrderItemEntity { Id = 12, OrderId = 7, ProductId = 14, Quantity = 1, UnitPrice = 1299.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 8: Burak - İptal edilen = 12999.00
                new OrderItemEntity { Id = 13, OrderId = 8, ProductId = 16, Quantity = 1, UnitPrice = 12999.00m, CreatedAt = DateTime.UtcNow },

                // Sipariş 9: Admin - Gaming Mouse = 1499.99
                new OrderItemEntity { Id = 14, OrderId = 9, ProductId = 1, Quantity = 1, UnitPrice = 1499.99m, CreatedAt = DateTime.UtcNow },

                // Sipariş 10: Admin - Oversize Hoodie = 899.90
                new OrderItemEntity { Id = 15, OrderId = 10, ProductId = 4, Quantity = 1, UnitPrice = 899.90m, CreatedAt = DateTime.UtcNow }
            );

            // ==================== CART ITEMS ====================
            modelBuilder.Entity<CartItemEntity>().HasData(
                // Zeynep'in sepeti
                new CartItemEntity { Id = 1, UserId = 5, ProductId = 22, Quantity = 1, CreatedAt = DateTime.UtcNow },
                new CartItemEntity { Id = 2, UserId = 5, ProductId = 15, Quantity = 2, CreatedAt = DateTime.UtcNow },

                // Can'ın sepeti
                new CartItemEntity { Id = 3, UserId = 6, ProductId = 20, Quantity = 1, CreatedAt = DateTime.UtcNow },
                new CartItemEntity { Id = 4, UserId = 6, ProductId = 24, Quantity = 3, CreatedAt = DateTime.UtcNow },

                // Selin'in sepeti
                new CartItemEntity { Id = 5, UserId = 7, ProductId = 6, Quantity = 2, CreatedAt = DateTime.UtcNow },

                // Burak'ın sepeti
                new CartItemEntity { Id = 6, UserId = 8, ProductId = 11, Quantity = 1, CreatedAt = DateTime.UtcNow },
                new CartItemEntity { Id = 7, UserId = 8, ProductId = 13, Quantity = 1, CreatedAt = DateTime.UtcNow }
            );

            // ==================== ENTITY CONFIGURATIONS ====================
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CartItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
        }
    }
}
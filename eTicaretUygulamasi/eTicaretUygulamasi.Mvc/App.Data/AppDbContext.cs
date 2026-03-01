using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages{ get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItemS { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bu satır her zaman en başta bir kez durmalı
            base.OnModelCreating(modelBuilder);

            // 1. Sipariş Öğeleri (OrderItem) ve Ürünler arasındaki çakışmayı gider
            modelBuilder.Entity<OrderItemEntity>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Sepet Öğeleri (CartItem) ve Kullanıcılar arasındaki çakışmayı gider
            modelBuilder.Entity<CartItemEntity>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Ürün Yorumları (ProductComment) ve Kullanıcılar arasındaki çakışmayı gider
            // (Görsel 65cb10'daki hatayı bu satır çözer)
            modelBuilder.Entity<ProductCommentEntity>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Eğer tablolarında başka özel kısıtlamalar (MaxLength, Required vb.) varsa 
            // onları da buranın altına eklemeye devam edebilirsin.
        }
    }
}

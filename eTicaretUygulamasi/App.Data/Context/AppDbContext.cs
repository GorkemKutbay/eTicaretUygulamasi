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

            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity() { Id = 1, Name = "admin", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 2, Name = "seller", CreatedAt = DateTime.UtcNow },
                new RoleEntity() { Id = 3, Name = "buyer", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity() { Id = 1, FirstName = "admin", LastName = "admin", Email = "admin@mail.com", Enabled = true, RoleId = 1, Password = "1234", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<CategoryEntity>().HasData(
                new List<CategoryEntity>{
                new() { Id = 1, Name = "Electronics", Color = "Blue", IconCssClass = "fa fa-fw fa-bolt", CreatedAt = DateTime.UtcNow },
                new() { Id = 2, Name = "Clothing", Color = "Red", IconCssClass = "fa fa-fw fa-shopping-bag", CreatedAt = DateTime.UtcNow },
                new() { Id = 3, Name = "Home", Color = "Green", IconCssClass = "fa fa-fw fa-home", CreatedAt = DateTime.UtcNow },
                new() { Id = 4, Name = "Books", Color = "Orange", IconCssClass = "fa fa-fw fa-book", CreatedAt = DateTime.UtcNow },
                new() { Id = 5, Name = "Health", Color = "Purple", IconCssClass = "fa fa-fw fa-heart", CreatedAt = DateTime.Now },
                new() { Id = 6, Name = "Sports", Color = "Yellow", IconCssClass = "fa fa-fw fa-trophy", CreatedAt = DateTime.UtcNow },
                new() { Id = 7, Name = "Toys", Color = "Pink", IconCssClass = "fa fa-fw fa-child", CreatedAt = DateTime.UtcNow },
                new() { Id = 8, Name = "Automotive", Color = "Grey", IconCssClass = "fa fa-fw fa-car", CreatedAt = DateTime.UtcNow },
                new() { Id = 9, Name = "Furniture", Color = "Brown", IconCssClass = "fa fa-fw fa-tree", CreatedAt = DateTime.UtcNow },
                new() { Id = 10, Name = "Food", Color = "Black", IconCssClass = "fa fa-fw fa-cutlery", CreatedAt = DateTime.UtcNow }
                }
             );
            modelBuilder.Entity<ProductEntity>().HasData(
            new ProductEntity { Id = 1, SellerId = 1, CategoryId = 1, DDName = "Gaming Mouse Pro X", Price = 1499.99m, Details = "RGB aydınlatmalı, 16000 DPI profesyonel oyuncu mouse.", StockAmount = 25, CreatedAt = DateTime.UtcNow, Enabled = true },
            new ProductEntity { Id = 2, SellerId = 1, CategoryId = 2, DDName = "Oversize Hoodie", Price = 899.90m, Details = "Unisex, pamuklu oversize hoodie.", StockAmount = 50, CreatedAt = DateTime.UtcNow, Enabled = true },
            new ProductEntity { Id = 3, SellerId = 1, CategoryId = 4, DDName = "Clean Code Kitabı", Price = 599.00m, Details = "Yazılım geliştiriciler için temiz kod rehberi.", StockAmount = 30, CreatedAt = DateTime.UtcNow, Enabled = true },
            new ProductEntity { Id = 4, SellerId = 1, CategoryId = 6, DDName = "Fitness Dumbbell Set", Price = 1299.00m, Details = "Ayarlanabilir ağırlık seti.", StockAmount = 15, CreatedAt = DateTime.UtcNow, Enabled = true },
            new ProductEntity { Id = 5, SellerId = 1, CategoryId = 3, DDName = "Ahşap Kitaplık", Price = 3499.99m, Details = "Modern tasarım 5 raflı kitaplık.", StockAmount = 8, CreatedAt = DateTime.UtcNow, Enabled = true }
);

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

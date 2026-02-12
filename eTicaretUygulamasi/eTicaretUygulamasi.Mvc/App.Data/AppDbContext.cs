using Microsoft.EntityFrameworkCore;

namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class AppDbContext:DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntitiy> Roles { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages{ get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<CategoryEntity> CategoryEntities { get; set; }
        public DbSet<CartItemEntity> CartItemEntities { get; set; }
        public DbSet<OrderEntity> OrderEntities { get; set; }
        public DbSet<OrderItemEntity> OrderItemEntities { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
            
        }
    }
}

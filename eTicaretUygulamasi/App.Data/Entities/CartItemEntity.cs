using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class CartItemEntity
    {

        public int Id { get; set; }


        public int UserId { get; set; }


        public int ProductId { get; set; }


        [Range(1, 255)] 
        public byte Quantity { get; set; }


        public DateTime CreatedAt { get; set; }

       
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; } = null!;
        
    }
    internal class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
    {
        public void Configure(EntityTypeBuilder<CartItemEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Quantity).IsRequired().HasDefaultValue(1); // 1 item by default
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
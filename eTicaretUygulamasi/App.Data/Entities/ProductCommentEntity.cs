using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductCommentEntity
    {
       
     
        public int Id { get; set; }


        public int ProductId { get; set; }

   
        public int UserId { get; set; }


        public string Text { get; set; }

   
        public byte StarCount { get; set; }


        public bool IsConfirmed { get; set; } = false; // default:false kuralı için

  
        public DateTime CreatedAt { get; set; }

        
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; }

        
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }
        
    }
    internal class ProductCommentEntityConfiguration : IEntityTypeConfiguration<ProductCommentEntity>
    {
        public void Configure(EntityTypeBuilder<ProductCommentEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.Text).IsRequired().HasMaxLength(500);
            builder.Property(e => e.StarCount).IsRequired().HasDefaultValue(3); // 3 stars by default
            builder.Property(e => e.IsConfirmed).IsRequired().HasDefaultValue(false); // Not confirmed by default
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

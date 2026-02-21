using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductImageEntity
    {
        
        public int Id { get; set; }

        
        public int ProductId { get; set; }

        
        public string Url { get; set; }

     
        public DateTime CreatedAt { get; set; }

        
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; }
        


    }
    internal class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImageEntity>
    {
        public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Url).IsRequired().HasMaxLength(250);
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductEntity
    {
    
        public int Id { get; set; }

       
        public int SellerId { get; set; }
        public UserEntity Seller { get; set; } 

     
        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; } = null!;

   
        public string DDName { get; set; }


        public decimal Price { get; set; }

   
        public string Details { get; set; }


        public byte StockAmount { get; set; }

  
        public DateTime CreatedAt { get; set; }

        
        public bool Enabled { get; set; } = true;
        
        
    }
    internal class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.SellerId).IsRequired();
            builder.Property(e => e.CategoryId).IsRequired();
            builder.Property(e => e.DDName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.Details).HasMaxLength(1000);
            builder.Property(e => e.StockAmount).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.Enabled).IsRequired().HasDefaultValue(true);

            builder.HasOne(d => d.Seller)
                .WithMany()
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class OrderItemEntity
    {
       
        public int Id { get; set; }


        public int OrderId { get; set; }

        
        public OrderEntity Order { get; set; }

       
        public int ProductId { get; set; }

        
        public ProductEntity Product { get; set; }

  
        public byte Quantity { get; set; }

      
        public decimal UnitPrice { get; set; }

   
        public DateTime CreatedAt { get; set; }
        
    }
    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.OrderId).IsRequired();
            builder.Property(e => e.ProductId).IsRequired();
            builder.Property(e => e.Quantity).IsRequired().HasDefaultValue(1); // 1 item by default
            builder.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(d => d.Order)
                .WithMany()
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

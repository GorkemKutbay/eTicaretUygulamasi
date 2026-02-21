using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class OrderEntity
    {
     
        public int Id { get; set; }


  
        public int UserId { get; set; }

     
        public UserEntity User { get; set; }


      
        public string OrderCode { get; set; }


       
        public string Address { get; set; }



        public DateTime CreatedAt { get; set; }
        
    }
    internal class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.OrderCode).IsRequired().HasMaxLength(250);
            builder.Property(e => e.Address).IsRequired().HasMaxLength(250);
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

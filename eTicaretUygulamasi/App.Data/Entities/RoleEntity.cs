using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class RoleEntity
    {
       
        public int Id { get; set; }

   
        public string Name { get; set; }

        
        public DateTime CreatedAt { get; set; }
        

    }
    internal class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(10);
            builder.Property(e => e.CreatedAt).IsRequired();
        }

    }
}

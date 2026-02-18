using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class CartItemEntity
    {
        [Required, Key]
        public int Id { get; set; }

        [Required] 
        public int UserId { get; set; }

        [Required] 
        public int ProductId { get; set; }

        [Required]
        [Range(1, 255)] 
        public byte Quantity { get; set; }

        [Required] 
        public DateTime CreatedAt { get; set; }

       
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; } = null!;
    }
}
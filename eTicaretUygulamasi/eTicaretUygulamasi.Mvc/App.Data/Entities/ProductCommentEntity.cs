using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductCommentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)] // min:2, max:500 kuralı için
        public string Text { get; set; }

        [Required]
        [Range(1, 5)] 
        public byte StarCount { get; set; }

        [Required]
        public bool IsConfirmed { get; set; } = false; // default:false kuralı için

        [Required]
        public DateTime CreatedAt { get; set; }

        
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; }

        
        [ForeignKey("UserId")]
        public virtual UserEntity User { get; set; }
    }
}

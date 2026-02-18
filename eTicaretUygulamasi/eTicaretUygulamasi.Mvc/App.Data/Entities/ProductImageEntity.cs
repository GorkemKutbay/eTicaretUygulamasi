using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductImageEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Url]
        [StringLength(250, MinimumLength = 10)] // min:10, max:250 kuralı için
        public string Url { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        
        [ForeignKey("ProductId")]
        public virtual ProductEntity Product { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class ProductEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int SellerId { get; set; }
        public UserEntity Seller { get; set; } 

        [Required]
        public int CategoryId { get; set; }

        public CategoryEntity? Category { get; set; }

        [Required,Range(2,100)]
        public string DDName { get; set; }

        [Required,DataType(DataType.Currency),Range(0.01,double.MaxValue,ErrorMessage ="Negatif Fiyat girilemez!!!")]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string Details { get; set; }

        [Required]
        public byte StockAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool Enabled { get; set; } = true;
    }
}

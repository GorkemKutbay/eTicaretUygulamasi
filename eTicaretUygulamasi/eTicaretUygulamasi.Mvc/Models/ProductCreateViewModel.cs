using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Ürün adı boş geçilemez.")]
        public string Name { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public string? Details { get; set; }
    }
}

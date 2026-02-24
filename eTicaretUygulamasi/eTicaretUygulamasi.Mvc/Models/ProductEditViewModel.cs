using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı boş olamaz!")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter!")]
        public string DDName { get; set; }

        [Required(ErrorMessage = "Fiyat boş olamaz!")]
        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0'dan büyük olmalı!")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "En fazla 1000 karakter!")]
        public string? Details { get; set; }

        [Required(ErrorMessage = "Stok boş olamaz!")]
        [Range(0, 255, ErrorMessage = "Stok 0-255 arası olmalı!")]
        public byte StockAmount { get; set; }

        [Required(ErrorMessage = "Kategori seçilmeli!")]
        public int CategoryId { get; set; }

        public int SellerId { get; set; }
    }
}

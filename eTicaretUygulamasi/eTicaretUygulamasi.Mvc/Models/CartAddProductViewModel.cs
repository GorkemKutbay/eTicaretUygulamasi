using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class CartAddProductViewModel
    {
        [Required(ErrorMessage = "Ürün bilgisi gereklidir!")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Adet bilgisi gereklidir!")]
        [Range(1, 255, ErrorMessage = "Adet 1 ile 255 arasında olmalıdır!")]
        public byte Quantity { get; set; } = 1;



        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string CategoryName { get; set; }
    }
}

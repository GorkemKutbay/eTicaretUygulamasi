using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class CartEditViewModel
    {
        public List<CartEditItemViewModel> Items { get; set; } = new List<CartEditItemViewModel>();
      
        public decimal TotalPrice => Items?.Sum(i => i.Price * i.Quantity) ?? 0;
    }

    
    public class CartEditItemViewModel
    {       
        public int Id { get; set; }        
        public int ProductId { get; set; }     
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Adet bilgisi gereklidir!")]
        [Range(1, 255, ErrorMessage = "Adet 1 ile 255 arasında olmalıdır!")]
        public byte Quantity { get; set; }

    }
}

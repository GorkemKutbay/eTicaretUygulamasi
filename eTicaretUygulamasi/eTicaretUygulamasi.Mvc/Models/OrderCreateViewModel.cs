using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class OrderCreateViewModel
    {
        [Required(ErrorMessage = "Adres alanı zorunludur!")]
        [StringLength(250, ErrorMessage = "Adres en fazla 250 karakter olabilir!")]
        public string Address { get; set; } = string.Empty;
       
        public List<OrderCreateItemViewModel> Items { get; set; } = new List<OrderCreateItemViewModel>();
        public decimal TotalPrice => Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0;
    }

    public class OrderCreateItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
    }
}

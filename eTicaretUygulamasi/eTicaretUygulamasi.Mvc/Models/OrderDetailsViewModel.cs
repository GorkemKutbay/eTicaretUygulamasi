namespace eTicaretUygulamasi.Mvc.Models
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }

        public List<OrderDetailsItemViewModel> Items { get; set; } = new List<OrderDetailsItemViewModel>();
    }

    public class OrderDetailsItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public byte Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
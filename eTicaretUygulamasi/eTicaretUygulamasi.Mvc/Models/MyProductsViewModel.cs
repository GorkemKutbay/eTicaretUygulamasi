namespace eTicaretUygulamasi.Mvc.Models
{
    public class MyProductsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool Enabledd { get; set; } = true;
    }
}

namespace eTicaretUygulamasi.Mvc.Models
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}

namespace Admin.Models
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }
        public string DDName { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public byte StockAmount { get; set; }
    }
}

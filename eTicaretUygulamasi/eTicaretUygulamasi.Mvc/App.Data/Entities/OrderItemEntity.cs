using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class OrderItemEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))] // ForeignKey attribute to specify the relationship
        public OrderEntity Order { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))] // ForeignKey attribute to specify the relationship
        public ProductEntity Product { get; set; }

        [Required, Range(1, byte.MaxValue)]
        public byte Quantity { get; set; }

        [Required, DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

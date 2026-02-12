using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class OrderEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]   
        public int Id { get; set; }


        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))] // ForeignKey attribute to specify the relationship
        public UserEntity User { get; set; }


        [Required, MinLength(2)]
        public string OrderCode { get; set; }


        [Required, MinLength(2), MaxLength(250)]
        public string Address { get; set; }


        [Required]
        public DateTime CreatedAt { get; set; }
    }
}

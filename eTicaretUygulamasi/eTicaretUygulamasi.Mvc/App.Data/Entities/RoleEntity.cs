using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class RoleEntity
    {
        [Required]
        public int Id { get; set; }

        [Required, Range(2, 10)]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

    }
}

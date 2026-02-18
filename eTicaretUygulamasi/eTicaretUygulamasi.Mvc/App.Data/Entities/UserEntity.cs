using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTicaretUygulamasi.Mvc.App.Data.Entities
{
    public class UserEntity
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required,Range(2,50)]
        public string FirstName { get; set; }

        [Required, Range(2, 50)]
        public string LastName { get; set; }

        [Required,MinLength(1)]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }
        public RoleEntity Role { get; set; }

        [Required]
        public bool Enabled { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; }





    }
}

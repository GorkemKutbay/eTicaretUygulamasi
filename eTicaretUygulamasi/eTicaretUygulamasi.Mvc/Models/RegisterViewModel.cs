using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class RegisterViewModel
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; }

        [Required,MaxLength(50)]
        public string LastName { get; set; }

        [Required,MaxLength(256),EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required,Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public int RoleId { get; set; }

    }
}

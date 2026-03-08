using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Models.ViewModels
{
    public class ProfileEditViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçersiz e-posta formatı.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Geçersiz telefon formatı.")]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
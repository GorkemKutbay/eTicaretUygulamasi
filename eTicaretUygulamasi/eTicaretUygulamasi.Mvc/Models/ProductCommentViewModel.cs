using System.ComponentModel.DataAnnotations;

namespace eTicaretUygulamasi.Mvc.Models
{
    public class ProductCommentViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Yorum yazmalısınız!")]
        [StringLength(500, ErrorMessage = "En fazla 500 karakter!")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Yıldız seçmelisiniz!")]
        [Range(1, 5, ErrorMessage = "Yıldız 1-5 arası olmalı!")]
        public byte StarCount { get; set; }
    }
}

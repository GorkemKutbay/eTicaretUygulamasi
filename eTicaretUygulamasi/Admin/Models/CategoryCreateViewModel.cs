using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Kategori adı boş olamaz!")]
        [StringLength(50, ErrorMessage = "En fazla 50 karakter girebilirsiniz.")]
        public string Name { get; set; }
    }
}

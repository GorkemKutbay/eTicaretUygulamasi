using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class CategoryEditViewModel
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı boş olamaz!")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter girebilirsiniz.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Renk boş olamaz!")]
        [StringLength(6, ErrorMessage = "En fazla 6 karakter girebilirsiniz.")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "İkon CSS sınıfı boş olamaz!")]
        [StringLength(50, ErrorMessage = "En fazla 50 karakter girebilirsiniz.")]
        public string IconCssClass { get; set; } = null!;
    }
}

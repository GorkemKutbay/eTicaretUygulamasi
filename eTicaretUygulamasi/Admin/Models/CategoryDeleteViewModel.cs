using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class CategoryDeleteViewModel
    {
        [Required]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public DateTime DateTime { get; set; }


    }
}

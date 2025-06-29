using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class UpdateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(7)]
        public string Color { get; set; }
    }
}

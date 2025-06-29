using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(7)] // HEX color format
        public string Color { get; set; }
    }

}

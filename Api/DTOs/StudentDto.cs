using System.ComponentModel.DataAnnotations;

namespace Api.DTOs
{
    public class StudentDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Email { get; set; }
    }
}

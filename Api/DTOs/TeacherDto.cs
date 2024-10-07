using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class TeacherDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    public string Email { get; set; }
}

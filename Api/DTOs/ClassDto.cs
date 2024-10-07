using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class ClassDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; }

    [Range(5, 50)]
    public int Capacity { get; set; }
}

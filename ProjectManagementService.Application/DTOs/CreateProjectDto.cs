using System.ComponentModel.DataAnnotations;

namespace ProjectManagementService.Application.DTOs;

public class CreateProjectDto
{
    [Required]
    public long OwnerId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string? DemoUrl { get; set; }
    
    public DateOnly? StartDate { get; set; }
    
    public DateOnly? EndDate { get; set; }
    
    [MaxLength(500)]
    public string? ShortIntro { get; set; }
    
    [MaxLength(500)]
    public string? Highlight { get; set; }
    
    public string? Description { get; set; }
}

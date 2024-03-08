using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Employee
{
    [Column("EmployeeId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(30, ErrorMessage = "Name can have a maximum of 60 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Age is required.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    [MaxLength(20, ErrorMessage = "Position can have a maximum of 20 characters.")]
    public string? Position { get; set; }

    [ForeignKey(nameof(Company))]
    public Guid CompanyId { get; set; }

    public Company? Company { get; set; }
}

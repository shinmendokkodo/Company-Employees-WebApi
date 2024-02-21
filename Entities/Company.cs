using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

public class Company
{
    [Column("CompanyId")]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(60, ErrorMessage = "Name can have a maximum of 60 characters.")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Address is required.")]
    [MaxLength(60, ErrorMessage = "Address can have a maximum of 60 characters.")]
    public string? Address { get; set; }
    
    public string? Country { get; set; }
    
    public ICollection<Employee>? Employees { get; set; }
    
    
}
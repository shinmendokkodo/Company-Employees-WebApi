namespace Shared.DataTransferObjects;

public record EmployeeDto : BaseDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; } 
    public int Age { get; init; }
    public string? Position { get; init; }
    
    public override string ToCsvString() => $"\"{Id}\", \"{Name}\", \"{Age}\", \"{Position}\"";
}
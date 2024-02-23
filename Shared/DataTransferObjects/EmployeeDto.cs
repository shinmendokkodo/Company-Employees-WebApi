namespace Shared.DataTransferObjects;

public record EmployeeDto(Guid Id, string Name, int Age, string Position) : BaseDto
{
    public override string ToCsvString() => $"\"{Id}\", \"{Name}\", \"{Age}\", \"{Position}\"";
}

namespace Shared.DataTransferObjects;

public record EmployeeDto(Guid Id, string Name, int Age, string Position) : BaseDto
{
    public override string ToCsvString()
    {
        return $"\"{Id}\", \"{Name}\", \"{Age}\", \"{Position}\"";
    }
}
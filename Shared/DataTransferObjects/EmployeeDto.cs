using System.Runtime.Serialization;

namespace Shared.DataTransferObjects;

[DataContract(Name = "Employee")]
public record EmployeeDto
{
    [DataMember]
    public Guid Id { get; init; }

    [DataMember]
    public string? Name { get; init; }

    [DataMember]
    public int Age { get; init; }

    [DataMember]
    public string? Position { get; init; }
}
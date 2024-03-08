using System.Runtime.Serialization;

namespace Shared.DataTransferObjects;

[DataContract(Name = "Company")]
public record CompanyDto
{
    [DataMember]
    public Guid Id { get; init; }

    [DataMember]
    public string? Name { get; init; }

    [DataMember]
    public string? FullAddress { get; init; }
}
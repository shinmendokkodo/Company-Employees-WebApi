namespace Shared.DataTransferObjects;

public record CompanyDto : BaseDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? FullAddress { get; init; }

    public override string ToCsvString() => $"\"{Id}\", \"{Name}\", \"{FullAddress}\"";
}
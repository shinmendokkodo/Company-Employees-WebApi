namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "name";

    public uint MinAge { get; set; }
    public uint MaxAge { get; set; } = 65;

    public bool ValidAgeRange => MaxAge > MinAge;

    public string? SearchTerm { get; set; }
}

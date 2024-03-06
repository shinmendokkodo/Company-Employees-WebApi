namespace Entities.Exceptions;

public sealed class EmployeeCollectionBadRequestException()
    : BadRequestException("Employee collection sent from a client is null.");

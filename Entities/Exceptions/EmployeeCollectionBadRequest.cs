namespace Entities.Exceptions;

public sealed class EmployeeCollectionBadRequest() : BadRequestException("Employee collection sent from a client is null.");

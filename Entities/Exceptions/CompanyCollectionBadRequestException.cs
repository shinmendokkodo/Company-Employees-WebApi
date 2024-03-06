namespace Entities.Exceptions;

public sealed class CompanyCollectionBadRequestException()
    : BadRequestException("Company collection sent from a client is null.");

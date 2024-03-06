namespace Entities.Exceptions;

public class UnprocessableRequestException(string? message) : UnprocessableEntityException(message);
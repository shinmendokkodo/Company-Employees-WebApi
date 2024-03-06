namespace Entities.Exceptions;

public abstract class UnprocessableEntityException(string? message) : Exception(message);
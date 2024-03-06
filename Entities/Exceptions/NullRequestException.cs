namespace Entities.Exceptions;

public sealed class NullRequestException(string message)
    : BadRequestException(message);
using Entities.Exceptions;

namespace Service;

public sealed class MaxAgeRangeBadRequestException() : BadRequestException("Max age can't be less than min age.");
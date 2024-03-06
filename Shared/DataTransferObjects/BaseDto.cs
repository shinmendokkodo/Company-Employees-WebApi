namespace Shared.DataTransferObjects;

public abstract record BaseDto
{
    public abstract string ToCsvString();
}
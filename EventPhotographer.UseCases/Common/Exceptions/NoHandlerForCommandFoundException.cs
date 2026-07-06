namespace EventPhotographer.UseCases.Common.Exceptions;

public class NoHandlerForCommandFoundException : Exception
{
    public NoHandlerForCommandFoundException(string? message = null) : base(message) { }
}

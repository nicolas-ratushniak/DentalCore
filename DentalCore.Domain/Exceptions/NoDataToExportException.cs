namespace DentalCore.Domain.Exceptions;

public class NoDataToExportException : Exception
{
    public NoDataToExportException()
    {
    }

    public NoDataToExportException(string? message) : base(message)
    {
    }

    public NoDataToExportException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
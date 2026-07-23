namespace Core.Exceptions;

public class EmailNotFoundException : Exception
{
    public EmailNotFoundException() : base("Email not found") { }

    public EmailNotFoundException(string message) : base(message) { }

    public EmailNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}


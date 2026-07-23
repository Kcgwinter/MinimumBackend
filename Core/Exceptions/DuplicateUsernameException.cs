namespace Core.Exceptions;

public class DuplicateUsernameException : Exception
{
    public DuplicateUsernameException() : base("Username already exists") { }

    public DuplicateUsernameException(string message) : base(message) { }

    public DuplicateUsernameException(string message, Exception innerException) : base(message, innerException) { }
}


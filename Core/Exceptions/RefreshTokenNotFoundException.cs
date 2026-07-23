namespace Core.Exceptions;

public class RefreshTokenNotFoundException : Exception
{
    public RefreshTokenNotFoundException() : base("Refresh token not found") { }

    public RefreshTokenNotFoundException(string message) : base(message) { }

    public RefreshTokenNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}


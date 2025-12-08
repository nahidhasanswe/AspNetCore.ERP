namespace ERP.Core.Exceptions;

public class DomainException : Exception
{
    public DomainException() : base()
    {
        
    }
    
    public DomainException(string? message)
        : base(message)
    {
        
    }

    // Creates a new Exception.  All derived classes should
    // provide this constructor.
    // Note: the stack trace is not started until the exception
    // is thrown
    //
    public DomainException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        
    }
}
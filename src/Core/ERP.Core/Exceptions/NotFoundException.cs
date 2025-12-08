namespace ERP.Core.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() : base()
    {
        
    }
    
    public NotFoundException(string? message)
        : base(message)
    {
        
    }

    // Creates a new Exception.  All derived classes should
    // provide this constructor.
    // Note: the stack trace is not started until the exception
    // is thrown
    //
    public NotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        
    }
}
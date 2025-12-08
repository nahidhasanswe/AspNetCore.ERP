using System.Net;

namespace ERP.Core.Web.Response;

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse(object data, string? message = null)
        : base (data, message)
    {
    }

    public ApiResponse(object data, int count, string? message = null)
        : base(data, count, message)
    {
       
    }

    public ApiResponse(string? message, HttpStatusCode code, IDictionary<string, string[]>? errors = null)
        : base (message, code, errors)
    {
        
    }
}


public class ApiResponse<TResult>
    where TResult: class
{
    public Error? Error { get; set; }
    public Pagination? Pagination { get; set; }
    public TResult? Data { get; set; }
    public string? Message { get; set; }
    public bool Status { get; set; }


    public ApiResponse(TResult data, string? message = null)
    {
        Message = message;
        Status = true;
        Data = data;
    }

    public ApiResponse(TResult data, int count, string? message = null)
    {
        Message = message;
        Status = true;
        Data = data;
        Pagination = new Pagination
        {
            Count = count
        };
    }

    /// <summary>
    /// Initialize with Validation Errors
    /// </summary>
    /// <param name="message">Message for validation error</param>
    /// <param name="code">HttpStatus code.</param>
    /// <param name="errors">Validation Errors details.</param>
    public ApiResponse(string? message, HttpStatusCode code, IDictionary<string, string[]>? errors = null)
    {
        Message = message;
        Status = false;

        Error = new Error
        {
            ErrorMsg = Message,
            Code = ((int)code).ToString(),
            ErrorDetails = errors ?? new Dictionary<string, string[]>()
        };
    }
}

public class Error
{
    public string? Code { get; set; }
    public IDictionary<string, string[]> ErrorDetails { get; set; } = new Dictionary<string, string[]>();
    public string? ErrorMsg { get; set; }
}

public class Pagination
{
    public int Count { get; set; }
}
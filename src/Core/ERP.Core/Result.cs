using ERP.Core.Collections;

namespace ERP.Core;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }
    
    protected Result(bool isSuccess, List<string> errors)
    {
        if (isSuccess && !errors.Any())
            throw new InvalidOperationException("Success result cannot have an error.");
        if (!isSuccess && !errors.Any())
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = string.Join(". ", errors);
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(List<string> errors) => new(false, errors);
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
    
    public new static PaginationResult<T> SuccessForPagination<T>(IPagedList<T> value) where T : class => new(value, true, string.Empty);
    public new static PaginationResult<T> FailureForPagination<T>(string error) where T : class => new(default!, false, error);
}

public class Result<T> : Result
{
    public T Value { get; }

    protected internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }
}

public class PaginationResult<T> : Result<IPagedList<T>>
    where T : class
{
    protected internal PaginationResult(IPagedList<T> data, bool isSuccess, string error)
        : base(data, isSuccess, error)
    {
        
    }
}
using MediatR;

namespace ERP.Core.Behaviors;

public interface IResultQuery<TResponse> : IQuery<Result<TResponse>>
{
    
}

public interface IResultPaginationQuery<TResponse> : IPaginationQuery<Result<TResponse>>
{
    
}



public interface IQuery<out TResponse> : IRequest<TResponse> { }

public interface IPaginationQuery<out TResponse> : IQuery<TResponse>
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
}
using MediatR;

namespace ERP.Core.Behaviors;

public interface IRequestCommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    
}
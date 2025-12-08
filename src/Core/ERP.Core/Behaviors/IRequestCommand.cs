using MediatR;

namespace ERP.Core.Behaviors;

public interface IRequestCommand<TRequest> : IRequest<Result<TRequest>>
{
    
}
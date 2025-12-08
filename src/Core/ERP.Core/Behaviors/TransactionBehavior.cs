using System.Reflection;
using ERP.Core.Uow;
using MediatR;

namespace ERP.Core.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IUnitOfWorkManager unitOfWork) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IQuery<TResponse>)
            return await next(cancellationToken);

        var requestType = request.GetType();

        var enableTransactionAttr = requestType.GetCustomAttribute<EnableTransactionAttribute>();

        // Check that command is enabled for transaction
        if (enableTransactionAttr is null || !enableTransactionAttr.Enabled)
        {
            var response = await next(cancellationToken);
            return response;
        }

        using var scope = unitOfWork.Begin();

        await scope.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);
            await scope.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await scope.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
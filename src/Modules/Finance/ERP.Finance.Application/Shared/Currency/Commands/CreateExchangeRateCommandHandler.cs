using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Shared.Currency;

namespace ERP.Finance.Application.Shared.Currency.Commands;

public class CreateExchangeRateCommandHandler(
    IExchangeRateRepository repository,
    IUnitOfWorkManager unitOfWork
    )
    : IRequestCommandHandler<CreateExchangeRateCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateExchangeRateCommand command, CancellationToken cancellationToken)
    {
        // Add business rule: Check for duplicate rate on the same date/pair
        
        
        
        var rate = new ExchangeRate(
            command.FromCurrency,
            command.ToCurrency,
            command.EffectiveDate,
            command.Rate
        );

        using var scope = unitOfWork.Begin();
        
        await repository.AddAsync(rate, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(rate.Id);
    }
}
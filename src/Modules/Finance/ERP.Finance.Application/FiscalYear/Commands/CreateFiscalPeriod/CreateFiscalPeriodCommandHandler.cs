using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateFiscalPeriod;

public class CreateFiscalPeriodCommandHandler(
    IFiscalPeriodRepository repository,
    IUnitOfWorkManager unitOfWork
    )
    : IRequestCommandHandler<CreateFiscalPeriodCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        // Add business rule: Check for overlapping periods before creating
        
        var name = $"{command.Year}-{command.Month:D2}";
        var period = new FiscalPeriod(name, command.StartDate.Date, command.EndDate.Date); // Assuming a constructor exists

        using var scope = unitOfWork.Begin();
        
        await repository.AddAsync(period, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(period.Id);
    }
}
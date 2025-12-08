using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;

namespace ERP.Finance.Application.FiscalYear.Commands.CloseFiscalPeriod;

public class CloseFiscalPeriodCommandHandler(
    IFiscalPeriodRepository repository,
    IUnitOfWorkManager unitOfWork
    )
    : IRequestCommandHandler<CloseFiscalPeriodCommand, bool>
{
    public async Task<Result<bool>> Handle(CloseFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await repository.GetByIdAsync(command.PeriodId, cancellationToken);
        
        if (period == null) throw new ApplicationException("Fiscal Period not found.");

        // Domain Logic: The Close method enforces the invariants (e.g., can't close hard-closed)
        period.Close(command.TargetStatus);

        using var scope = unitOfWork.Begin();

        await repository.UpdateAsync(period, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}
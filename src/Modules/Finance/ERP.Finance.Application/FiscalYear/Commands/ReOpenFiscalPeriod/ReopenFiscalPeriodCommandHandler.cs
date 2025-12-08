using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;

namespace ERP.Finance.Application.FiscalYear.Commands.ReOpenFiscalPeriod;

public class ReopenFiscalPeriodCommandHandler(
    IFiscalPeriodRepository repository,
    IUnitOfWorkManager unitOfWork
    )
    : IRequestCommandHandler<ReopenFiscalPeriodCommand, bool>
{
    public async Task<Result<bool>> Handle(ReopenFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        var period = await repository.GetByIdAsync(command.PeriodId, cancellationToken);
        
        if (period == null) throw new ApplicationException("Fiscal Period not found.");

        // Domain Logic: The Reopen method enforces the invariants (e.g., can't reopen hard-closed)
        period.Reopen(); 
        
        using var scope = unitOfWork.Begin();

        await repository.UpdateAsync(period, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}
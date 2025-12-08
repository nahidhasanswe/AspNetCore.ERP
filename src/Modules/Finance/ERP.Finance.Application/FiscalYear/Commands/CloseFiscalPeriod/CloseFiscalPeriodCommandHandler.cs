using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.FiscalYear.Enums;

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
        
        if (period == null) 
            return Result.Failure<bool>("Fiscal Period not found.");

        // Domain Logic: Call the appropriate method based on the desired status.
        // The domain aggregate enforces the invariants (e.g., can only soft-close an open period).
        switch (command.TargetStatus)
        {
            case PeriodStatus.SoftClose:
                period.SoftClose();
                break;
            case PeriodStatus.HardClose:
                period.HardClose();
                break;
            default:
                return Result.Failure<bool>($"Closing to status '{command.TargetStatus}' is not a valid closing operation.");
        }

        using var scope = unitOfWork.Begin();

        await repository.UpdateAsync(period, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(true);
    }
}
using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.ReopenFiscalPeriod;

public class ReopenFiscalPeriodCommandHandler(
    IFiscalPeriodRepository fiscalPeriodRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<ReopenFiscalPeriodCommand, Result>
{
    public async Task<Result> Handle(ReopenFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fiscalPeriod = await fiscalPeriodRepository.GetByIdAsync(command.FiscalPeriodId, cancellationToken);
        if (fiscalPeriod == null)
        {
            return Result.Failure("Fiscal Period not found.");
        }

        if (fiscalPeriod.BusinessUnitId != command.BusinessUnitId)
        {
            return Result.Failure("Fiscal Period does not belong to the specified Business Unit.");
        }

        fiscalPeriod.Reopen();

        await fiscalPeriodRepository.UpdateAsync(fiscalPeriod, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
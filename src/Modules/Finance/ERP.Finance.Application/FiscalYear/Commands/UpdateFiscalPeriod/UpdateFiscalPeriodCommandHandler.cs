using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;

namespace ERP.Finance.Application.FiscalYear.Commands.UpdateFiscalPeriod;

public class UpdateFiscalPeriodCommandHandler(
    IFiscalPeriodRepository fiscalPeriodRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<UpdateFiscalPeriodCommand, Result>
{
    public async Task<Result> Handle(UpdateFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fiscalPeriod = await fiscalPeriodRepository.GetByIdAsync(command.FiscalPeriodId, cancellationToken);
        if (fiscalPeriod == null)
        {
            return Result.Failure("Fiscal Period not found.");
        }

        fiscalPeriod.Update(command.NewName, command.NewStartDate, command.NewEndDate);

        await fiscalPeriodRepository.UpdateAsync(fiscalPeriod, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
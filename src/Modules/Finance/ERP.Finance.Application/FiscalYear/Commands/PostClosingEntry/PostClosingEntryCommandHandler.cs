using ERP.Core;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;
namespace ERP.Finance.Application.FiscalYear.Commands.PostClosingEntry;

public class PostClosingEntryCommandHandler(
    IFiscalPeriodRepository fiscalPeriodRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestHandler<PostClosingEntryCommand, Result>
{
    public async Task<Result> Handle(PostClosingEntryCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fiscalPeriod = await fiscalPeriodRepository.GetByIdAsync(command.FiscalPeriodId, cancellationToken);
        if (fiscalPeriod == null)
        {
            return Result.Failure("Fiscal Period not found.");
        }

        fiscalPeriod.PostClosingEntry();

        await fiscalPeriodRepository.UpdateAsync(fiscalPeriod, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
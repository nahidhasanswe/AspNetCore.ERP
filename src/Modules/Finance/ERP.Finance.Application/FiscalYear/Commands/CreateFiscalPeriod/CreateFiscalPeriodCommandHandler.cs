using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateFiscalPeriod;

public class CreateFiscalPeriodCommandHandler(
    IFiscalPeriodRepository fiscalPeriodRepository,
    IUnitOfWorkManager unitOfWork)
    : IRequestCommandHandler<CreateFiscalPeriodCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        using var scope = unitOfWork.Begin();

        var fiscalPeriod = new FiscalPeriod(
            command.BusinessUnitId, // Pass BusinessUnitId
            command.Name,
            command.StartDate,
            command.EndDate
        );

        await fiscalPeriodRepository.AddAsync(fiscalPeriod, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(fiscalPeriod.Id);
    }
}
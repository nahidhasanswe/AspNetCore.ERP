using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.FiscalYear.Commands.CreateFiscalPeriod;

public class CreateFiscalPeriodCommandHandler : IRequestCommandHandler<CreateFiscalPeriodCommand, Guid>
{
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateFiscalPeriodCommandHandler(IFiscalPeriodRepository fiscalPeriodRepository, IUnitOfWorkManager unitOfWork)
    {
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateFiscalPeriodCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var fiscalPeriod = new FiscalPeriod(command.Name, command.StartDate, command.EndDate);

        await _fiscalPeriodRepository.AddAsync(fiscalPeriod, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(fiscalPeriod.Id);
    }
}
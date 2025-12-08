using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using ERP.Finance.Domain.FiscalYear.Aggregates;
using ERP.Finance.Domain.Shared.ValueObjects;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudget;

public class CreateBudgetCommandHandler(
    IBudgetRepository repository,
    IFiscalPeriodRepository fiscalPeriodRepository,
    IUnitOfWorkManager unitOfWork
) : IRequestCommandHandler<CreateBudgetCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateBudgetCommand command, CancellationToken cancellationToken)
    {
        var lineItems = command.LineItems.Select(dto => 
            new BudgetItem(dto.GlAccountId, new Money(dto.Amount, dto.Currency), command.FiscalPeriod, dto.CostCenterId)).ToList();

        var period = await fiscalPeriodRepository.GetPeriodByNameAsync(command.FiscalPeriod, cancellationToken);

        if (period is null)
            return Result.Failure<Guid>("Fiscal period not found.");

        var budget = new Budget(command.BusinessUnitId, command.Name, command.FiscalPeriod, period.StartDate, period.EndDate);
        
        command.LineItems.ToList().ForEach(item => budget.AddItem(new BudgetItem(item.GlAccountId, new Money(item.Amount, item.Currency), command.FiscalPeriod, item.CostCenterId)));

        using var scope = unitOfWork.Begin();
        await repository.AddAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);
        
        return Result.Success(budget.Id);
    }
}
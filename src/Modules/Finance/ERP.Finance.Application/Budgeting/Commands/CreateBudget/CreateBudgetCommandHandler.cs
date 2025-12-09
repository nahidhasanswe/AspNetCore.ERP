using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Core.Uow;
using ERP.Finance.Domain.Budgeting.Aggregates;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.Budgeting.Commands.CreateBudget;

public class CreateBudgetCommandHandler : IRequestCommandHandler<CreateBudgetCommand, Guid>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IUnitOfWorkManager _unitOfWork;

    public CreateBudgetCommandHandler(IBudgetRepository budgetRepository, IUnitOfWorkManager unitOfWork)
    {
        _budgetRepository = budgetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBudgetCommand command, CancellationToken cancellationToken)
    {
        using var scope = _unitOfWork.Begin();

        var budget = new Budget(
            command.BusinessUnitId,
            command.Name,
            command.FiscalPeriod,
            command.StartDate,
            command.EndDate,
            command.ParentBudgetId // Pass ParentBudgetId
        );

        foreach (var itemDto in command.Items)
        {
            var budgetItem = new BudgetItem(
                itemDto.AccountId,
                itemDto.BudgetedAmount,
                itemDto.Period,
                itemDto.CostCenterId
            );
            budget.AddItem(budgetItem);
        }

        await _budgetRepository.AddAsync(budget, cancellationToken);
        await scope.SaveChangesAsync(cancellationToken);

        return Result.Success(budget.Id);
    }
}
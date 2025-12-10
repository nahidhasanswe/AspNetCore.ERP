using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;

namespace ERP.Finance.Application.Encumbrance.Commands.AdjustEncumbrance;

public class AdjustEncumbranceCommand : IRequest<Result>
{
    public Guid BusinessUnitId { get; set; } // New property
    public Guid EncumbranceId { get; set; }
    public Money NewAmount { get; set; }
}
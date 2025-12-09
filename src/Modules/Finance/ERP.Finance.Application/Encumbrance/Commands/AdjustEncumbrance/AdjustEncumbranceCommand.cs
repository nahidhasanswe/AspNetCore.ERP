using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.Encumbrance.Commands.AdjustEncumbrance;

public class AdjustEncumbranceCommand : IRequest<Result>
{
    public Guid EncumbranceId { get; set; }
    public Money NewAmount { get; set; }
}
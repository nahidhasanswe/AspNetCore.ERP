using ERP.Core;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.Encumbrance.Commands.ConvertToCommitment;

public class ConvertToCommitmentCommand : IRequest<Result>
{
    public Guid EncumbranceId { get; set; }
    public Money NewAmount { get; set; }
}
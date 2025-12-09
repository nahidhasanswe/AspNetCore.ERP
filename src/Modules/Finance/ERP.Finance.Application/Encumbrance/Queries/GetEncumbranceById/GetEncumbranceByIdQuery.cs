using ERP.Core;
using ERP.Finance.Application.Encumbrance.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.Encumbrance.Queries.GetEncumbranceById;

public class GetEncumbranceByIdQuery : IRequest<Result<EncumbranceDetailsDto>>
{
    public Guid EncumbranceId { get; set; }
}
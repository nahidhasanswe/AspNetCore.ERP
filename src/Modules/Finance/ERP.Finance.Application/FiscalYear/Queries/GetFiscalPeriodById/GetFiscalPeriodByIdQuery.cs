using ERP.Core;
using ERP.Finance.Application.FiscalYear.DTOs;
using MediatR;
using System;

namespace ERP.Finance.Application.FiscalYear.Queries.GetFiscalPeriodById;

public class GetFiscalPeriodByIdQuery : IRequest<Result<FiscalPeriodDto>>
{
    public Guid FiscalPeriodId { get; set; }
}
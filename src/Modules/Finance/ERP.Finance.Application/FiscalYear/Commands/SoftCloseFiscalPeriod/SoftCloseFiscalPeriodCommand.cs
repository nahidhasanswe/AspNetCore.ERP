using ERP.Core;
using MediatR;
using System;

namespace ERP.Finance.Application.FiscalYear.Commands.SoftCloseFiscalPeriod;

public class SoftCloseFiscalPeriodCommand : IRequest<Result>
{
    public Guid FiscalPeriodId { get; set; }
}
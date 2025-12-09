using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxPayableReport;

public class GetTaxPayableReportQuery : IRequest<Result<IEnumerable<TaxPayableReportDto>>>
{
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}
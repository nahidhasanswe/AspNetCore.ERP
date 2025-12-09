using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.TaxManagement.Queries.GetTaxCollectedReport;

public class GetTaxCollectedReportQuery : IRequest<Result<IEnumerable<TaxCollectedReportDto>>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
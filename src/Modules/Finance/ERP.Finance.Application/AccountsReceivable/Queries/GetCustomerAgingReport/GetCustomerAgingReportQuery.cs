using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerAgingReport;

public class GetCustomerAgingReportQuery : IRequest<Result<IEnumerable<CustomerAgingDto>>>
{
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}
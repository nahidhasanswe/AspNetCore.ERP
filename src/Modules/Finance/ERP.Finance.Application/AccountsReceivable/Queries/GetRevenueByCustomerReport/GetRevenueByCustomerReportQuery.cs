using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetRevenueByCustomerReport;

public class GetRevenueByCustomerReportQuery : IRequest<Result<IEnumerable<RevenueByCustomerDto>>>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
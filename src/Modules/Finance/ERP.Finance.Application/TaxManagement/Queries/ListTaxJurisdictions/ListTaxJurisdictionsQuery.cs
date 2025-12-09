using ERP.Core;
using ERP.Finance.Application.TaxManagement.DTOs;
using MediatR;
using System.Collections.Generic;

namespace ERP.Finance.Application.TaxManagement.Queries.ListTaxJurisdictions;

public class ListTaxJurisdictionsQuery : IRequest<Result<IEnumerable<TaxJurisdictionSummaryDto>>>
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}
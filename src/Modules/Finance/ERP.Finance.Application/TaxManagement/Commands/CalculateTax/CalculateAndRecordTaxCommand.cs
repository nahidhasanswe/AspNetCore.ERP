using ERP.Core;
using ERP.Core.Behaviors;
using ERP.Finance.Domain.Shared.ValueObjects;
using MediatR;
using System;

namespace ERP.Finance.Application.TaxManagement.Commands.CalculateTax;

public class CalculateAndRecordTaxCommand : IRequestCommand<bool>
{
    public Money BaseAmount { get; set; } // Money is expected to be mapped here
    public string JurisdictionCode { get; set; } 
    public DateTime TransactionDate { get; set; }
    public Guid SourceTransactionId { get; set; } 
    public bool IsSalesTransaction { get; set; }
    public Guid? CostCenterId { get; set; }
    
    public Guid SourceControlAccountId { get; set; }
    public string Reference { get; set; } // Added this property
}
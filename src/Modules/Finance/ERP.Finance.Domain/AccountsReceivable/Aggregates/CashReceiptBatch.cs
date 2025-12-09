using ERP.Core.Aggregates;
using ERP.Core.Exceptions;
using ERP.Finance.Domain.AccountsReceivable.Enums;
using ERP.Finance.Domain.AccountsReceivable.Events;
using ERP.Finance.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.Finance.Domain.AccountsReceivable.Aggregates;

public enum CashReceiptBatchStatus
{
    Open,
    Posted,
    Cancelled
}

public class CashReceiptBatch : AggregateRoot
{
    public DateTime BatchDate { get; private set; }
    public Guid CashAccountId { get; private set; } // The GL account to be debited (Bank Account)
    public Money TotalBatchAmount { get; private set; }
    public CashReceiptBatchStatus Status { get; private set; }
    public string Reference { get; private set; } // e.g., Deposit Slip Number

    private readonly List<Guid> _receiptIds = new(); // IDs of CashReceipts in this batch
    public IReadOnlyCollection<Guid> ReceiptIds => _receiptIds.AsReadOnly();

    private CashReceiptBatch() { }

    public CashReceiptBatch(DateTime batchDate, Guid cashAccountId, string reference) : base(Guid.NewGuid())
    {
        BatchDate = batchDate;
        CashAccountId = cashAccountId;
        Reference = reference;
        TotalBatchAmount = new Money(0m, "USD"); // Will be updated as receipts are added
        Status = CashReceiptBatchStatus.Open;
    }

    public void AddReceipt(CashReceipt receipt)
    {
        if (Status != CashReceiptBatchStatus.Open)
            throw new DomainException("Can only add receipts to an open batch.");
        if (receipt.Status != ReceiptStatus.Unapplied && receipt.Status != ReceiptStatus.PartiallyApplied)
            throw new DomainException("Only unapplied or partially applied receipts can be added to a batch.");
        if (_receiptIds.Contains(receipt.Id)) return; // Idempotency

        // Ensure currency consistency
        if (TotalBatchAmount.Amount == 0)
        {
            TotalBatchAmount = new Money(receipt.TotalReceivedAmount.Amount, receipt.TotalReceivedAmount.Currency);
        }
        else if (TotalBatchAmount.Currency != receipt.TotalReceivedAmount.Currency)
        {
            throw new DomainException("Cannot add receipts of different currencies to the same batch.");
        }
        else
        {
            TotalBatchAmount = new Money(TotalBatchAmount.Amount + receipt.TotalReceivedAmount.Amount, TotalBatchAmount.Currency);
        }

        _receiptIds.Add(receipt.Id);
        // Optionally, change receipt status to 'Batched' or similar
    }

    public void RemoveReceipt(Guid receiptId)
    {
        if (Status != CashReceiptBatchStatus.Open)
            throw new DomainException("Can only remove receipts from an open batch.");
        if (!_receiptIds.Remove(receiptId))
            throw new DomainException("Receipt not found in this batch.");

        // Recalculate TotalBatchAmount (requires fetching the receipt's amount)
        // For simplicity, this would typically be done by a service that fetches the receipt.
        // For now, we'll assume the service handles the recalculation.
    }

    public void Post()
    {
        if (Status != CashReceiptBatchStatus.Open)
            throw new DomainException("Only open batches can be posted.");
        if (!_receiptIds.Any())
            throw new DomainException("Cannot post an empty batch.");

        Status = CashReceiptBatchStatus.Posted;
        // Raise event for GL posting
        AddDomainEvent(new CashReceiptBatchPostedEvent(
            this.Id,
            this.BatchDate,
            this.CashAccountId,
            this.TotalBatchAmount,
            this.Reference,
            this.ReceiptIds
        ));
    }

    public void Cancel()
    {
        if (Status != CashReceiptBatchStatus.Open)
            throw new DomainException("Only open batches can be cancelled.");
        
        Status = CashReceiptBatchStatus.Cancelled;
        // Raise event for GL reversal if needed (e.g., if receipts were marked as 'Batched' and need to revert)
    }
}
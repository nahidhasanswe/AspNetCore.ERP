using ERP.Core;
using ERP.Finance.Application.AccountsReceivable.DTOs;
using ERP.Finance.Domain.AccountsReceivable.Aggregates;
using ERP.Finance.Domain.AccountsReceivable.Events; // For PaymentReceivedEvent, etc.
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERP.Finance.Application.AccountsReceivable.Queries.GetCustomerStatement;

public class GetCustomerStatementQueryHandler(
    ICustomerRepository customerRepository,
    ICustomerInvoiceRepository invoiceRepository,
    ICashReceiptRepository cashReceiptRepository,
    ICustomerCreditMemoRepository creditMemoRepository)
    : IRequestHandler<GetCustomerStatementQuery, Result<CustomerStatementDto>>
{
    // Potentially other repositories for adjustments, deductions, etc.

    public async Task<Result<CustomerStatementDto>> Handle(GetCustomerStatementQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure<CustomerStatementDto>("Customer not found.");
        }

        // Fetch all relevant transactions for the customer
        var allInvoices = (await invoiceRepository.ListAllAsync(cancellationToken))
            .Where(inv => inv.CustomerId == request.CustomerId && inv.IssueDate <= request.EndDate)
            .ToList();

        var allCashReceipts = (await cashReceiptRepository.ListAllAsync(cancellationToken))
            .Where(cr => cr.CustomerId == request.CustomerId && cr.ReceiptDate <= request.EndDate)
            .ToList();

        var allCreditMemos = (await creditMemoRepository.ListAllAsync(cancellationToken))
            .Where(cm => cm.CustomerId == request.CustomerId && cm.MemoDate <= request.EndDate)
            .ToList();

        // Determine the primary currency (assuming all transactions are in one currency for simplicity)
        var currency = customer.DefaultCurrency; // Or derive from invoices/receipts

        // Calculate beginning balance (outstanding balance of invoices before StartDate)
        decimal beginningBalance = allInvoices
            .Where(inv => inv.IssueDate < request.StartDate)
            .Sum(inv => inv.OutstandingBalance.Amount); // This is a simplification; a true beginning balance would consider all transactions up to StartDate

        var transactions = new List<CustomerStatementTransactionDto>();

        // Add invoices
        foreach (var inv in allInvoices.Where(inv => inv.IssueDate >= request.StartDate && inv.IssueDate <= request.EndDate))
        {
            transactions.Add(new CustomerStatementTransactionDto(
                inv.IssueDate,
                "Invoice",
                inv.InvoiceNumber,
                $"Invoice {inv.InvoiceNumber}",
                inv.TotalAmount.Amount,
                0m,
                0m // Running balance calculated later
            ));
        }

        // Add payments (this is complex as payments are recorded on invoices)
        // For simplicity, we'll represent cash receipts as a single transaction for now.
        foreach (var cr in allCashReceipts.Where(cr => cr.ReceiptDate >= request.StartDate && cr.ReceiptDate <= request.EndDate))
        {
            transactions.Add(new CustomerStatementTransactionDto(
                cr.ReceiptDate,
                "Payment",
                cr.TransactionReference,
                $"Payment {cr.TransactionReference}",
                0m,
                cr.TotalReceivedAmount.Amount,
                0m
            ));
        }

        // Add credit memos
        foreach (var cm in allCreditMemos.Where(cm => cm.MemoDate >= request.StartDate && cm.MemoDate <= request.EndDate))
        {
            transactions.Add(new CustomerStatementTransactionDto(
                cm.MemoDate,
                "Credit Memo",
                cm.Id.ToString(), // Or a memo number
                $"Credit Memo {cm.Reason}",
                0m,
                cm.OriginalAmount.Amount,
                0m
            ));
        }

        // Sort transactions by date
        transactions = transactions.OrderBy(t => t.TransactionDate).ToList();

        decimal runningBalance = beginningBalance;
        var finalTransactions = new List<CustomerStatementTransactionDto>();

        foreach (var t in transactions)
        {
            runningBalance += t.Debit - t.Credit;
            finalTransactions.Add(t with { RunningBalance = runningBalance });
        }

        decimal endingBalance = runningBalance;

        var statement = new CustomerStatementDto(
            customer.Id,
            customer.Name,
            DateTime.UtcNow, // Statement generation date
            request.StartDate,
            request.EndDate,
            beginningBalance,
            endingBalance,
            currency,
            finalTransactions
        );

        return Result.Success(statement);
    }
}
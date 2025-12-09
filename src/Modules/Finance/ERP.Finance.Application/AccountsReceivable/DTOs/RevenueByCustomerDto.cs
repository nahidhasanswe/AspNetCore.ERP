using System;

namespace ERP.Finance.Application.AccountsReceivable.DTOs;

public record RevenueByCustomerDto(
    Guid CustomerId,
    string CustomerName,
    decimal TotalRevenueAmount,
    string Currency
);
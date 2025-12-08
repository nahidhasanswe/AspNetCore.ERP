namespace ERP.Finance.Domain.GeneralLedger.DTOs;

public record AccountBalanceDto(Guid AccountId, decimal Balance);
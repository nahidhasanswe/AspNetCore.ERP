namespace ERP.Finance.Domain.Shared.ValueObjects;

/// <summary>
/// Represents a payment method as a Value Object, encapsulating its code, 
/// description, and crucial financial metadata (ClearingAccountId).
/// </summary>
public record PaymentMethod
{
    // Properties that define the identity of the Value Object
    public string Code { get; init; } 
    public string Description { get; init; } 
    public bool IsElectronic { get; init; } 
    
    // CRITICAL METADATA: The GL account used temporarily to hold the payment 
    // before it clears the actual bank account.
    public Guid ClearingAccountId { get; init; } 

    // Private constructor to ensure instances are only created internally 
    // (using the static factory methods below).
    private PaymentMethod(string code, string description, bool isElectronic, Guid clearingAccountId)
    {
        Code = code;
        Description = description;
        IsElectronic = isElectronic;
        ClearingAccountId = clearingAccountId;
    }

    // --- Static Factory Instances (The only valid instances) ---
    
    // Note: Use actual configured GUIDs for ClearingAccountId in a real system.
    public static PaymentMethod ACH = new("ACH", "Automated Clearing House", true, Guid.Parse("CASH-US-ACH-001"));
    public static PaymentMethod WireTransfer = new("WIRE", "Bank Wire Transfer", true, Guid.Parse("CASH-INTL-WIRE-002"));
    public static PaymentMethod Cheque = new("CHQ", "Physical Check/Cheque", false, Guid.Parse("CASH-US-CHQ-003"));
    public static PaymentMethod Cash = new("CASH", "Cash", false, Guid.Parse("CASH-PH-CHS-004"));

    // --- Factory Method for Lookups ---
    
    /// <summary>
    /// Retrieves a PaymentMethod instance based on its code string.
    /// </summary>
    public static PaymentMethod FromCode(string code)
    {
        return code?.ToUpperInvariant() switch
        {
            "ACH" => ACH,
            "WIRE" => WireTransfer,
            "CHQ" => Cheque,
            "CASH" => Cash,
            _ => throw new ArgumentException($"Invalid or unsupported payment method code: {code}")
        };
    }
}
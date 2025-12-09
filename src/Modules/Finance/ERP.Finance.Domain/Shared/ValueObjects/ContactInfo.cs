using ERP.Core.ValueObjects;

namespace ERP.Finance.Domain.Shared.ValueObjects;

public class ContactInfo : ValueObject
{
    public string Phone { get; private set; }
    public string Email { get; private set; }

    private ContactInfo() { } // EF Core

    public ContactInfo(string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone cannot be empty", nameof(phone));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        Phone = phone;
        Email = email;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Phone;
        yield return Email;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
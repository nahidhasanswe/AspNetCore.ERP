using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Domain.ValueObjects;

namespace ERP.Domain.Aggregates.PatientAggregate;

public class Patient : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Gender { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public Address? Address { get; private set; }
    public string? EmergencyContact { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    private Patient() { } // EF Core

    public Patient(
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        string gender,
        ContactInfo contactInfo,
        Address? address = null,
        string? emergencyContact = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        if (dateOfBirth > DateTime.UtcNow.AddYears(-18))
            throw new ArgumentException("Patient must be at least 18 years old", nameof(dateOfBirth));

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth.Date;
        Gender = gender;
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        Address = address;
        EmergencyContact = emergencyContact;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            var today = DateTime.UtcNow;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public Result Update(
        string firstName, 
        string lastName, 
        DateTime dateOfBirth, 
        string gender, 
        string? emergencyContact, 
        ContactInfo? contactInfo = null, 
        Address? address = null)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        EmergencyContact = emergencyContact;
        UpdatedAt = DateTime.UtcNow;
        
        if (contactInfo is not null)
            ContactInfo = contactInfo;
        
        if (address is not null)
            Address = address;
        
        return Result.Success();
    }

    public Result UpdateContactInfo(ContactInfo contactInfo)
    {
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
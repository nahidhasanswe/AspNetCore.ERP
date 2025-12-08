using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Events;
using ERP.Domain.ValueObjects;

namespace ERP.Domain.Aggregates.ClinicAggregate;

public class Clinic : AggregateRoot
{
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime UpdatedAt { get; set; }

    private readonly List<Doctor> _doctors = new();
    public IReadOnlyCollection<Doctor> Doctors => _doctors.AsReadOnly();

    private Clinic() { } // EF Core

    public Clinic(string name, Address address, ContactInfo contactInfo)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Clinic name cannot be empty", nameof(name));

        Name = name;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        
        this.AddDomainEvent(new NewClinicRegistrationEvent(Id));
    }

    public Result UpdateInfo(string name, Address address, ContactInfo contactInfo)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Clinic name cannot be empty");

        Name = name;
        Address = address ?? throw new ArgumentNullException(nameof(address));
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        UpdatedAt  = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Clinic is already deactivated");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive)
            return Result.Failure("Clinic is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
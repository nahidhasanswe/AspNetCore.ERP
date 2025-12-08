using ERP.Core;
using ERP.Core.Aggregates;
using ERP.Domain.Aggregates.ClinicAggregate;
using ERP.Domain.ValueObjects;

namespace ERP.Domain.Aggregates.DoctorAggregate;

public class Doctor : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Specialization { get; private set; }
    public string LicenseNumber { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public Guid ClinicId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime UpdatedAt { get; set; }

    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Schedule> Schedules => _schedules.AsReadOnly();

    public Clinic Clinic { get; private set; } = null!; // Navigation property

    private Doctor() { } // EF Core

    public Doctor(
        string firstName,
        string lastName,
        string specialization,
        string licenseNumber,
        ContactInfo contactInfo,
        Guid clinicId)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        LicenseNumber = licenseNumber;
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        ClinicId = clinicId;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";

    public Result Update(
        string firstName,
        string lastName,
        string specialization,
        string licenseNumber,
        ContactInfo contactInfo,
        Guid clinicId)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        LicenseNumber = licenseNumber;
        ContactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
        ClinicId = clinicId;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        
        return Result.Success();
    }

    public Result AddSchedule(Schedule schedule)
    {
        if (_schedules.Any(s => s.IsActive && s.Overlaps(schedule)))
            return Result.Failure("Schedule overlaps with existing schedule");

        _schedules.Add(schedule);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result RemoveSchedule(Guid scheduleId)
    {
        var schedule = _schedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
            return Result.Failure("Schedule not found");

        var result = schedule.Deactivate();
        if (result.IsSuccess)
            UpdatedAt = DateTime.UtcNow;

        return result;
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return Result.Failure("Doctor is already deactivated");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
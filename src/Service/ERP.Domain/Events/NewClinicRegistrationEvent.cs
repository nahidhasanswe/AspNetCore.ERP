using ERP.Core.Events;

namespace ERP.Domain.Events;

public class NewClinicRegistrationEvent(Guid clinicId) : DomainEvent()
{
    public Guid ClinicId { get; } = clinicId;
}
using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Appointment;

public sealed class GetByIdSpecification
    : Specification<Aggregates.AppointmentAggregate.Appointment>
{
    public GetByIdSpecification(Guid id)
        : base(x => x.Id == id)
    {
        AddInclude(x => x.TimeSlot);
        AddInclude(x => x.Patient);
        AddInclude(x => x.Doctor);
        AddInclude(x => x.Clinic);
    }
}

public sealed class GetByTimeSlotIdSpecification
    : Specification<Aggregates.AppointmentAggregate.Appointment>
{
    public GetByTimeSlotIdSpecification(Guid timeSlotId)
        : base(x => x.TimeSlotId == timeSlotId)
    {
        AddInclude(x => x.TimeSlot);
        AddInclude(x => x.Patient);
        AddInclude(x => x.Doctor);
    }
}

public sealed class GetByPatientIdSpecification
    : Specification<Aggregates.AppointmentAggregate.Appointment>
{
    public GetByPatientIdSpecification(Guid patientId)
        : base(x => x.PatientId == patientId)
    {
        AddInclude(x => x.TimeSlot);
        AddInclude(x => x.Clinic);
        AddInclude(x => x.Doctor);
        
        ApplyOrderBy(x => x.BookingDate);
    }
}

public sealed class GetByDoctorIdSpecification
    : Specification<Aggregates.AppointmentAggregate.Appointment>
{
    public GetByDoctorIdSpecification(Guid doctorId, DateTime? date = null)
        : base()
    {
        AddInclude(x => x.TimeSlot);
        AddInclude(x => x.Clinic);
        AddInclude(x => x.Patient);
        
        ApplyOrderBy(x => x.TimeSlot.SlotStartDateTime);

        if (date is not null)
        {
            var startOfDay = date.Value.Date;
            var endOfDay = startOfDay.AddDays(1);
            
            AddCriteria(x => x.Doctor.Id == doctorId && x.TimeSlot.SlotStartDateTime >= startOfDay && x.TimeSlot.SlotStartDateTime < endOfDay);
        }
        else
        {
            AddCriteria(x => x.Doctor.Id == doctorId);
        }
    }
}
    
    
    
using System.Linq.Expressions;
using ERP.Core.Specifications;
using ERP.Domain.Enums;
using LinqKit;

namespace ERP.Domain.Specifications.Appointment;

public sealed class GetAppointmentPaginationListSpecification : PagedListSpecification<Aggregates.AppointmentAggregate.Appointment>
{
    public GetAppointmentPaginationListSpecification(
        Guid? patientId,
        Guid? doctorId,
        Guid? clinicId,
        DateTime? startDate,
        DateTime? endDate,
        AppointmentStatus? status,
        int pageIndex,
        int pageSize,
        string? sort)
        : base(pageIndex, pageSize)
    {
        var query = PredicateBuilder.New<Aggregates.AppointmentAggregate.Appointment>(true);
        
        if (patientId.HasValue && patientId != Guid.Empty)
        {
            query.And(a => a.PatientId == patientId.Value);
        }

        if (doctorId.HasValue && doctorId != Guid.Empty)
        {
            query.And(a => a.DoctorId == doctorId.Value);
        }

        if (clinicId.HasValue && clinicId != Guid.Empty)
        {
            query.And(a => a.ClinicId == clinicId.Value);
        }

        if (startDate.HasValue)
        {
            query.And(a => a.BookingDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query.And(a => a.BookingDate <= endDate.Value);
        }

        if (status is not null)
        {
            query.And(a => a.Status == status);
        }
        
        AddCriteria(query);
        
        if (!string.IsNullOrEmpty(sort))
        {
            // name(default), address, city

            var sortExpr = sort.Split(' ');
            var descending = sortExpr switch
            {
                [_, "desc"] => true,
                _ => false
            };

            var columnName = sortExpr[0];

            Expression<Func<Aggregates.AppointmentAggregate.Appointment, object>> orderBy = columnName switch
            {
                "doctor" => x => x.Doctor.FirstName,
                "clinic" => x => x.Clinic.Name,
                "bookingDate" => x => x.BookingDate,
                _ => x => x.BookingDate
            };

            ApplyOrderBy(orderBy, descending);
        }
        else
        {
            ApplyOrderBy(x => x.BookingDate);
        }
    }
}

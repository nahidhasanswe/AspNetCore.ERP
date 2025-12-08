using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Doctor;

public class GetByIdWithSchedulesSpecification: Specification<Aggregates.DoctorAggregate.Doctor>
{
    public GetByIdWithSchedulesSpecification(Guid id)
        : base(c => c.Id == id)
    {
        AddInclude(x => x.Schedules);
    }
}
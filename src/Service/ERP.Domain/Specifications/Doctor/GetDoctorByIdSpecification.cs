using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Doctor;

public class GetDoctorByIdSpecification : Specification<Aggregates.DoctorAggregate.Doctor>
{
    public GetDoctorByIdSpecification(Guid id)
        : base (x => x.Id.Equals(id))
    {
        AddInclude(x => x.Clinic);
    }
}
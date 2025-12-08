using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Doctor;

public sealed class GetBySpecializationSpecification
    : Specification<Aggregates.DoctorAggregate.Doctor>
{
    public GetBySpecializationSpecification(string specialization)
        : base(x => x.Specialization == specialization && x.IsActive)
    {
        ApplyOrderBy(x => x.FirstName);
    }
}
using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Clinic;

public class GetClinicByIdSpecification(Guid id) : Specification<Aggregates.ClinicAggregate.Clinic>(c => c.Id == id);
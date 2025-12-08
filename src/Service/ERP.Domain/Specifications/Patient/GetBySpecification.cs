using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Patient;

public class GetByIdSpecification(Guid id) : Specification<Aggregates.PatientAggregate.Patient>(c => c.Id == id);

public class GetByEmailSpecification(string email) : Specification<Aggregates.PatientAggregate.Patient>(c => c.ContactInfo.Email == email);

public class GetByPhoneSpecification(string phone) : Specification<Aggregates.PatientAggregate.Patient>(c => c.ContactInfo.Phone == phone);
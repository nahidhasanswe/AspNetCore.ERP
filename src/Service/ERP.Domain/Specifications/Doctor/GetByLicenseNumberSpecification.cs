using ERP.Core.Specifications;

namespace ERP.Domain.Specifications.Doctor;

public class GetByLicenseNumberSpecification(string licenseNumber)
    : Specification<Aggregates.DoctorAggregate.Doctor>(c => c.LicenseNumber == licenseNumber);
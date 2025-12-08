using ERP.Core.Repository;

namespace ERP.Domain.Aggregates.PatientAggregate;

public interface IPatientRepository: IRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken = default);
}
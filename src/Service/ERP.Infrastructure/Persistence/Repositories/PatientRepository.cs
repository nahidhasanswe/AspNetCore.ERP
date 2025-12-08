using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Domain.Aggregates.PatientAggregate;
using ERP.Domain.Specifications.Patient;

namespace ERP.Infrastructure.Persistence.Repositories;

public class PatientRepository(IDbContextProvider<BookingDbContext> dbContextProvider) : EfRepository<BookingDbContext, Patient>(dbContextProvider), IPatientRepository
{
    public Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(new GetByEmailSpecification(email), cancellationToken);
    }

    public Task<Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(new GetByPhoneSpecification(phone), cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return AnyAsync(new GetByEmailSpecification(email), cancellationToken);
    }

    public Task<bool> ExistsByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return AnyAsync(new GetByPhoneSpecification(phone), cancellationToken);
    }
}
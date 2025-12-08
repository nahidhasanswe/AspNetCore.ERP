using ERP.Core.EF;
using ERP.Core.EF.Repository;
using ERP.Domain.Aggregates.DoctorAggregate;
using ERP.Domain.Specifications.Doctor;

namespace ERP.Infrastructure.Persistence.Repositories;

public class DoctorRepository(IDbContextProvider<BookingDbContext> dbContextProvider) : EfRepository<BookingDbContext, Doctor>(dbContextProvider), IDoctorRepository
{
    public Task<Doctor?> GetByIdWithSchedulesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return FirstOrDefaultAsync(new GetByIdWithSchedulesSpecification(id),  cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetByClinicIdAsync(Guid clinicId, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new GetByClinicIdSpecification(clinicId), cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new GetBySpecializationSpecification(specialization), cancellationToken);
    }

    public Task<bool> ExistsByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return AnyAsync(new GetByLicenseNumberSpecification(licenseNumber),  cancellationToken);
    }
}
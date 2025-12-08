using ERP.Core.Repository;

namespace ERP.Finance.Domain.AccountsPayable.Aggregates;

public interface IVendorOnboardingRequestRepository : IRepository<VendorOnboardingRequest>
{
    Task<IReadOnlyCollection<VendorOnboardingRequest>> GetAllAsync(OnboardingStatus? status, CancellationToken cancellationToken = default);
}
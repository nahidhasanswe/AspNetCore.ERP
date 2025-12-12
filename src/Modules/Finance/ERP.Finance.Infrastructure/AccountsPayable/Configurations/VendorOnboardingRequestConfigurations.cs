using ERP.Finance.Domain.AccountsPayable.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.AccountsPayable.Configurations;

public class VendorOnboardingRequestConfigurations : IEntityTypeConfiguration<VendorOnboardingRequest>
{
    public void Configure(EntityTypeBuilder<VendorOnboardingRequest> builder)
    {
        builder.ToTable("VendorOnboardingRequests");
        builder.HasKey(vor => vor.Id);

        builder.Property(vor => vor.ProposedName).IsRequired().HasMaxLength(200);
        builder.Property(vor => vor.ProposedTaxId).HasMaxLength(50);
        builder.Property(vor => vor.ProposedPaymentTerms).HasMaxLength(100);
        builder.Property(vor => vor.ProposedDefaultCurrency).HasMaxLength(3);
        builder.Property(vor => vor.RejectionReason).HasMaxLength(1000);
        builder.Property(vor => vor.ApprovedVendorId);
        builder.Property(vor => vor.CreatedAt).IsRequired();

        builder.OwnsOne(vor => vor.ProposedAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ProposedStreet").HasMaxLength(200);
            address.Property(a => a.City).HasColumnName("ProposedCity").HasMaxLength(100);
            address.Property(a => a.State).HasColumnName("ProposedState").HasMaxLength(100);
            address.Property(a => a.Country).HasColumnName("ProposedCountry").HasMaxLength(100);
            address.Property(a => a.PostalCode).HasColumnName("ProposedPostalCode").HasMaxLength(20);
        });

        builder.OwnsOne(vor => vor.ProposedContactInfo, contact =>
        {
            contact.Property(c => c.Phone).HasColumnName("ProposedPhone").HasMaxLength(50);
            contact.Property(c => c.Email).HasColumnName("ProposedEmail").HasMaxLength(100);
        });

        builder.OwnsOne(vor => vor.ProposedBankDetails, bank =>
        {
            bank.Property(b => b.BankName).HasColumnName("ProposedBankName").HasMaxLength(200);
            bank.Property(b => b.AccountNumber).HasColumnName("ProposedAccountNumber").HasMaxLength(50);
            bank.Property(b => b.RoutingNumber).HasColumnName("ProposedRoutingNumber").HasMaxLength(50);
            bank.Property(b => b.AccountName).HasColumnName("ProposedAccountName").HasMaxLength(50);
        });

        builder.Property(vor => vor.Status).HasConversion<string>().HasMaxLength(50);
        builder.HasIndex(vor => vor.Status);
    }
}
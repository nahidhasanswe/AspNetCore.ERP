using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class RecurringJournalEntryConfigurations : IEntityTypeConfiguration<RecurringJournalEntry>
{
    public void Configure(EntityTypeBuilder<RecurringJournalEntry> builder)
    {
        builder.ToTable("RecurringJournalEntries");
        builder.HasKey(rje => rje.Id);

        builder.Property(rje => rje.BusinessUnitId).IsRequired();
        builder.HasIndex(rje => rje.BusinessUnitId);

        builder.Property(rje => rje.Description).IsRequired().HasMaxLength(500);
        builder.Property(rje => rje.ReferenceNumber).HasMaxLength(100);
        builder.Property(rje => rje.StartDate).IsRequired();
        builder.Property(rje => rje.EndDate);
        builder.Property(rje => rje.Frequency).IsRequired().HasMaxLength(50);
        builder.Property(rje => rje.LastPostedDate);

        builder.HasMany(rje => rje.Lines).WithOne().HasForeignKey("RecurringJournalEntryId").OnDelete(DeleteBehavior.Cascade);
    }
}
using ERP.Finance.Domain.GeneralLedger.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Finance.Infrastructure.GeneralLedger.Configurations;

public class JournalEntryConfigurations : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");
        builder.HasKey(je => je.Id);

        builder.Property(je => je.BusinessUnitId).IsRequired();
        builder.HasIndex(je => je.BusinessUnitId);

        builder.Property(je => je.PostingDate).IsRequired();
        builder.HasIndex(je => je.PostingDate);

        builder.Property(je => je.Description).IsRequired().HasMaxLength(500);
        builder.Property(je => je.ReferenceNumber).HasMaxLength(100);
        builder.Property(je => je.IsPosted).IsRequired();
        builder.HasIndex(je => je.IsPosted);

        builder.HasMany(je => je.Lines).WithOne().HasForeignKey("JournalEntryId").OnDelete(DeleteBehavior.Cascade);
    }
}
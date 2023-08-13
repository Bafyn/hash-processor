using HashProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HashProcessor.Worker.Persistence.Configurations;

internal class HashConfiguration : IEntityTypeConfiguration<Hash>
{
    public void Configure(EntityTypeBuilder<Hash> builder)
    {
        builder.ToTable("Hashes", "hashprocessor");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).UseIdentityColumn();
        builder.Property(e => e.Date).HasColumnType("DATE");
    }
}

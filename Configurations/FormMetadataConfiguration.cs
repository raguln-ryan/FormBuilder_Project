using FormBuilder.API.Models.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FormBuilder.API.Configurations
{
    public class FormMetadataConfiguration : IEntityTypeConfiguration<FormMetadata>
    {
        public void Configure(EntityTypeBuilder<FormMetadata> builder)
        {
            builder.ToTable("form_metadata");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FormName)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(f => f.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(f => f.PublishedBy)
                   .HasMaxLength(100);

            builder.Property(f => f.CreatedDate)
                   .IsRequired();

            builder.Property(f => f.PublishedDate)
                   .IsRequired(false);

            builder.Property(f => f.IsPublished)
                   .HasDefaultValue(false);

            builder.Property(f => f.WorkflowUsage)
                   .HasMaxLength(50);
        }
    }
}

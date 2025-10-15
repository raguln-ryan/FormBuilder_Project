using FormBuilder.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.Configurations
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseDetail> ResponseDetails { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ResponseDetail foreign key
            modelBuilder.Entity<ResponseDetail>()
                .HasOne<Response>()
                .WithMany(r => r.Details)
                .HasForeignKey(d => d.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure FileAttachment foreign key
            modelBuilder.Entity<FileAttachment>()
                .HasOne(f => f.Response)
                .WithMany()
                .HasForeignKey(f => f.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for better performance
            modelBuilder.Entity<FileAttachment>()
                .HasIndex(f => f.ResponseId)
                .HasDatabaseName("IX_FileAttachment_ResponseId");

            modelBuilder.Entity<FileAttachment>()
                .HasIndex(f => new { f.ResponseId, f.QuestionId })
                .HasDatabaseName("IX_FileAttachment_ResponseId_QuestionId");
        }
    }
}

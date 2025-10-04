using FormBuilder.API.Models.Relational;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.Data
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        public DbSet<FormMetadata> FormMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MySqlDbContext).Assembly);
        }
    }
}

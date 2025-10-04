using System.Collections.Generic;
using System.Threading.Tasks;
using FormBuilder.API.DataAccessLayer.Interfaces;
using FormBuilder.API.Models.Relational;
using FormBuilder.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.DataAccessLayer.Implementations
{
    public class FormMetadataDAL : IFormMetadataDAL
    {
        private readonly MySqlDbContext _context;

        public FormMetadataDAL(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<FormMetadata>> GetAllAsync()
        {
            return await _context.FormMetadata.ToListAsync();
        }

        public async Task<FormMetadata> GetByIdAsync(int id)
        {
            return await _context.FormMetadata.FindAsync(id);
        }

        public async Task CreateAsync(FormMetadata entity)
        {
            await _context.FormMetadata.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, FormMetadata entity)
        {
            var existing = await _context.FormMetadata.FindAsync(id);
            if (existing == null) return false;

            existing.FormName = entity.FormName;
            existing.PublishedBy = entity.PublishedBy;
            existing.PublishedDate = entity.PublishedDate;
            existing.WorkflowUsage = entity.WorkflowUsage;
            existing.IsPublished = entity.IsPublished;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.FormMetadata.FindAsync(id);
            if (entity == null) return false;

            _context.FormMetadata.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublishAsync(int id)
        {
            var entity = await _context.FormMetadata.FindAsync(id);
            if (entity == null) return false;

            entity.IsPublished = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

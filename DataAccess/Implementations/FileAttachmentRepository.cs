using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using System.Collections.Generic;
using System.Linq;

namespace FormBuilder.API.DataAccess.Implementations
{
    public class FileAttachmentRepository : IFileAttachmentRepository
    {
        private readonly MySqlDbContext _context;

        public FileAttachmentRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public void Add(FileAttachment fileAttachment)
        {
            _context.FileAttachments.Add(fileAttachment);
        }

        public void AddRange(List<FileAttachment> fileAttachments)
        {
            _context.FileAttachments.AddRange(fileAttachments);
        }

        public FileAttachment? GetById(int id)
        {
            return _context.FileAttachments.FirstOrDefault(f => f.Id == id);
        }

        public FileAttachment? GetByResponseAndQuestion(int responseId, string questionId)
        {
            return _context.FileAttachments
                .FirstOrDefault(f => f.ResponseId == responseId && f.QuestionId == questionId);
        }

        public List<FileAttachment> GetByResponseId(int responseId)
        {
            return _context.FileAttachments
                .Where(f => f.ResponseId == responseId)
                .ToList();
        }

        public void Delete(int id)
        {
            var fileAttachment = _context.FileAttachments.Find(id);
            if (fileAttachment != null)
            {
                _context.FileAttachments.Remove(fileAttachment);
            }
        }

        public void DeleteByResponseId(int responseId)
        {
            var files = _context.FileAttachments.Where(f => f.ResponseId == responseId);
            _context.FileAttachments.RemoveRange(files);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
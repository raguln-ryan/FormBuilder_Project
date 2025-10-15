using FormBuilder.API.Models;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IFileAttachmentRepository
    {
        void Add(FileAttachment fileAttachment);
        void AddRange(List<FileAttachment> fileAttachments);
        FileAttachment? GetById(int id);
        FileAttachment? GetByResponseAndQuestion(int responseId, string questionId);
        List<FileAttachment> GetByResponseId(int responseId);
        void Delete(int id);
        void DeleteByResponseId(int responseId);
        bool SaveChanges();
    }
}
using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using System.Collections.Generic;
using System.Linq;

namespace FormBuilder.API.DataAccess.Implementations
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly MySqlDbContext _db;

        public ResponseRepository(MySqlDbContext db)
        {
            _db = db;
        }

        public void Add(Response response)
        {
            _db.Responses.Add(response);

            if (response.Details != null)
            {
                foreach (var detail in response.Details)
                {
                    detail.ResponseId = response.Id;
                    _db.ResponseDetails.Add(detail);
                }
            }

            _db.SaveChanges();
        }

        public Response? GetById(int id)
        {
            return _db.Responses
                      .Where(r => r.Id == id)
                      .Select(r => new Response
                      {
                          Id = r.Id,
                          FormId = r.FormId,
                          UserId = r.UserId,
                          SubmittedAt = r.SubmittedAt,
                          Details = _db.ResponseDetails
                                       .Where(d => d.ResponseId == r.Id)
                                       .ToList() ?? new List<ResponseDetail>()
                      })
                      .FirstOrDefault();
        }

        public IEnumerable<Response> GetByFormId(int formId)
        {
            return _db.Responses
                      .Where(r => r.FormId == formId)
                      .Select(r => new Response
                      {
                          Id = r.Id,
                          FormId = r.FormId,
                          UserId = r.UserId,
                          SubmittedAt = r.SubmittedAt,
                          Details = _db.ResponseDetails
                                       .Where(d => d.ResponseId == r.Id)
                                       .ToList() ?? new List<ResponseDetail>()
                      })
                      .ToList();
        }

        // ✅ Implement the new GetAll method
        public IEnumerable<Response> GetAll()
        {
            return _db.Responses
                      .Select(r => new Response
                      {
                          Id = r.Id,
                          FormId = r.FormId,
                          UserId = r.UserId,
                          SubmittedAt = r.SubmittedAt,
                          Details = _db.ResponseDetails
                                       .Where(d => d.ResponseId == r.Id)
                                       .ToList() ?? new List<ResponseDetail>()
                      })
                      .ToList();
        }

        public void Update(Response response)
        {
            _db.Responses.Update(response);

            if (response.Details != null)
            {
                foreach (var detail in response.Details)
                {
                    _db.ResponseDetails.Update(detail);
                }
            }

            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var response = GetById(id);
            if (response != null)
            {
                var details = _db.ResponseDetails
                                 .Where(d => d.ResponseId == response.Id)
                                 .ToList() ?? new List<ResponseDetail>();

                _db.ResponseDetails.RemoveRange(details);
                _db.Responses.Remove(response);
                _db.SaveChanges();
            }
        }
    }
}

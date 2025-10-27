using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            // EF Core will automatically handle the Details list
            // and set ResponseId for each ResponseDetail
            _db.Responses.Add(response);

            // Save everything in a single transaction
            _db.SaveChanges();
        }


        public Response? GetById(string id)
        {
            if (!int.TryParse(id, out int intId))
                return null; // invalid ID

            return _db.Responses
                      .Where(r => r.Id == intId) // now both are int
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
        public IEnumerable<Response> GetByFormId(string formId)
        {
            var responses = _db.Responses
                      .Where(r => r.FormId == formId)
                      .ToList();
    
            // Explicitly load User data for each response
            foreach (var response in responses)
            {
                // Load the User from the database
                response.User = _db.Users.FirstOrDefault(u => u.Id == response.UserId);
        
                // Load the Details
                response.Details = _db.ResponseDetails
                             .Where(d => d.ResponseId == response.Id)
                             .ToList();
            }
    
            return responses;
        }

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

        public void Delete(string id)
        {
            if (!int.TryParse(id, out int intId)) return;

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

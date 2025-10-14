using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FormBuilder.API.DataAccess.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly IMongoCollection<Question> _questions;

        public QuestionRepository(MongoDbContext dbContext)
        {
            _questions = dbContext.Questions;
        }

        public void Add(Question question) => _questions.InsertOne(question);

        public Question GetById(string id) => _questions.Find(q => q.QuestionId == id).FirstOrDefault();

        public IEnumerable<Question> GetAll() => _questions.Find(q => true).ToList();

        public void Update(Question question) => _questions.ReplaceOne(q => q.QuestionId == question.QuestionId, question);

        public void Delete(string id) => _questions.DeleteOne(q => q.QuestionId == id);
    }
}

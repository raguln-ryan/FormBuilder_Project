using FormBuilder.API.DataAccess.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.API.Configurations;
using System.Linq;

namespace FormBuilder.API.DataAccess.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlDbContext _db;

        public UserRepository(MySqlDbContext db)
        {
            _db = db;
        }

        public User GetById(int id) => _db.Users.FirstOrDefault(u => u.Id == id);

        public User GetByEmail(string email) => _db.Users.FirstOrDefault(u => u.Email == email);

        public void Add(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public void Update(User user)
        {
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
            }
        }
    }
}

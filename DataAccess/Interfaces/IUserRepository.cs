using FormBuilder.API.Models;

namespace FormBuilder.API.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        void Delete(int id);
    }
}

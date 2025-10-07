using Microsoft.EntityFrameworkCore;
using FormBuilder.API.Configurations;

namespace FormBuilder.API.Services
{
    public class MySqlService
    {
        private readonly MySqlDbContext _context;

        public MySqlService(MySqlDbContext context)
        {
            _context = context;
        }

        public MySqlDbContext GetContext() => _context;
    }
}

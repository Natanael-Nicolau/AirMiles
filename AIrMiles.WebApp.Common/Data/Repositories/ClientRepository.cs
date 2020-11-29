using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Client> GetAllWithUsers()
        {
            return _context.Clients
                .Where(c => !c.IsDeleted)
                .Include(c => c.User)
                .AsNoTracking();
        }

        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients
                .Where(c => !c.IsDeleted)
                .Include(c => c.User)
                .Where(c => c.User.Email == email)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}

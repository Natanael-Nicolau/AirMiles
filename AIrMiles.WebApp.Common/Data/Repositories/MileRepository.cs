using AIrMiles.WebApp.Common.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class MileRepository : GenericRepository<Mile>, IMileRepository
    {
        private readonly DataContext _context;

        public MileRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public int GetClientTotalStatusMiles(int clientID)
        {

            return _context.Miles
                .Where(m => m.MilesTypeId == 1 && m.ClientId == clientID && !m.IsDeleted)
                .Sum(m => m.Qtd);
        }

        public int GetClientTotalBonusMiles(int clientID)
        {

            return _context.Miles
                 .Where(m => m.MilesTypeId == 2 && m.ClientId == clientID && !m.IsDeleted)
                 .Sum(m => m.Qtd);
        }

        public List<Mile> GetAllBonusMiles(int clientID)
        {

            return _context.Miles
                 .Where(m => m.MilesTypeId == 2 && m.ClientId == clientID && !m.IsDeleted)
                 .OrderBy(m => m.ExpirationDate)
                 .ToList();
        }

        public async Task<bool> TransferMilesAsync(List<Mile> clientMiles, int transferMiles)
        {
            bool isSuccess = true;

            for (int i = transferMiles, j = 0; i != 0; j++)
            {
                if (i < 0)
                {
                    isSuccess = false;
                }

                var currentMile = clientMiles[j];

                if (currentMile.Qtd >= i)
                {
                    i = 0;
                    currentMile.Qtd -= i;
                }
                else
                {
                    i -= currentMile.Qtd;
                    currentMile.Qtd = 0;
                }

                await UpdateAsync(currentMile);
            }

            _context.RemoveRange(clientMiles.Where(m => m.Qtd == 0));
            await _context.SaveChangesAsync();

            return isSuccess;
        }
    }
}

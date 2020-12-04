using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
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
                .AsNoTracking()
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

        public async Task<bool> SpendMilesAsync(List<Mile> clientMiles, int milesToSpend)
        {
            bool isSuccess = true;

            for (int i = milesToSpend, j = 0; i != 0; j++)
            {
                if (i < 0)
                {
                    isSuccess = false;
                }

                var currentMile = clientMiles[j];

                if (currentMile.Qtd >= i)
                {
                    currentMile.Qtd -= i;
                    i = 0;
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

        public async Task DeleteExpiredMilesAsync()
        {
            var milesToRemove = _context.Miles
                .Where(m => m.ExpirationDate <= DateTime.Now);

            _context.RemoveRange(milesToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task ResetClientStatusMiles(int clientId)
        {
            var clientStatusMiles = _context.Miles
                .Where(m => m.ClientId == clientId && m.MilesTypeId == 1);


            _context.RemoveRange(clientStatusMiles);
            await _context.SaveChangesAsync();
        }
    }
}

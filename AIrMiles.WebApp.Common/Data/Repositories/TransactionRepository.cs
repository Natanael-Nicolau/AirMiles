using AIrMiles.WebApp.Common.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly DataContext _context;

        public TransactionRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public List<Transaction> GetAllByClientId(int clientID)
        {
            return _context.Transactions.Where(t => t.ClientID == clientID).ToList();
        }
    }
}

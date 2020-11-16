﻿using AIrMiles.WebApp.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public async Task<Client> GetByEmailAsync(string email)
        {
            return await _context.Clients.Include(c => c.User)
                .Where(c => c.User.Email == email)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
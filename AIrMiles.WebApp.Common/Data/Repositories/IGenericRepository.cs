using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIrMiles.WebApp.Common.Data.Repositories
{
    public interface IGenericRepository<T> where T : class
    {

        IQueryable<T> GetAll();

        IQueryable<T> GetAllWithDeleted();

        Task<T> GetByIdAsync(int id);


        Task CreateAsync(T entity);


        Task UpdateAsync(T entity);


        Task DeleteAsync(T entity);


        Task<bool> ExistAsync(int id);
    }
}

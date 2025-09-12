using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETickets.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        // Read ---------------------------------------------------------------------------------
        Task<List<T>> GetAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
        Task<T> GetOneAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? include = null, bool tracked = true);
        //----------------------------------------------------------------------------------------

        // Create --------------------------------------------------------------------------------
        Task AddAsync(T entity);
        //----------------------------------------------------------------------------------------

        // Update --------------------------------------------------------------------------------
        Task Update(T entity);
        //----------------------------------------------------------------------------------------

        // Delete---------------------------------------------------------------------------------
        Task DeleteAsync(T entity);
        //----------------------------------------------------------------------------------------

        // Save ----------------------------------------------------------------------------------
        Task CommitAsync();
        //----------------------------------------------------------------------------------------
    }
}

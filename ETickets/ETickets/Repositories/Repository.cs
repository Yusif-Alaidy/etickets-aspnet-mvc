using ETickets.DataAccess;
using ETickets.Models;
using ETickets.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace ETickets.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private CineBookContext _context;
        private DbSet<T> _db;
        public Repository(CineBookContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        // Read ---------------------------------------------------------------------------------
        public async Task<List<T>> GetAsync(Expression<Func<T,bool>>? filter = null, Expression<Func<T, object>>[]? include = null, bool tracked = true)
        {
            var data = _db.AsQueryable();

            if (filter is not null) 
            {
                data = data.Where(filter);
            }
            if (include is not null) 
            {
                foreach (var item in include)
                {
                    data = data.Include(item);
                }
            }
            if (!tracked)
            {
                data = data.AsNoTracking();
            }

            return await data.ToListAsync();
        }
        public async Task<T> GetOneAsync(Expression<Func<T, bool>>? filter = null, Expression<Func<T, object>>[]? include = null, bool tracked = true)
        {

            return (await GetAsync(filter, include, tracked)).FirstOrDefault()!;
        }
        //----------------------------------------------------------------------------------------

        // Create --------------------------------------------------------------------------------
        public async Task AddAsync(T entity)                                         
        {
            await _db.AddAsync(entity);
        }
        //----------------------------------------------------------------------------------------

        // Update --------------------------------------------------------------------------------
        public async Task Update(T entity)
        {
            _db.Update(entity);
        }
        //----------------------------------------------------------------------------------------

        // Delete---------------------------------------------------------------------------------
        public async Task DeleteAsync(T entity)
        {
             _db.Remove(entity);
        }
        //----------------------------------------------------------------------------------------

        // Save ----------------------------------------------------------------------------------
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
        //----------------------------------------------------------------------------------------
    }
}

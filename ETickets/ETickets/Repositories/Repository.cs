using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETickets.Repositories
{
    public class Repository<T> where T : class
    {
        private CineBookContext _context;
        private DbSet<T> _db;
        public Repository(CineBookContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        // Read ---------------------------------------------------------------------------------
        public async Task<List<T>> GetAsync(Expression<Func<T,bool>>? filter = null)
        {
            var data = _db.AsQueryable();

            if (filter is not null) 
            {
                data = data.Where(filter);
            }

            return await data.ToListAsync();
        }
        public async Task<T> GetOneAsync(Expression<Func<T,bool>> filter)
        {

            return await _db.FirstOrDefaultAsync(filter);
        }
        //----------------------------------------------------------------------------------------

        // Create --------------------------------------------------------------------------------
        public async Task AddAsync(T T)                                         
        {
            await _db.AddAsync(T);
        }
        //----------------------------------------------------------------------------------------

        // Update --------------------------------------------------------------------------------
        public async Task Update(T T)
        {
            _db.Update(T);
        }
        //----------------------------------------------------------------------------------------

        // Delete---------------------------------------------------------------------------------
        public async Task DeleteAsync(T T)
        {
             _db.Remove(T);
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

using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETickets.Repositories
{
    public class Repository
    {
        private CineBookContext _context;
        public Repository(CineBookContext context)
        {
            _context = context;
        }

        // Read ---------------------------------------------------------------------------------
        public async Task<List<Category>> GetAsync(Expression<Func<Category,bool>>? filter = null)
        {
            var car = _context.Categories.AsQueryable();

            if (filter is not null) 
            {
                car = car.Where(filter);
            }

            return await car.ToListAsync();
        }
        public async Task<Category> GetOneAsync(Expression<Func<Category,bool>> filter)
        {

            return await _context.Categories.FirstOrDefaultAsync(filter);
        }
        //----------------------------------------------------------------------------------------

        // Create --------------------------------------------------------------------------------
        public async Task AddAsync(Category category)                                         
        {
            await _context.Categories.AddAsync(category);
        }
        //----------------------------------------------------------------------------------------

        // Update --------------------------------------------------------------------------------
        public async Task Update(Category category)
        {
            _context.Categories.Update(category);
        }
        //----------------------------------------------------------------------------------------

        // Delete---------------------------------------------------------------------------------
        public async Task DeleteAsync(Category category)
        {
             _context.Categories.Remove(category);
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

using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MovieController : Controller
    {
        private readonly CineBookContext _context;

        public MovieController(CineBookContext context)
        {
            _context = context;
        }
        //[HttpPost]
        public IActionResult Create(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return View();
        }
    }
}

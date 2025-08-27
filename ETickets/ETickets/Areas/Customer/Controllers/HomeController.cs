using ETickets.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly CineBookContext _context;

        public HomeController(CineBookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var movies = _context.Movies.Include(e => e.Category);
            return View(movies.ToList());
        }
        public IActionResult Actor()
        {
            return View();
        }
        public IActionResult Movie(int id)
        {
            var movie = _context.Movies.Include(e=>e.Category).Include(e=>e.Cinema).FirstOrDefault(e => e.Id == id);
            if (movie is null) { return NotFound(); }

            return View(movie);
        }
    }
}

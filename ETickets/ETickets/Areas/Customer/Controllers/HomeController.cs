using ETickets.DataAccess;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ETickets.ViewModel;
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

        public IActionResult Index(FilterVM filterVM)
        {
            Home data = new();
            var categories = _context.Categories;
            var cinemas = _context.Cinemas;
            var movies = _context.Movies.Include(e => e.Category).Include(e => e.Cinema).AsQueryable();
            if (movies is null) return NotFound();

            //-Filter ------------------------------------------------------------------------------------

            // search
            if (filterVM.search is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterVM.search));
                ViewBag.Search = filterVM.search;
            }

            // min price
            if (filterVM.minPrice is not null)
            {
                movies = movies.Where(e => e.Price >= filterVM.minPrice);
                ViewBag.MinPrice = filterVM.minPrice;
            }

            // max price
            if (filterVM.maxPrice is not null)
            {
                movies = movies.Where(e => e.Price <= filterVM.maxPrice);
                ViewBag.MaxPrice = filterVM.maxPrice;
            }

            // Category price
            if (filterVM.categoryId is not null) 
            { 
                movies = movies.Where(e => e.Category.Id == filterVM.categoryId); 
                ViewBag.CategoryId = filterVM.categoryId;
            }

            // Cienma price
            if (filterVM.cinemaId is not null)
            {
                movies = movies.Where(e => e.Cinema.Id == filterVM.cinemaId);
                ViewBag.CinemaId = filterVM.cinemaId;
            }
            // --------------------------------------------------------------------------------------------

            data.Movies = movies.ToList();
            data.Categories = categories.ToList();
            data.Cinemas = cinemas.ToList();

            return View(data);
        }
        public IActionResult Actor(int id)
        {
            ActorDetails data = new();

            var actor = _context.Actors.FirstOrDefault(e => e.Id == id);
            if (actor == null) return NotFound();
            data.Actor = actor;

            var Movies = _context.Movies.Where(e => e.Actors.Any(e => e.Id == actor.Id));
            data.Movies = Movies.ToList();


            return View(data);
        }
        public IActionResult Movie(int id)
        {

            MovieDetails data = new();
            var movie = _context.Movies.Include(e=>e.Category).Include(e=>e.Cinema).Include(e => e.Actors).FirstOrDefault(e => e.Id == id);
            if (movie is null) { return NotFound(); }
            data.Movie = movie;

            var similerMovie = _context.Movies.Where(e => e.Category == movie.Category).Where(e => e.Id != movie.Id);
            data.Movies = similerMovie.ToList();



            return View(data);
        }
    }
}

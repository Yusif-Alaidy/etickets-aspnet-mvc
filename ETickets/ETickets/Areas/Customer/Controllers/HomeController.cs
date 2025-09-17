using ETickets.Models;
using System.Threading.Tasks;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        #region Fields
        private readonly IRepository<Movie> repositoryMovie;
        private readonly IRepository<Cinema> repositoryCinema;
        private readonly IRepository<Category> repositoryCategory;
        private readonly IRepository<Actor> repositoryActor;
        #endregion

        #region Constructor
        public HomeController(IRepository<Movie> repositoryMovie, IRepository<Cinema> repositoryCinema, IRepository<Category> repositoryCategory, IRepository<Actor> repositoryActor)
        {
            this.repositoryMovie = repositoryMovie;
            this.repositoryCategory = repositoryCategory;
            this.repositoryActor = repositoryActor;
            this.repositoryCinema = repositoryCinema;
            
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(FilterVM filterVM, int page = 1)
        {
            Home data = new();

            // Base data
            var categories = await repositoryCategory.GetAsync();
            var cinemas = await repositoryCinema.GetAsync();
            var movies = await repositoryMovie.GetAsync(include: [e => e.Category! , e => e.Cinema!]);
            
            if (movies is null) return NotFound();

            #region Filtering
            if (filterVM.search is not null)
            {
                
                movies = await repositoryMovie.GetAsync(e=>e.Name.Contains(filterVM.search));
                ViewBag.Search = filterVM.search;
            }

            if (filterVM.minPrice is not null)
            {
                movies = await repositoryMovie.GetAsync(e => e.Price >= filterVM.minPrice);
                ViewBag.MinPrice = filterVM.minPrice;
            }

            if (filterVM.maxPrice is not null)
            {
                movies = await repositoryMovie.GetAsync(e => e.Price <= filterVM.maxPrice);
                ViewBag.MaxPrice = filterVM.maxPrice;
            }

            if (filterVM.categoryId is not null)
            {
                movies = await repositoryMovie.GetAsync(e => e.Category.Id == filterVM.categoryId);
                ViewBag.CategoryId = filterVM.categoryId;
            }

            if (filterVM.cinemaId is not null)
            {
                movies = await repositoryMovie.GetAsync(e => e.Cinema!.Id == filterVM.cinemaId);
                ViewBag.CinemaId = filterVM.cinemaId;
            }
            #endregion

            #region Pagination
            var totalNumberOfPages = Math.Ceiling(movies.Count() / 10.0);
            var currentPage = page;
            ViewBag.totalNumberOfPages = totalNumberOfPages;
            ViewBag.currentPage = currentPage;

            movies = movies.Skip((page - 1) * 10).Take(10).ToList();
            #endregion

            data.Movies = movies;
            data.Categories = categories;
            data.Cinemas = cinemas;

            return View(data);
        }
        #endregion

        #region Actor
        public async Task<IActionResult> Actor(int id)
        {
            ActorDetails data = new();

            var actor = await repositoryActor.GetOneAsync(e => e.Id == id);
            if (actor == null) return NotFound();
            data.Actor = actor;

            var Movies = await repositoryMovie.GetAsync(e => e.Actors!.Any(e => e.Id == actor.Id));
            data.Movies = Movies;

            return View(data);
        }
        #endregion

        #region Movie
        // get specific movie
        public async Task<IActionResult> Movie(int id)
        {
            MovieDetails data = new();

            //var movie = _context.Movies
            //                    .Include(e => e.Category)
            //                    .Include(e => e.Cinema)
            //                    .Include(e => e.Actors)
            //                    .FirstOrDefault(e => e.Id == id);

            var movie = await repositoryMovie.GetOneAsync(e => e.Id == id, include: [e=>e.Category!,e=>e.Cinema!,e=>e.Actors!]);

            if (movie is null) return NotFound();

            data.Movie = movie;

            var similerMovie = await repositoryMovie.GetAsync(e => e.Category == movie.Category && e.Id != movie.Id);

            data.Movies = similerMovie;

            return View(data);
        }
        #endregion
    }
}

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        #region Fields
        private readonly CineBookContext _context;
        #endregion

        #region Constructor
        public HomeController(CineBookContext context)
        {
            _context = context;
        }
        #endregion

        #region Index
        public IActionResult Index(FilterVM filterVM, int page = 1)
        {
            Home data = new();

            // Base data
            var categories = _context.Categories;
            var cinemas = _context.Cinemas;
            var movies = _context.Movies
                                .Include(e => e.Category)
                                .Include(e => e.Cinema)
                                .AsQueryable();

            if (movies is null) return NotFound();

            #region Filtering
            if (filterVM.search is not null)
            {
                movies = movies.Where(e => e.Name.Contains(filterVM.search));
                ViewBag.Search = filterVM.search;
            }

            if (filterVM.minPrice is not null)
            {
                movies = movies.Where(e => e.Price >= filterVM.minPrice);
                ViewBag.MinPrice = filterVM.minPrice;
            }

            if (filterVM.maxPrice is not null)
            {
                movies = movies.Where(e => e.Price <= filterVM.maxPrice);
                ViewBag.MaxPrice = filterVM.maxPrice;
            }

            if (filterVM.categoryId is not null)
            {
                movies = movies.Where(e => e.Category.Id == filterVM.categoryId);
                ViewBag.CategoryId = filterVM.categoryId;
            }

            if (filterVM.cinemaId is not null)
            {
                movies = movies.Where(e => e.Cinema.Id == filterVM.cinemaId);
                ViewBag.CinemaId = filterVM.cinemaId;
            }
            #endregion

            #region Pagination
            var totalNumberOfPages = Math.Ceiling(movies.Count() / 10.0);
            var currentPage = page;
            ViewBag.totalNumberOfPages = totalNumberOfPages;
            ViewBag.currentPage = currentPage;

            movies = movies.Skip((page - 1) * 10).Take(10);
            #endregion

            data.Movies = movies.ToList();
            data.Categories = categories.ToList();
            data.Cinemas = cinemas.ToList();

            return View(data);
        }
        #endregion

        #region Actor
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
        #endregion

        #region Movie
        public IActionResult Movie(int id)
        {
            MovieDetails data = new();

            var movie = _context.Movies
                                .Include(e => e.Category)
                                .Include(e => e.Cinema)
                                .Include(e => e.Actors)
                                .FirstOrDefault(e => e.Id == id);

            if (movie is null) return NotFound();

            data.Movie = movie;

            var similerMovie = _context.Movies
                                       .Where(e => e.Category == movie.Category)
                                       .Where(e => e.Id != movie.Id);
            data.Movies = similerMovie.ToList();

            return View(data);
        }
        #endregion
    }
}

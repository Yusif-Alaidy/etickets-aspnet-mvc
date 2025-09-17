namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        #region Fields & Constructor
        // Repository for Movie entity
        private readonly IRepository<Movie> _repository;

        // Repository for Category entity
        private readonly IRepository<Category> _repositoryCategory;

        // Repository for Cinema entity
        private readonly IRepository<Cinema> _repositoryCinema;

        // Inject repositories through constructor
        public MovieController(IRepository<Movie> repository, IRepository<Category> repositoryCategory, IRepository<Cinema> repositoryCinema)
        {
            _repository = repository;
            _repositoryCategory = repositoryCategory;
            _repositoryCinema = repositoryCinema;
        }
        #endregion

        #region Index
        // Display list of movies including related Category and Cinema
        public async Task<IActionResult> Index()
        {
            var movies = await _repository.GetAsync(include: [e => e.Category!, e => e.Cinema!]);
            return View(movies);
        }
        #endregion

        #region Create
        // Render create form with categories and cinemas
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _repositoryCategory.GetAsync();
            var cinemas = await _repositoryCinema.GetAsync();
            if (categories is null || cinemas is null) return NoContent();

            var data = new CreateMovieVM
            {
                Categories = categories,
                Cinemas = cinemas
            };

            return View(data);
        }

        // Handle creation of new movie with image upload
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImgUrl)
        {
            if (!ModelState.IsValid)
                return View(movie);

            if (ImgUrl is null) return BadRequest();

            if (ImgUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgUrl.CopyTo(stream);
                }

                movie.ImgUrl = fileName;
            }

            var categories = await _repositoryCategory.GetAsync();
            var cinemas = await _repositoryCinema.GetAsync();
            if (categories is null || cinemas is null) return NoContent();

            var data = new CreateMovieVM
            {
                Categories = categories,
                Cinemas = cinemas
            };

            await _repository.AddAsync(movie);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Create Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        // Render update form with current movie, categories, and cinemas
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var movie = await _repository.GetOneAsync(e => e.Id == Id);
            var categories = await _repositoryCategory.GetAsync();
            var cinemas = await _repositoryCinema.GetAsync();

            if (movie == null) return NoContent();

            var data = new UpdateMovieVM
            {
                Category = categories,
                Cinema = cinemas,
                Movie = movie,
            };

            return View(data);
        }

        // Handle movie update
        [HttpPost]
        public async Task<IActionResult> Update(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                var data = new UpdateMovieVM
                {
                    Category = await _repositoryCategory.GetAsync(),
                    Cinema = await _repositoryCinema.GetAsync(),
                    Movie = movie
                };
                return View(data);
            }

            var dbMovie = await _repository.GetOneAsync(e => e.Id == movie.Id);
            if (dbMovie == null) return NotFound();

            dbMovie.Name = movie.Name;
            dbMovie.Description = movie.Description;
            dbMovie.Price = movie.Price;
            dbMovie.ImgUrl = movie.ImgUrl ?? "default.png";
            dbMovie.TrailerUrl = movie.TrailerUrl;
            dbMovie.StartDate = movie.StartDate;
            dbMovie.EndDate = movie.EndDate;
            dbMovie.MovieStatus = movie.MovieStatus;
            dbMovie.CinemaId = movie.CinemaId;
            dbMovie.CategoryId = movie.CategoryId;

            await _repository.Update(dbMovie);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Update Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        // Handle movie deletion
        public async Task<IActionResult> Delete(int Id)
        {
            var movie = await _repository.GetOneAsync(e => e.Id == Id);
            if (movie == null) return NotFound();

            await _repository.DeleteAsync(movie);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Delete Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

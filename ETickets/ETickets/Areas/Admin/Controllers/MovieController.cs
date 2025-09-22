using ETickets.Areas.Admin.ModelView;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.AdminRole}, {SD.SuperAdminRole}")]
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
            var vm = new CreateMovieVM
            {
                Categories = await GetCategoriesSelectListAsync(),
                Cinemas = await GetCinemasSelectListAsync()
            };

            return View(vm);
        }

        // Handle creation of new movie with image upload
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImgUrl)
        {
            if (ImgUrl is not null && ImgUrl.Length > 0)
            {
                movie.ImgUrl = await SaveImageAsync(ImgUrl);
            }

            await _repository.AddAsync(movie);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Create Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Helpers

        private async Task<List<SelectListItem>> GetCategoriesSelectListAsync()
        {
            return (await _repositoryCategory.GetAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
        }

        private async Task<List<SelectListItem>> GetCinemasSelectListAsync()
        {
            return (await _repositoryCinema.GetAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
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

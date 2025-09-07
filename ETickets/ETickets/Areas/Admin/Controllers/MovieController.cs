using ETickets.DataAccess;
using ETickets.Models;
using ETickets.Repositories;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //private readonly CineBookContext _context;

        private Repository<Movie> _repository;
        private Repository<Category> _repositoryCategory;
        private Repository<Cinema> _repositoryCinema;

        public MovieController(Repository<Movie> repository, Repository<Category> repositoryCategory, Repository<Cinema> repositoryCinema)
        {
            _repository = repository;
            _repositoryCategory = repositoryCategory;
            _repositoryCinema = repositoryCinema;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _repository.GetAsync(include: [e=> e.Category!, e=> e.Cinema!]);
            return View(movies);
        }

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

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImgUrl)
        {
            if (ImgUrl is null)
                return BadRequest();

            if (ImgUrl.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                // djsl-kds232-91321d-sadas-dasd213213.png
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgUrl.CopyTo(stream);
                }

                // Save img in DB
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

       

            await _repository.GetAsync();
            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var movie = await _repository.GetOneAsync(e => e.Id == Id);

            
            var categories = await _repositoryCategory.GetAsync();
            var cinemas = await _repositoryCinema.GetAsync();

            if (movie == null) 
                return NoContent();

            UpdateMovieVM data = new UpdateMovieVM
            {
                Category = categories,
                Cinema = cinemas,
                Movie = movie,
            };

            return View(data);
        }


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



            // هات الفيلم القديم
            var dbMovie = await _repository.GetOneAsync(e => e.Id == movie.Id);
            if (dbMovie == null) return NotFound();

            // عدل الخصائص اللي جت من الـ form
            dbMovie.Name = movie.Name;
            dbMovie.Description = movie.Description;
            dbMovie.Price = movie.Price;
            if (movie.ImgUrl is not null) dbMovie.ImgUrl = movie.ImgUrl;
            else dbMovie.ImgUrl = "default.png";
            dbMovie.TrailerUrl = movie.TrailerUrl;
            dbMovie.StartDate = movie.StartDate;
            dbMovie.EndDate = movie.EndDate;
            dbMovie.MovieStatus = movie.MovieStatus;
            dbMovie.CinemaId = movie.CinemaId;
            dbMovie.CategoryId = movie.CategoryId;


            await _repositoryCinema.GetAsync();

            // بعد التحديث يفضل ترجع لصفحة Index أو Details للفيلم
            return RedirectToAction("Index");
        }

        //[HttpDelete]
        public async Task<IActionResult> Delete(int Id) 
        { 
            var movie = await _repository.GetOneAsync(e => e.Id == Id);
            if (movie == null) return NotFound();

            await _repository.DeleteAsync(movie);
            await _repository.CommitAsync();

            return RedirectToAction("Index");
        }

    }
}

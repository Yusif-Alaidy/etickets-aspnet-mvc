using ETickets.DataAccess;
using ETickets.Models;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public IActionResult Index()
        {
            var movies = _context.Movies.Include(e => e.Category).Include(e => e.Cinema);
            return View(movies.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories =_context.Categories.ToList();
            var cinemas = _context.Cinemas.ToList();
            if (categories is null || cinemas is null) return NoContent(); 
            var data = new CreateMovieVM 
            { 
                Categories = categories,
                Cinemas = cinemas
            };

            return View(data);
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImgUrl)
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

            var categories = _context.Categories.ToList();
            var cinemas = _context.Cinemas.ToList();
            if (categories is null || cinemas is null) return NoContent();
            var data = new CreateMovieVM
            {
                Categories = categories,
                Cinemas = cinemas
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            //return View(data);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);

            var Categories = _context.Categories.ToList();
            var Cinemas = _context.Cinemas.ToList();

            if (movie == null) 
                return NoContent();

            UpdateMovieVM data = new UpdateMovieVM
            {
                Category = Categories,
                Cinema = Cinemas,
                Movie = movie,
            };

            return View(data);
        }


        [HttpPost]
        public IActionResult Update(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                var data = new UpdateMovieVM
                {
                    Category = _context.Categories.ToList(),
                    Cinema = _context.Cinemas.ToList(),
                    Movie = movie
                };
                return View(data);
            }



            // هات الفيلم القديم
            var dbMovie = _context.Movies.FirstOrDefault(m => m.Id == movie.Id);
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

            //_context.Movies.Update(movie);
            _context.SaveChanges();

            // بعد التحديث يفضل ترجع لصفحة Index أو Details للفيلم
            return RedirectToAction("Index");
        }

        //[HttpDelete]
        public IActionResult Delete(int id) 
        { 
            var movie = _context.Movies.FirstOrDefault(x => x.Id == id);
            if (movie == null) return NotFound();
            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}

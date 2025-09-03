using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private CineBookContext _context;
        public CinemaController(CineBookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var Cinemas = _context.Cinemas;
            if (Cinemas is null) return NotFound();

            return View(Cinemas.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Cinema Cinema, IFormFile CinemaLogo)
        {

            if (CinemaLogo is null) return BadRequest();
            if (CinemaLogo.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
                // djsl-kds232-91321d-sadas-dasd213213.png
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    CinemaLogo.CopyTo(stream);
                }

                // Save img in DB
                Cinema.CinemaLogo = fileName;
            }

            _context.Cinemas.Add(Cinema);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.Id == Id);
            if (cinema is null) return NotFound();

            return View(cinema);
        }

        [HttpPost]
        public IActionResult Update(Cinema cinema, IFormFile? CinemaLogo)
        {

            var productInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(e => e.Id == cinema.Id);

            if (productInDb is null)
                return NotFound();

            if (CinemaLogo is not null)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
                // djsl-kds232-91321d-sadas-dasd213213.png
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    CinemaLogo.CopyTo(stream);
                }

                // Remove old Img from wwwroot
                var oldFileName = productInDb.CinemaLogo;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                // Save img in DB
                cinema.CinemaLogo = fileName;
            }
            else
            {
                cinema.CinemaLogo = productInDb.CinemaLogo;
            }

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault( e => e.Id == id);
            if (cinema is null) return NoContent();

            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

    }
}

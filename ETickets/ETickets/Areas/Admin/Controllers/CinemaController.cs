using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(Cinema Cinema)
        {
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
        public IActionResult Update(Cinema Cinema)
        {
            
            _context.Cinemas.Update(Cinema);
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

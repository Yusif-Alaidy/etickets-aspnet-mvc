using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private CineBookContext _context;
        public CategoryController(CineBookContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categorys = _context.Categories;
            if (categorys is null) return NotFound();

            return View(categorys.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == Id);
            if (category is null) return NotFound();

            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {

            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(e => e.Id == id);
            if (category is null) return NoContent();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

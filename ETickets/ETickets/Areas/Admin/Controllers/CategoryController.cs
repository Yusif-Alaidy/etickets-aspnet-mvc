using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;
using ETickets.Repositories;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private Repository<Category> _repository;
        public CategoryController(Repository<Category> repository)
        {
            _repository = repository;
        }

        // Home -------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            //var categorys = _context.Categories;

            var categorys = await _repository.GetAsync();
            if (categorys is null) return NotFound();

            return View(categorys.ToList());
        }
        // ------------------------------------------------------------------

        // Create -----------------------------------------------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            //_context.Categories.Add(category);
            //_context.SaveChanges();

            await _repository.AddAsync(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Create Successfully";
            return RedirectToAction(nameof(Index));
        }
        // ------------------------------------------------------------------

        // Update -----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == Id);

            var category = await _repository.GetOneAsync(e => e.Id == Id);
            if (category is null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {

            //_context.Categories.Update(category);
            //_context.SaveChanges();

            await _repository.Update(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Update Successfully";

            return RedirectToAction(nameof(Index));
        }
        // --------------------------------------------------------------------

        // Delete -------------------------------------------------------------
        public async Task<IActionResult> Delete(int Id)
        {
            //var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            var category = await _repository.GetOneAsync(e => e.Id == Id);
            if (category is null) return NoContent();

            //_context.Categories.Remove(category);
            //_context.SaveChanges();

            await _repository.DeleteAsync(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Delete Successfully";
            return RedirectToAction(nameof(Index));
        }
        // --------------------------------------------------------------------
    }
}

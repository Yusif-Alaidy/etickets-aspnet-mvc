using Microsoft.AspNetCore.Authorization;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.AdminRole}, {SD.SuperAdminRole}")]

    public class CategoryController : Controller
    {
        #region Fields
        private readonly IRepository<Category> _repository;
        #endregion

        #region Constructor
        public CategoryController(IRepository<Category> repository)
        {
            _repository = repository;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            var categories = await _repository.GetAsync();
            if (categories is null) return NotFound();

            return View(categories.ToList());
        }
        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _repository.AddAsync(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Create Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _repository.GetOneAsync(e => e.Id == id);
            if (category is null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _repository.Update(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Update Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetOneAsync(e => e.Id == id);
            if (category is null) return NoContent();

            await _repository.DeleteAsync(category);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Delete Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

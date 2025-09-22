namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.AdminRole}, {SD.SuperAdminRole}")]
    public class CinemaController : Controller
    {
        #region Fields & Constructor
        // Repository to handle data operations for Cinema entity
        private readonly IRepository<Cinema> _repository;

        // Inject repository through constructor
        public CinemaController(IRepository<Cinema> repo)
        {
            _repository = repo;
        }
        #endregion

        #region Index
        // Display list of all cinemas
        public async Task<IActionResult> Index()
        {
            var Cinemas = await _repository.GetAsync();
            if (Cinemas is null) return NotFound();

            return View(Cinemas.ToList());
        }
        #endregion

        #region Create
        // Render create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Handle creation of new cinema with logo upload
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile cinemaLogo)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            if (cinemaLogo is null) return BadRequest();

            if (cinemaLogo.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(cinemaLogo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    cinemaLogo.CopyTo(stream);
                }

                cinema.CinemaLogo = fileName;
            }

            await _repository.AddAsync(cinema);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Create Successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        // Render update form
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var cinema = _repository.GetOneAsync(e => e.Id == Id);
            if (cinema is null) return NotFound();

            return View(cinema);
        }

        // Handle update of cinema details and logo replacement
        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema, IFormFile? CinemaLogo)
        {
            if (!ModelState.IsValid)
                return View(cinema);

            var productInDb = await _repository.GetOneAsync(e => e.Id == cinema.Id);
            if (productInDb is null) return NotFound();

            if (CinemaLogo is not null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(CinemaLogo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    CinemaLogo.CopyTo(stream);
                }

                var oldFileName = productInDb.CinemaLogo;
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldFileName);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                cinema.CinemaLogo = fileName;
            }
            else
            {
                cinema.CinemaLogo = productInDb.CinemaLogo;
            }

            await _repository.Update(cinema);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Update Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        // Handle cinema deletion
        public async Task<IActionResult> Delete(int Id)
        {
            var cinema = await _repository.GetOneAsync(e => e.Id == Id);
            if (cinema is null) return NoContent();

            await _repository.DeleteAsync(cinema);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Delete Successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

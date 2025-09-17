namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        #region Fields
        private readonly IRepository<Actor> _repository;
        #endregion

        #region Constructor
        public ActorController(IRepository<Actor> repo)
        {
            _repository = repo;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            var actors = await _repository.GetAsync();
            if (actors is null) return NotFound();

            return View(actors);
        }
        #endregion

        #region Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile profilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            if (profilePicture is null)
                return BadRequest();

            if (profilePicture.Length > 0)
            {
                // Save file to wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                // Save file path in DB
                actor.ProfilePicture = fileName;
            }

            await _repository.AddAsync(actor);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Create Successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var actor = await _repository.GetOneAsync(c => c.Id == id);
            if (actor is null) return NotFound();

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }

            await _repository.Update(actor);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Update Successfully";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _repository.GetOneAsync(e => e.Id == id);
            if (actor is null) return NoContent();

            await _repository.DeleteAsync(actor);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Delete Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}

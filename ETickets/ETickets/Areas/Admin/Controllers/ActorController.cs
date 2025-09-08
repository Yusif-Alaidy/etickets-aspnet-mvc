using ETickets.DataAccess;
using ETickets.Models;
using ETickets.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {

        private Repository<Actor> _repository;
        public ActorController(Repository<Actor> repo)
        {
            _repository = repo;
        }

        public async Task<IActionResult> Index()
        {
            var Actors = await _repository.GetAsync();
            if (Actors is null) return NotFound();

            return View(Actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor Actor, IFormFile ProfilePicture)
        {

            if (!ModelState.IsValid)
            {
                return View(Actor); // validation messages will show up
            }

            if (ProfilePicture is null)
                return BadRequest();

            if (ProfilePicture.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                // djsl-kds232-91321d-sadas-dasd213213.png
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ProfilePicture.CopyToAsync(stream);
                }

                // Save img in DB
                Actor.ProfilePicture = fileName;
            }

            
            await _repository.AddAsync(Actor);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Create Successfully";


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var Actor = await _repository.GetOneAsync(c => c.Id == Id);
            if (Actor is null) return NotFound();

            return View(Actor);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Actor Actor)
        {

            if (!ModelState.IsValid)
            {
                return View(Actor); // validation messages will show up
            }

            await _repository.Update(Actor);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Update Successfully";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {

            var Actor = await _repository.GetOneAsync(e => e.Id == Id);
            if (Actor is null) return NoContent();

            await _repository.DeleteAsync(Actor);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Create Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

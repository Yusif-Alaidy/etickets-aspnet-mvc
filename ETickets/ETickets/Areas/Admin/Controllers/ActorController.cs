using ETickets.DataAccess;
using ETickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private CineBookContext _context;
        public ActorController(CineBookContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Actors = _context.Actors;
            if (Actors is null) return NotFound();

            return View(Actors.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor Actor, IFormFile ProfilePicture)
        {

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

            await _context.Actors.AddAsync(Actor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var Actor = await _context.Actors.FirstOrDefaultAsync(c => c.Id == Id);
            if (Actor is null) return NotFound();

            return View(Actor);
        }
        [HttpPost]
        public IActionResult Update(Actor Actor)
        {

            _context.Actors.Update(Actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var Actor = _context.Actors.FirstOrDefault(e => e.Id == id);
            if (Actor is null) return NoContent();

            _context.Actors.Remove(Actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

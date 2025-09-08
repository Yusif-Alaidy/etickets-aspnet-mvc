using ETickets.DataAccess;
using ETickets.Models;
using ETickets.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private Repository<Cinema> _repository;
        public CinemaController(Repository<Cinema> repo)
        {
            _repository = repo;
        }

        public async Task<IActionResult> Index()
        {
            var Cinemas = await _repository.GetAsync();
            if (Cinemas is null) return NotFound();

            return View(Cinemas.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile cinemaLogo)
        {

            if (!ModelState.IsValid)
            {
                return View(cinema); // validation messages will show up
            }


            if (cinemaLogo is null) return BadRequest();
            if (cinemaLogo.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(cinemaLogo.FileName);
                // djsl-kds232-91321d-sadas-dasd213213.png
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    cinemaLogo.CopyTo(stream);
                }

                // Save img in DB
                cinema.CinemaLogo = fileName;
            }

            
            await _repository.AddAsync(cinema);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Create Successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int Id)
        {
            var cinema = _repository.GetOneAsync(e => e.Id == Id);
            if (cinema is null) return NotFound();

            return View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema, IFormFile? CinemaLogo)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema); // validation messages will show up
            }


            var productInDb = await _repository.GetOneAsync(e => e.Id == cinema.Id); // add no Traking

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

 
 

            await _repository.Update(cinema);
            await _repository.CommitAsync();

            TempData["Success-Notification"] = "Update Successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var cinema = await _repository.GetOneAsync(e => e.Id == Id);
            if (cinema is null) return NoContent();

            await _repository.DeleteAsync(cinema);
            await _repository.CommitAsync();
            TempData["Success-Notification"] = "Delete Successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}

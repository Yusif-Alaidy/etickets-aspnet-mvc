using ETickets.Models;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        public UserManager<ApplicationUser> _userManager { get; }
        public AccountController(UserManager<ApplicationUser> userManager) 
        {
            _userManager = userManager;
        }

        // Register Actions -----------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            ApplicationUser applicationUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
            };

            var resualt = await _userManager.CreateAsync(applicationUser,request.Password);
            if (!resualt.Succeeded) 
            {
                foreach (var item in resualt.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            TempData["Success-Notification"] = "Add User Successfuly";
            //return RedirectToAction(nameof(Index));
            return View();
        }
        // -----------------------------------------------------------------------
    }
}

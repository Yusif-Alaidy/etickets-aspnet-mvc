using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdminRole}")]
    public class UserController : Controller
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion
        #region constructor
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        #endregion
        #region Index -> return all users
        public IActionResult Index()
        {
            var users = _userManager.Users;

            return View(users.ToList());
        }
        #endregion
    }
}

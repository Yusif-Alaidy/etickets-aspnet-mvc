using ETickets.Models;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace ETickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        #region Fields & Constructor

        private readonly IEmailSender emailSender;
        public UserManager<ApplicationUser> _userManager { get; }

        // Inject UserManager and EmailSender
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            this.emailSender = emailSender;
        }

        #endregion

        #region Register

        // GET: Register page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Handle register form submission
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // Create application user from request
            ApplicationUser applicationUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
            };

            // Save user with password
            var result = await _userManager.CreateAsync(applicationUser, request.Password);
            if (!result.Succeeded)
            {
                // Show validation errors from Identity
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(request);
            }

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            token = WebUtility.UrlEncode(token);

            // Build confirmation link
            var link = Url.Action(nameof(ConfirmEmail), "Account",
                new { area = "Identity", userId = applicationUser.Id, token = token },
                Request.Scheme);

            // Send confirmation email
            await emailSender.SendEmailAsync(
                applicationUser.Email,
                "Confirm Your Account!",
                $"<h1>Confirm your account by clicking <a href='{link}'>here</a></h1>");

            // Success message
            TempData["Success-Notification"] = "User created successfully, please confirm your account";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        #endregion

        #region Confirm Email

        // Confirm user email using token
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, WebUtility.UrlDecode(token));

            if (!result.Succeeded)
            {
                TempData["Error-Notification"] = "Invalid token, please resend confirmation email";
            }
            else
            {
                TempData["Success-Notification"] = "Account activated successfully";
            }

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        #endregion
    }
}

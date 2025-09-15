using ETickets.Models;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Net;
using System.Threading.Tasks;

namespace ETickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IEmailSender emailSender;

        public UserManager<ApplicationUser> _userManager { get; }
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            this.emailSender = emailSender;
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

            var resualt = await _userManager.CreateAsync(applicationUser, request.Password);
            if (!resualt.Succeeded) 
            {
                foreach (var item in resualt.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(request);

            }

            // Send Email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            token = WebUtility.UrlEncode(token);

            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = applicationUser.Id, token = token }, Request.Scheme);
            //var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", UserId = applicationUser.Id, Token = token }, Request.Scheme);


            await emailSender.SendEmailAsync(applicationUser.Email,
                "Confirm Your Account!", $"<h1> Confirm Your Account By Clicking <a href='{link}'>here</a> </h1>");

            TempData["Success-Notification"] = "Add User Successfuly, please confirm your account";
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        }
        // -----------------------------------------------------------------------

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, WebUtility.UrlDecode(token));

            if (!result.Succeeded)
            {
                TempData["Error-Notification"] = "Inalid Token, Resend Email Confirimation";
            }
            else 
            {
                TempData["Success-Notification"] = "Activate Account Successfully";
                
            }

            //return RedirectToAction(nameof(Login));
            return RedirectToAction("Index", "Home", new { area = "Customer" });

        }
    }
}

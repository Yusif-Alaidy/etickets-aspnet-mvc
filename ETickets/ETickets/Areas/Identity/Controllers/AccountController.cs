using ETickets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace ETickets.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        #region Fields 

        private readonly IEmailSender emailSender;
        private readonly SignInManager<ApplicationUser> signInManager;
        public UserManager<ApplicationUser> _userManager { get; }

        #endregion

        #region Constructor
        // Inject UserManager and EmailSender
        public AccountController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            this.emailSender = emailSender;
            this.signInManager = signInManager;
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

        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already authenticated, if yes redirect to Home
            if (User.Identity is not null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            // Try to find user by username or email
            var user = await _userManager.FindByNameAsync(loginVM.EmailORUserName) ?? await _userManager.FindByEmailAsync(loginVM.EmailORUserName);

            if (user is null)
            {
                TempData["error-notification"] = "Invalid User Name/Email Or Password";
                return View(loginVM);
            }

            var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

            if (!result)
            {
                TempData["error-notification"] = "Invalid User Name/Email Or Password";
                return View(loginVM);
            }

            if (!user.EmailConfirmed)
            {
                TempData["error-notification"] = "Confirm Your Account!";
                return View(loginVM);
            }

            if (!user.LockoutEnabled)
            {
                TempData["error-notification"] = $"You have a block till {user.LockoutEnd}";
                return View(loginVM);
            }

            await signInManager.SignInAsync(user, loginVM.RememberMe);
            TempData["success-notification"] = "Login Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }



        #endregion // Authentication actions (Login & Logout)

        #region Logout

        public async Task<IActionResult> Logout()
        {
            // Sign out the current user and clear authentication cookies
            await signInManager.SignOutAsync();
            TempData["success-notification"] = "Logout Successfully";
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        #endregion

        #region ResendEmailConfirmation

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resendEmailConfirmationVM);
            }
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailORUserName) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailORUserName);

            if (user is null)
            {
                TempData["error-notification"] = "Invalid User Name/Email Or Password";
                return View(resendEmailConfirmationVM);
            }

            // Send Email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);




            //var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token = token }, Request.Scheme);
            var link = Url.Action(nameof(ConfirmEmail), "Account",
                new { area = "Identity", userId = user.Id, token = token },
                Request.Scheme);
            await emailSender.SendEmailAsync(user.Email!, "Confirm Your Account!", $"<h1>Confirm Your Account By Clicking <a href='{link}'>here</a></h1>");

            TempData["success-notification"] = "Send Email successfully, Please Confirm Your Account";
            return RedirectToAction("Login");

           
        }

        #endregion

    }
}

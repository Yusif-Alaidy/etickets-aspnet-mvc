using Microsoft.AspNetCore.Mvc;

namespace ETickets.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Actor()
        {
            return View();
        }
        public IActionResult Movie()
        {
            return View();
        }
    }
}

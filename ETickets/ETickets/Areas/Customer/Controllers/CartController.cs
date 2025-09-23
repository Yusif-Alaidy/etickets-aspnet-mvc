using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ETickets.Areas.Customer.Controllers
{

    [Authorize]
    [Area("Customer")]

    //[Area(SD.CustomerArea)]
    public class CartController : Controller
    {
        #region Fields
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Cart> repositoryCart;
        #endregion

        #region Constructore
        public CartController(UserManager<ApplicationUser> userManager, IRepository<Cart> repositoryCart)
        {
            this.userManager = userManager;
            this.repositoryCart = repositoryCart;
        }
        #endregion

        #region Home
        public async Task<IActionResult> Index()
        {
            // get user
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            // get me all carts for this user
            var carts = await repositoryCart.GetAsync(e => e.ApplicationUserId == user.Id, include: [e => e.Movie]);

            ViewBag.totalAmount = carts.Sum(e => e.Movie.Price * e.Count);

            return View(carts);
        }
        #endregion

        #region Add To Cart
        public async Task<IActionResult> AddToCart(AddToCartVM vm)
        {
            // get user
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // check if this cart already exist
            var cart = await repositoryCart.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == vm.MovieId);
            if (cart is not null)
            {
                cart.Count = +vm.Count;
                TempData["success-notification"] = "Update to cart successfuly";
            }
            else
            {
                // create cart
                await repositoryCart.AddAsync(new()
                {
                    ApplicationUserId = user.Id,
                    MovieId = vm.MovieId,
                    Count = vm.Count
                });
            }

            await repositoryCart.CommitAsync();

            return RedirectToAction("Index", "Cart", new { Area = "Customer" });
        }
        #endregion

        #region IncrementCount
        public async Task<IActionResult> IncrementCount(int MovieId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await repositoryCart.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if (cart is not null)
            {
                cart.Count += 1;
                await repositoryCart.CommitAsync();
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region DecrementCount
        public async Task<IActionResult> DecrementCount(int MovieId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await repositoryCart.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if (cart is not null)
            {
                if (cart.Count > 1)
                {
                    cart.Count -= 1;
                    await repositoryCart.CommitAsync();
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region DeleteProductFromCart
        public async Task<IActionResult> DeleteProductFromCart(int MovieId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return NotFound();

            var cart = await repositoryCart.GetOneAsync(e => e.ApplicationUserId == user.Id && e.MovieId == MovieId);

            if (cart is not null)
            {
                await repositoryCart.DeleteAsync(cart);
                await repositoryCart.CommitAsync();
            }

            return RedirectToAction("Index");
        }
        #endregion

    }
}

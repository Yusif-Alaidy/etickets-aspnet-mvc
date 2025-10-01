using ETickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace ETickets.Areas
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> userManger;
        private readonly ILogger<CheckoutController> _logger;
        private readonly IRepository<Cart> repositoryCart;
        private readonly IRepository<Order> repositoryOrder;
        private readonly IRepository<OrderItems> repositoryOrderItem;
        private readonly IEmailSender emailSender;
        private readonly CineBookContext _context;

        public CheckoutController(UserManager<ApplicationUser> userManger, ILogger<CheckoutController> logger, IRepository<Cart> repositoryCart, IRepository<Order> repositoryOrder, IRepository<OrderItems> repositoryOrderItem, IEmailSender emailSender, CineBookContext _context)
        {
            this.userManger = userManger;
            this._logger = logger;
            this.repositoryCart = repositoryCart;
            this.repositoryOrder = repositoryOrder;
            this.repositoryOrderItem = repositoryOrderItem;
            this.emailSender = emailSender;
            this._context = _context;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            //var transaction = _context.Database.BeginTransaction();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await userManger.GetUserAsync(User);
                var carts = await repositoryCart.GetAsync(e => e.ApplicationUserId == user.Id, include: [e => e.Movie]);
                var orders = await repositoryOrder.GetOneAsync(e => e.Id == orderId);

                // 1. Transform Cart => Order items

                //List<OrderItems> orderItems = []

                //foreach (var item in carts)
                //{
                //    orderItems.Add(new()
                //    {
                //        OrderId = orderId,
                //        MovieId = item.MovieId,
                //        Count = item.Count,
                //        Price = item.Movie.Price
                //    });
                //}

               
                var orderItems = carts.Select(e => new OrderItems()
                {
                    MovieId = e.MovieId,
                    OrderId = orders.Id,
                    Count = e.Count,
                    Price = (decimal)e.Movie.Price
                }).ToList();


                await repositoryOrderItem.CreateRangeAsync(orderItems);



                // 2. Decrement Quantity -> movie
                foreach (var item in carts)
                {
                    item.Movie.Quantity -= item.Count;
                }

                // 3. Delete Old Cart
                await repositoryCart.DeleteRangeAsync(carts);
                await repositoryCart.CommitAsync();
                // 4. Update Order Prop.
                var order = await repositoryOrder.GetOneAsync(e => e.Id == orderId);

                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.SessionId = session.Id;

                order.OrderStatus = OrderStatus.Completed;
                //order.TransactionStatus = true;
                order.TransctionId = session.PaymentIntentId;

                await repositoryOrder.CommitAsync();

                // 5. Send Email to user
                await emailSender.SendEmailAsync(user.Email, "Thanks", "Order Completed");
                transaction.Commit();
                return View();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);

                transaction.Rollback();

            }

            return View();
        }

        public async Task<IActionResult> Cancel(int orderId)
        {
            var order = await repositoryOrder.GetOneAsync(e => e.Id == orderId);

            if (order is null)
                return NotFound();

            // update order status
            order.OrderStatus = OrderStatus.Canceled;

            await repositoryOrder.CommitAsync();

            return View();

        }
    }
}

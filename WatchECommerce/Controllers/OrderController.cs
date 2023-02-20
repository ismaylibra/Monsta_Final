using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;

namespace WatchECommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public OrderController(WatchDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OrderProduct()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("LogIn", "Account");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user is null) return BadRequest();

            var basket = await _dbContext
                    .Baskets
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.BasketProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

            if (basket is null) return NotFound();

            _dbContext.Baskets.Remove(basket);

            var order = new Order
            {
                UserId = user.Id,
                CreateTime = DateTime.Now,
                OrderItems = new List<OrderItem>()
            };

            foreach (var product in basket.BasketProducts)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.ProductId,
                    OrderId = order.Id,
                    Image = product.Product.MainImageUrl,
                    Count = product.Count
                   
                    
                });
            }

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;

namespace WatchECommerce.Controllers
{
    [Authorize(Roles = "User")]
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
        public IActionResult CheckOut()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(Order order)
        {
            if (!ModelState.IsValid) return View();
            User user = await _userManager.FindByNameAsync(User.Identity.Name);

            List<BasketItem> items = await _dbContext.BasketItems
                .Include(b => b.User)
                .Include(b => b.Product)
                .Where(b=>b.UserId == user.Id)
                .ToListAsync();

            order.BasketItems = items;
            order.User = user;
            order.Date = DateTime.Now;
            order.Status = null;
            order.TotalPrice = default;

            foreach (var item in items)
            {
                order.TotalPrice += item.Price*item.Quantity;
            }
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index","Home");



        }
    }
}

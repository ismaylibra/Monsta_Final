using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public ProductController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Brand)
                .ToListAsync();

            return View(products);
        }
    }
}

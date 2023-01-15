using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public HomeController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _dbContext.Sliders
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var banners = await _dbContext.Banners
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var blogs = await _dbContext.Blogs
                .Where(s => !s.IsDeleted)
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var products = await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .Include(p=>p.ProductImages)
                .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();


            var homeViewModel = new HomeViewModel
            {
                Sliders = sliders,
                Banners = banners,
                Blogs = blogs,
                Products = products
            };
           
            return View(homeViewModel);
        }
    }
}

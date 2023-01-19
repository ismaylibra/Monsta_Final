using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public PortfolioController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _dbContext.Blogs.Where(b => !b.IsDeleted).ToListAsync();
            return View(blogs);
        }
    }
}

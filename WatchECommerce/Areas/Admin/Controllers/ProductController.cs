using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.Areas.Admin.Controllers
{

    public class ProductController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public ProductController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _dbContext.Products.Where(p => !p.IsDeleted).ToListAsync();
            return View(products);
        }
    }
}

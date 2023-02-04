using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

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
        public async Task<IActionResult> Details (int? id)
        {
            if (id is null) return BadRequest();
            var product = await _dbContext.Products
                .Where(p => p.Id == id).
                Include(p => p.ProductImages)
                .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                .FirstOrDefaultAsync();

           

            if(product is null) return NotFound();

            if (product.Id != id) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> ShopSideBarCategory(int? id)
        {
            var categories = await _dbContext.ProductCategories
                .Where(c => c.Id == id)
                .Include(c => c.CategoryProducts).ThenInclude(c => c.Product)
                .ToListAsync();
            var products = await _dbContext.Products
                            .Where(p => !p.IsDeleted)
                            .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                            .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                            .Include(p => p.ProductImages)
                            .Include(p => p.Brand)
                            .ToListAsync();
            var model = new ShopSideBarCategoryViewModel
            {
                ProductCategories = categories,
                Products = products
            };
            return View(model);
        }
        public async Task<IActionResult> ShopSideBarBrand(int? id)
        {
            var brands = await _dbContext.Brands
                .Where(c => c.Id == id)
                .Include(c => c.Products)
                .ToListAsync();
            var products = await _dbContext.Products
                            .Where(p => !p.IsDeleted)
                            .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                            .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                            .Include(p => p.ProductImages)
                            .Include(p => p.Brand)
                            .ToListAsync();
            var model = new ShopSideBarBrandViewModel
            {
                Brands = brands,
                Products = products
            };
            return View(model);
        }

        public async Task<IActionResult> ShopSideBarColor(int? id)
        {
            var colors = await _dbContext.Colors
                .Where(c => c.Id == id)
                .Include(c => c.ProductColors).ThenInclude(c=>c.Product)
                .ToListAsync();
            var products = await _dbContext.Products
                            .Where(p => !p.IsDeleted)
                            .Include(p => p.ProductColors).ThenInclude(p => p.Color)
                            .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                            .Include(p => p.ProductImages)
                            .Include(p => p.Brand)
                            .ToListAsync();
            var model = new ShopSideBarColorViewModel
            {
                Colors = colors,
                Products = products
            };
            return View(model);
        }


 

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Watch.BLL.Data;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public ProductController(WatchDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

            if (User.Identity.IsAuthenticated)
            {

                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var wishList = await _dbContext
                    .WishLists
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                if (wishList != null)
                {
                    foreach (var WishlisProducts in wishList.WishListProducts)
                    {
                        var product = await _dbContext.Products.
                            Where(p => p.Id == WishlisProducts.Product.Id)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    foreach (var productId in productIdList)
                    {
                        var product = await _dbContext.Products.
                            Where(p => p.Id == productId)
                            .FirstOrDefaultAsync();

                        if (product is not null)
                        {
                            product.IsLike = true;
                        }
                    }
                }
            }

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

        public async Task<IActionResult> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return NoContent();

            var products = await _dbContext.Products
                .Where(product => !product.IsDeleted && product.Brand.Name.ToLower().StartsWith(searchText.ToLower()))
                .Include(p=>p.Brand)
                .ToListAsync();

            var model = new List<Product>();

            products.ForEach(product => model.Add(new Product
            {
                Id = product.Id,
                Name = product.Brand.Name,
                MainImageUrl = product.MainImageUrl,
            }));

            return PartialView("_ProductSearchPartial", products);
        }




    }
}

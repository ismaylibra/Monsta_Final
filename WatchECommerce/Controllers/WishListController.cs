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
    public class WishListController : Controller
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public WishListController(WatchDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<WishListViewModel> model = new();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var wishList = await _dbContext
                    .WishLists
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                foreach (var item in wishList.WishListProducts)
                {
                    model.Add(new WishListViewModel
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        Price = item.Product.Price,
                        DiscountPrice =item.Product.DiscountPrice,
                        ImageUrl = item.Product.MainImageUrl
                    });
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    foreach (var productId in productIdList)
                    {
                        var product = await _dbContext
                            .Products
                            .Where(p => p.Id == productId)
                            .FirstOrDefaultAsync();

                        model.Add(new WishListViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            DiscountPrice = (decimal)product.DiscountPrice,
                            ImageUrl = product.MainImageUrl
                        });
                    }


                }
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductToWishList(int? productId)
        {
            if (productId == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                var user= await _userManager.FindByNameAsync(User.Identity.Name);
                if(user == null) return BadRequest();

                var existWishList = await _dbContext
                    .WishLists
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .FirstOrDefaultAsync();

                if (existWishList is not null)
                {
                    var createdWishList = new WishList
                    {
                        UserId = user.Id,
                        WishListProducts = new List<WishListProduct>()
                    };


                    var existProduct = await _dbContext.Products
                        .Where(p => p.Id == productId)
                        .FirstOrDefaultAsync();

                    if (existProduct is null) return NotFound();

                    if (existWishList.WishListProducts.Any(x => x.ProductId == existProduct.Id)) return NotFound();

                    existWishList.WishListProducts.Add(new WishListProduct
                    {
                        WishListId = createdWishList.Id,
                        ProductId = existProduct.Id
                    });

                     _dbContext.Update(existWishList);

                }
                else
                {
                    var createdWishList = new WishList
                    {
                        UserId = user.Id,
                        WishListProducts = new List<WishListProduct>()
                    };

                    var wishlistProducts = new List<WishListProduct>();

                    var existProduct = await _dbContext.Products
                        .Where(p => p.Id == productId)
                        .FirstOrDefaultAsync();

                    if (existProduct is null) return NotFound();

                    wishlistProducts.Add(new WishListProduct
                    {
                        WishListId = createdWishList.Id,
                        ProductId = existProduct.Id
                    });

                    createdWishList.WishListProducts = wishlistProducts;

                    await _dbContext.WishLists.AddAsync(createdWishList);
                }

                await _dbContext.SaveChangesAsync();

            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);
                    if (productIdList.Contains(productId.Value))
                        return NoContent();

                    productIdList.Add(productId.Value);

                    var productIdListJson = JsonConvert.SerializeObject(productIdList);
                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);

                }
                else
                {
                    var productIdListJson = JsonConvert.SerializeObject(new List<int> { productId.Value });
                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);
                }

            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductFromWishList(int? productId)
        {
            if(productId == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user is null) return BadRequest();

                var wishList = await _dbContext
                    .WishLists
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.WishListProducts)
                    .FirstOrDefaultAsync();

                var existProduct = await _dbContext.Products.FindAsync(productId);

                if (existProduct is null) return NotFound();

                var existWishListProduct = await _dbContext
                    .WishListProducts
                    .Where(x => x.ProductId == existProduct.Id)
                    .FirstOrDefaultAsync();

                wishList.WishListProducts.Remove(existWishListProduct);
                _dbContext.Update(wishList);

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.WISH_LIST_COOKIE_NAME, out var cookie))
                {
                    var productIdList = JsonConvert.DeserializeObject<List<int>>(cookie);

                    productIdList.Remove(productId.Value);

                    var productIdListJson = JsonConvert.SerializeObject(productIdList);

                    Response.Cookies.Append(Constants.WISH_LIST_COOKIE_NAME, productIdListJson);
                }
            }

            return NoContent();
        }

    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Watch.BLL.Data;
using Watch.Core.Entities;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class BasketController : Controller
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public BasketController(WatchDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<BasketProductViewModel> model = new();


            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var basket = await _dbContext
                    .Baskets
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.BasketProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync();

                foreach (var item in basket.BasketProducts)
                {
                    var product = _dbContext.Products
                        .Where(p => p.Id == item.Product.Id && !p.IsDeleted)
                        .FirstOrDefault();

                    model.Add(new BasketProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Count = item.Count,
                        MainImageUrl = product.MainImageUrl,
                    });
                }
            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.BASKET_COOKIE_NAME, out var cookie))
                {
                    var productList = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookie);

                    foreach (var item in productList)
                    {
                        var product = _dbContext.Products
                            .Where(p => p.Id == item.Id && !p.IsDeleted)
                            .FirstOrDefault();

                        model.Add(new BasketProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Count = item.Count,
                            MainImageUrl = product.MainImageUrl
                        });
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int? productId)
        {
            if (productId is null) return NotFound();

            var product = await _dbContext.Products
                   .Where(product => product.Id == productId)
                   .FirstOrDefaultAsync();

            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var existBasket = await _dbContext
                   .Baskets
                   .Where(x => x.UserId == user.Id)
                   .Include(x => x.BasketProducts)
                   .FirstOrDefaultAsync();

                if (existBasket != null)
                {
                    var existBasketProduct = existBasket.BasketProducts
                      .Where(x => x.ProductId == product.Id)
                      .FirstOrDefault();

                    if (existBasketProduct is not null)
                    {
                        existBasketProduct.Count++;
                    }
                    else
                    {
                        var createdBasket = new Basket
                        {
                            UserId = user.Id,
                            BasketProducts = new List<BasketProduct>()
                        };

                        existBasket.BasketProducts.Add(new BasketProduct
                        {
                            BasketId = createdBasket.Id,
                            ProductId = product.Id,
                            Count = 1
                        });
                    }

                    _dbContext.Update(existBasket);
                }
                else
                {
                    var createdBasket = new Basket
                    {
                        UserId = user.Id,
                        BasketProducts = new List<BasketProduct>()
                    };

                    var basketProducts = new List<BasketProduct>
                    {
                        new BasketProduct
                        {
                            BasketId = createdBasket.Id,
                            ProductId = product.Id,
                            Count = 1
                        }
                    };

                    createdBasket.BasketProducts = basketProducts;

                    await _dbContext.Baskets.AddAsync(createdBasket);
                }

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var basket = Request.Cookies[Constants.BASKET_COOKIE_NAME];
                var basketItems = new List<BasketViewModel>();

                var basketItem = new BasketViewModel
                {
                    Id = product.Id,
                    Count = 1,
                };

                if (basket is not null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);

                    var existProduct = basketItems
                        .Where(x => x.Id == product.Id)
                        .FirstOrDefault();

                    if (existProduct is not null) existProduct.Count++;
                    else basketItems.Add(basketItem);
                }
                else
                {
                    basketItems.Add(basketItem);
                }

                Response.Cookies.Append(Constants.BASKET_COOKIE_NAME, JsonConvert.SerializeObject(basketItems));
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductQuality(int? productId, int count)
        {
            if (productId is null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var basket = await _dbContext
                    .Baskets
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.BasketProducts)
                    .FirstOrDefaultAsync();

                var existProduct = basket.BasketProducts.Where(product => product.ProductId == productId).FirstOrDefault();

                if (existProduct is null) return NotFound();

                existProduct.Count = count;

                _dbContext.Update(existProduct);

                await _dbContext.SaveChangesAsync();

            }
            else
            {

                if (Request.Cookies.TryGetValue(Constants.BASKET_COOKIE_NAME, out var cookie))
                {
                    var productList = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookie);

                    var existProduct = productList.Where(x => x.Id == productId).FirstOrDefault();

                    existProduct.Count = count;

                    var productIdListJson = JsonConvert.SerializeObject(productList);

                    Response.Cookies.Append(Constants.BASKET_COOKIE_NAME, productIdListJson);
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductBasket(int? productId)
        {
            if (productId == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user == null) return BadRequest();

                var existProduct = await _dbContext.Products.FindAsync(productId);

                if (existProduct == null) return NotFound();

                var existBasket = await _dbContext.Baskets
                    .Where(x => x.UserId == user.Id)
                   .Include(x => x.BasketProducts)
                   .FirstOrDefaultAsync();

                var existBasketProduct = existBasket.BasketProducts
                    .FirstOrDefault(x => x.ProductId == existProduct.Id);

                existBasket.BasketProducts.Remove(existBasketProduct);

                _dbContext.Update(existBasket);

                await _dbContext.SaveChangesAsync();

            }
            else
            {
                if (Request.Cookies.TryGetValue(Constants.BASKET_COOKIE_NAME, out var cookie))
                {
                    var productList = JsonConvert.DeserializeObject<List<BasketViewModel>>(cookie);

                    var existProduct = productList.Where(x => x.Id == productId).FirstOrDefault();

                    productList.Remove(existProduct);

                    var productIdListJson = JsonConvert.SerializeObject(productList);

                    Response.Cookies.Append(Constants.BASKET_COOKIE_NAME, productIdListJson);
                }
            }

            return NoContent();
        }






    }
}

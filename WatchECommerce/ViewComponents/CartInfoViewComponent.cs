using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Watch.BLL.Data;
using Watch.Core.IdentityModels;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.ViewComponents
{
    public class CartInfoViewComponent : ViewComponent
    {
        private readonly WatchDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CartInfoViewComponent(WatchDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<BasketProductViewModel> model = new();


            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);


                var basket = await _dbContext
                    .Baskets
                    .Where(x => x.UserId == user.Id)
                    .Include(x => x.BasketProducts)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync();

                if(basket?.BasketProducts is not null)
                {
                    foreach (var item in basket.BasketProducts)
                    {
                        var product = _dbContext.Products
                            .Where(p => p.Id == item.Product.Id && !p.IsDeleted)
                            .FirstOrDefault();

                        var finalPrice = default(decimal);

                        if (product.DiscountPrice > 0)
                        {
                            finalPrice = (decimal)product.Price - (decimal)product.DiscountPrice;
                        }
                        else
                        {
                            finalPrice = (decimal)product.Price;
                        }
                        model.Add(new BasketProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = finalPrice,
                            Count = item.Count,
                            MainImageUrl = product.MainImageUrl,
                        });
                    }

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
                        var finalPrice = default(decimal);

                        if (product.DiscountPrice > 0)
                        {
                            finalPrice = (decimal)product.Price - (decimal)product.DiscountPrice;
                        }
                        else
                        {
                            finalPrice = (decimal)product.Price;
                        }

                        model.Add(new BasketProductViewModel
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = finalPrice,
                            Count = item.Count,
                            MainImageUrl = product.MainImageUrl
                        });
                    }
                }
            }
            return View(model);
        }
    }
}

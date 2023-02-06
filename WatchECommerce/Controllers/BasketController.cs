using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Watch.BLL.BasketViewModels;
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
            return View();
        }
         public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id is null || id == 0) return NotFound();
            Product product = await _dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();

            if (product is null) return NotFound();

            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);

                if (user is null) return NotFound();

                BasketItem existed = await _dbContext.BasketItems.Where(b => b.UserId == user.Id && b.ProductId == product.Id).FirstOrDefaultAsync();


                if(existed is null)
                {
                    existed = new BasketItem
                    {
                        Product = product,
                        User = user,
                        Price = product.Price,
                        Quantity = 1

                    };
                    _dbContext.BasketItems.Add(existed);
                }
                else
                {
                    existed.Quantity++;
                }
                await _dbContext.SaveChangesAsync();
            }
            else
            {
               

                string basketStr = HttpContext.Request.Cookies["Basket"];



                var productPrice = product.Price - product.DiscountPrice;




                BasketViewModel basket;

                if (string.IsNullOrEmpty(basketStr))
                {
                    basket = new BasketViewModel();
                    BasketCookieItemViewModel cookieItem = new BasketCookieItemViewModel
                    {
                        Id = product.Id,
                        Quantity = 1

                    };
                    basket.BasketCookieItemViewModels = new List<BasketCookieItemViewModel>();
                    basket.BasketCookieItemViewModels.Add(cookieItem);
                    if (product.DiscountPrice > 0)
                    {

                        basket.TotalPrice = (decimal)productPrice;
                    }
                    else
                    {
                        basket.TotalPrice = product.Price;
                    }
                }
                else
                {
                    basket = JsonConvert.DeserializeObject<BasketViewModel>(basketStr);
                    BasketCookieItemViewModel existed = basket.BasketCookieItemViewModels.Find(p => p.Id == id);

                    if (existed == null)
                    {
                        BasketCookieItemViewModel cookieItem = new BasketCookieItemViewModel
                        {
                            Id = product.Id,
                            Quantity = 1

                        };
                        basket.BasketCookieItemViewModels.Add(cookieItem);
                        if (product.DiscountPrice > 0)
                        {

                            basket.TotalPrice += (decimal)productPrice;
                        }
                        else
                        {
                            basket.TotalPrice += product.Price;

                        }
                    }
                    else
                    {

                        if (product.DiscountPrice > 0)
                        {

                            basket.TotalPrice += (decimal)productPrice;
                        }
                        else
                        {
                            basket.TotalPrice += product.Price;

                        }
                        existed.Quantity++;
                    }

                }





                basketStr = JsonConvert.SerializeObject(basket);

                HttpContext.Response.Cookies.Append("Basket", basketStr);

            }



            return RedirectToAction("Index","Home");

        }
        
        public IActionResult ShowBasket()
        {
            if (HttpContext.Request.Cookies["Basket"] is null) NotFound();
            BasketViewModel basket = JsonConvert.DeserializeObject<BasketViewModel>(HttpContext.Request.Cookies["Basket"]);
            return Json(basket);

        }



    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class BasketController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public BasketController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var basketitems = Request.Cookies["basket"];
            return  Ok(JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketitems));
        }

        public async Task<IActionResult> AddToBasket(int? productId)
        {
            if (productId != null) NotFound();

            var product = await _dbContext.Products.Where(p => p.Id == productId)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync();

            if (product != null) NotFound();

            

            var basket = Request.Cookies["basket"];
            var basketItems = new List<BasketItemViewModel>();

            var basketItem = new BasketItemViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ProductImages = product.ProductImages,
                Count = 1
            };


            if (basket is null)
            {
             
                basketItems.Add(basketItem);

            }
            else
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basket);
                var existProduct = basketItems.Where(b => b.Id == product.Id).FirstOrDefault();
                if (existProduct is null)
                {
                    basketItems.Add(basketItem);
                }
                else
                {
                    existProduct.Count += 1;
                }

            }

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItems));

            return Ok();
        }
    }
}

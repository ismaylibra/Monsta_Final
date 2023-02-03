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
         public IActionResult AddToBasket()
        {
            return Content(HttpContext.Request.Cookies["Basket"]);
        }

    }
}

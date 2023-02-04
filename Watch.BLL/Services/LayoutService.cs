using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.BasketViewModels;

namespace Watch.BLL.Services
{
    public class LayoutService
    {
        private readonly WatchDbContext _dbContext;
        private readonly IHttpContextAccessor _http;

        public LayoutService(WatchDbContext dbContext, IHttpContextAccessor http)
        {
            _dbContext = dbContext;
            _http = http;
        }

        public List<Setting> GetSettings()
        {
            List<Setting> settings = _dbContext.Settings.ToList();
            return settings;
        }

        public LayoutBasketViewModel GetBasket()
        {

            string basketStr = _http.HttpContext.Request.Cookies["Basket"];

            if (!string.IsNullOrEmpty(basketStr))
            {
                BasketViewModel basket = JsonConvert.DeserializeObject<BasketViewModel>(basketStr);
                LayoutBasketViewModel layoutBasket = new LayoutBasketViewModel();
                layoutBasket.BasketItemViewModels = new List<BasketItemViewModel>();

                foreach (BasketCookieItemViewModel cookie in basket.BasketCookieItemViewModels)
                {
                    Product existed = _dbContext.Products.Include(p=>p.ProductImages).FirstOrDefault(p => p.Id == cookie.Id);

                    if(existed is  null)
                    {
                        basket.BasketCookieItemViewModels.Remove(cookie);
                        continue;
                    }
                    BasketItemViewModel basketItem = new BasketItemViewModel
                    {
                        Product = existed,
                        Quantity = cookie.Quantity
                    };
                    layoutBasket.BasketItemViewModels.Add(basketItem);
                  
                }
                layoutBasket.TotalPrice = basket.TotalPrice;
                return layoutBasket;
            }
            return null;
        }

    }
}

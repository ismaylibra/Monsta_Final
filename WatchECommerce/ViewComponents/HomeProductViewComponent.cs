﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.ViewComponents
{
    public class HomeProductViewComponent : ViewComponent
    {
        private readonly WatchDbContext _dbContext;

        public HomeProductViewComponent(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var product = await _dbContext.Products.Where(p => !p.IsDeleted)
                .Include(p => p.ProductImages)
                .Include(p => p.CategoryProducts).ThenInclude(p => p.Category)
                .ToListAsync();
            return View(product);
        }
    }
}
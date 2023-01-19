using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.ViewComponents
{
    public class ShopSideBarViewComponent : ViewComponent
    {
        private readonly WatchDbContext _dbContext;

        public ShopSideBarViewComponent(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories  = await _dbContext.ProductCategories.Where(c=>!c.IsDeleted).ToListAsync();
            var brands = await _dbContext.Brands.Where(c => !c.IsDeleted).ToListAsync();
            var colors = await _dbContext.Colors.Where(c => !c.IsDeleted).ToListAsync();

            var viewModel = new ShopSideBarViewModel
            {
                Categories = categories,
                Brands = brands,
                Colors=colors
            };
            return View(viewModel);
        }
    }
}

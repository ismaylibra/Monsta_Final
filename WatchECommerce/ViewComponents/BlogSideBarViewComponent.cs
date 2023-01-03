using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.ViewComponents
{
    public class BlogSideBarViewComponent : ViewComponent
    {
        private readonly WatchDbContext _dbContext;

        public BlogSideBarViewComponent(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = await _dbContext.Blogs.Where(b => !b.IsDeleted).OrderByDescending(b => b.Id).ToListAsync();
            var categories = await _dbContext.BlogCategories.Where(c => !c.IsDeleted).Include(c => c.Blogs).ToListAsync();
            var model = new BlogSideBarViewModel
            {
                Categories = categories,
                Blogs = blogs
            };
            return View(model);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Controllers
{
    public class AboutPageController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public AboutPageController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var about = await _dbContext.AboutPages.Where(a => !a.IsDeleted).ToListAsync();
            var whyUs = await _dbContext.WhyUs.Where(a => !a.IsDeleted).ToListAsync();
            var shortInfo = await _dbContext.whyUsShortInfos.Where(a => !a.IsDeleted).ToListAsync();
            var workers = await _dbContext.AboutWorkers.Where(w => !w.IsDeleted).ToListAsync();

            var model = new AboutPageViewModel
            {
                AboutPages = about,
                WhyUs = whyUs,
                WhyUsShortInfos = shortInfo,
                AboutWorkers = workers
            };
            return View(model);
        }
    }
}

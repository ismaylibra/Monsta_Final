using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.DAL.DAL;

namespace WatchECommerce.Controllers
{

    public class BlogController : Controller
    {
        private readonly WatchDbContext _dbContext;

        public BlogController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async  Task<IActionResult> Index(int page = 1)
        {
            var blogs = await _dbContext.Blogs.Where(b => !b.IsDeleted).Include(b => b.Category).OrderByDescending(b=>b.Id).ToListAsync();

            int perPage = 2;
            int pageCount = (int)Math.Ceiling((double)blogs.Count() / perPage);

            if (page <= 0) page = 1;
            if (page > pageCount) page = pageCount;

            ViewBag.CurrentPage = page;
            ViewBag.PageCount = pageCount;

            return View(blogs.Skip((page - 1) * perPage).Take(perPage).ToList());
        }
        public async Task<IActionResult> Details(int? id)

        {
            if (id is null) return NotFound();
            var blog = await _dbContext.Blogs.Where(b => !b.IsDeleted ).Include(b=>b.Category).FirstOrDefaultAsync(b=>b.Id == id);
            if (blog is null) return NotFound();


            return View(blog);
        }
        public async Task<IActionResult> BlogSidebar(int? id)
        {
            var categories = await _dbContext.BlogCategories.Where(c => c.Id == id).Include(c => c.Blogs).ToListAsync();
            return View(categories);
        }
    }
}

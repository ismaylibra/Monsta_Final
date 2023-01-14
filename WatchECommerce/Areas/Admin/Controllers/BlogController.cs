using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    
    public class BlogController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public BlogController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _dbContext.Blogs.Where(bc => !bc.IsDeleted).Include(bc => bc.Category).OrderByDescending(bc=>bc.Id).ToListAsync();

            return View(blogs);
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _dbContext.BlogCategories.Where(c => !c.IsDeleted).ToListAsync();

            var categoryListItem = new List<SelectListItem>
            {
                new SelectListItem("Select Category Of Blog" , "0")
            };

            categories.ForEach(c => categoryListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));
            var model = new BlogCreateViewModel
            {
                Categories = categoryListItem
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateViewModel model)
        {
            var categories = await _dbContext.BlogCategories.Where(c => !c.IsDeleted).Include(c => c.Blogs).ToListAsync();
            if (!ModelState.IsValid) return View(model);

            var categoryListItem = new List<SelectListItem>
             {
                 new SelectListItem("Select Category Of Blog", "0")
             };

            categories.ForEach(c => categoryListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var viewModel = new BlogCreateViewModel
            {
                Categories = categoryListItem


            };

            var createdBlog = new Blog();

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image", " An Image Must be Selected..!");
                return View(model);
            }

            if (!model.Image.IsAllowedSize(20))
            {
                ModelState.AddModelError("Image", "Image Size Can be Maximum 20mb..!");
                return View(model);
            }

            var unicalFileName = await model.Image.GenerateFile(Constants.BlogPath);

            if (model.CategoryId == 0)
            {
                ModelState.AddModelError("", "You Must Select Category");
                return View();
            }
            createdBlog.ImageUrl = unicalFileName;
            createdBlog.CategoryId = model.CategoryId;
            createdBlog.Title = model.Title;
            createdBlog.Author = model.Author;
            createdBlog.ShortContent = model.ShortContent;
            createdBlog.SpecifiedContent = model.SpecifiedContent;
            createdBlog.MainContent = model.MainContent;
            createdBlog.PublishTime = DateTime.Now;
           

            await _dbContext.Blogs.AddAsync(createdBlog);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();

            var category = await _dbContext.BlogCategories.Where(c => !c.IsDeleted).ToListAsync();
            if (category is null) return NotFound();
            var blog = await _dbContext.Blogs.Where(bc => !bc.IsDeleted && bc.Id == id).Include(bc =>bc.Category).FirstOrDefaultAsync();
            if (blog is null) return NotFound();

            if (blog.Id != id) return BadRequest();

            var selectedCategories = new List<SelectListItem>();

            category.ForEach(c => selectedCategories.Add(new SelectListItem(c.Name, c.Id.ToString())));
            var blogUpdateViewModel = new BlogUpdateViewModel
            {
                Id = blog.Id,
                Title = blog.Title,
                ImageUrl = blog.ImageUrl,
                ShortContent = blog.ShortContent,
                MainContent = blog.MainContent,
                SpecifiedContent = blog.SpecifiedContent,
                Author = blog.Author,
                Categories = selectedCategories

            };
            return View(blogUpdateViewModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, BlogUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id is null) return NotFound();
            var categories = await _dbContext.BlogCategories.Where(bc => !bc.IsDeleted).ToListAsync();
            if (categories is null) return NotFound();
            var blog = await _dbContext.Blogs.Where(b => !b.IsDeleted && b.Id == id).Include(b => b.Category).FirstOrDefaultAsync();
            if (blog is null) return NotFound();
            if (model.Image != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(new BlogUpdateViewModel
                    {

                        ImageUrl = blog.ImageUrl,

                    });
                }


                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", " An Image Must be Selected..!");
                    return View(new BlogUpdateViewModel
                    {

                        ImageUrl = blog.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image Size Can be Maximum 20mb..!");
                    return View(new BlogUpdateViewModel
                    {

                        ImageUrl = blog.ImageUrl,
                    });
                }



                var path = Path.Combine(Constants.RootPath, "img", "blog", blog.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.BlogPath);
                blog.ImageUrl = unicalFileName;
            }

            var selectedCategory = new BlogUpdateViewModel
            {
                CategoryId = model.CategoryId,


            };

            if (!ModelState.IsValid) return View(selectedCategory);





            blog.Title = model.Title;
            blog.MainContent = model.MainContent;
            blog.ShortContent = model.ShortContent;
            blog.SpecifiedContent = model.SpecifiedContent;
            blog.Author = model.Author;
            blog.CategoryId = model.CategoryId;



            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var blog = await _dbContext.Blogs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog is null) return NotFound();

            if (blog.Id != id) return BadRequest();

            var path = Path.Combine(Constants.RootPath, "assets", "img", "blog", blog.ImageUrl);

            var result = System.IO.File.Exists(path);
            if (result)
            {
                System.IO.File.Delete(path);
            }


            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

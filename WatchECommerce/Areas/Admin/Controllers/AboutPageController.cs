using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class AboutPageController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public AboutPageController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var aboutPages = await _dbContext.AboutPages.Where(a => !a.IsDeleted).ToListAsync();
            return View(aboutPages);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutPageCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image", "An image must be selected..!");
                return View(model);
            }

            if (!model.Image.IsAllowedSize(20))
            {
                ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                return View(model);
            }

            var unicalFileName = await model.Image.GenerateFile(Constants.AboutPath);

            var aboutPage = new AboutPage
            {
                ImageUrl = unicalFileName,
                Title = model.Title,
                Subtitle = model.Subtitle,
                Slogan = model.Slogan
            };
            await _dbContext.AboutPages.AddAsync(aboutPage);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var aboutPage = await _dbContext.AboutPages.FindAsync(id);
            if (aboutPage is null) return BadRequest();

            var aboutPageUpdateViewModel = new AboutPageUpdateViewModel
            {
                ImageUrl = aboutPage.ImageUrl,
                Title = aboutPage.Title,
                Subtitle = aboutPage.Subtitle,
                Slogan = aboutPage.Slogan
            };
            return View(aboutPageUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AboutPageUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var about = await _dbContext.AboutPages.FindAsync(id);
            if (about is null) return BadRequest();
            if (about.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new AboutPageUpdateViewModel
                    {

                        ImageUrl = about.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "An image must be selected..!");
                    return View(new AboutPageUpdateViewModel
                    {

                        ImageUrl = about.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(new AboutPageUpdateViewModel
                    {

                        ImageUrl = about.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "about", about.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.AboutPath);
                about.ImageUrl = unicalFileName;
            }

            about.Subtitle = model.Subtitle;
            about.Title = model.Title;
            about.Slogan = model.Slogan;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var about = await _dbContext.AboutPages.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (about is null) return BadRequest();
            if (about.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "about", about.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.AboutPages.Remove(about);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

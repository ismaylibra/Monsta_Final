using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class WhyUsController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public WhyUsController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var whyUs = await _dbContext.WhyUs.Where(wh => !wh.IsDeleted).ToListAsync();
            return View(whyUs);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WhyUsCreateViewModel model)
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

            var whyUs = new WhyUs
            {
                ImageUrl = unicalFileName,
                Title = model.Title,
                Description = model.Description
            };
            await _dbContext.WhyUs.AddAsync(whyUs);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var whyUs = await _dbContext.WhyUs.FindAsync(id);
            if (whyUs is null) return BadRequest();

            var whyUsUpdateViewModel = new WhyUsUpdateViewModel
            {
                ImageUrl = whyUs.ImageUrl,
                Title = whyUs.Title,
                Description = whyUs.Description
            };
            return View(whyUsUpdateViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(WhyUsUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var whyUs = await _dbContext.WhyUs.FindAsync(id);
            if (whyUs is null) return BadRequest();
            if (whyUs.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new WhyUsUpdateViewModel
                    {

                        ImageUrl = whyUs.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "An image must be selected..!");
                    return View(new WhyUsUpdateViewModel
                    {

                        ImageUrl = whyUs.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(new WhyUsUpdateViewModel
                    {

                        ImageUrl = whyUs.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "about", whyUs.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.AboutPath);
                whyUs.ImageUrl = unicalFileName;
            }

            whyUs.Description = model.Description;
            whyUs.Title = model.Title;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var whyUs = await _dbContext.WhyUs.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (whyUs is null) return BadRequest();
            if (whyUs.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "about", whyUs.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.WhyUs.Remove(whyUs);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class WhyUsShortInfoController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public WhyUsShortInfoController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var shortInfo = await _dbContext.whyUsShortInfos.Where(s => !s.IsDeleted).ToListAsync();
            return View(shortInfo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WhyUsShortInfoCreateViewModel model)
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

            var shortInfo = new WhyUsShortInfo
            {
                ImageUrl = unicalFileName,
                Description = model.Description,
                Subtitle = model.Subtitle
            };
            await _dbContext.whyUsShortInfos.AddAsync(shortInfo);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var shortInfo = await _dbContext.whyUsShortInfos.FindAsync(id);
            if (shortInfo is null) return BadRequest();

            var shortInfoUpdateViewModel = new WhyUsShortInfoUpdateViewModel
            {
                ImageUrl = shortInfo.ImageUrl,
                Description = shortInfo.Description,
                Subtitle = shortInfo.Subtitle

            };
            return View(shortInfoUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(WhyUsShortInfoUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var shortInfo = await _dbContext.whyUsShortInfos.FindAsync(id);
            if (shortInfo is null) return BadRequest();
            if (shortInfo.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new WhyUsShortInfoUpdateViewModel
                    {

                        ImageUrl = shortInfo.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "An image must be selected..!");
                    return View(new WhyUsShortInfoUpdateViewModel
                    {

                        ImageUrl = shortInfo.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(new WhyUsShortInfoUpdateViewModel
                    {

                        ImageUrl = shortInfo.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "about", shortInfo.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.AboutPath);
                shortInfo.ImageUrl = unicalFileName;
            }

            shortInfo.Description = model.Description;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var shortInfo = await _dbContext.whyUsShortInfos.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (shortInfo is null) return BadRequest();
            if (shortInfo.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "about", shortInfo.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.whyUsShortInfos.Remove(shortInfo);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

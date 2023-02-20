using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;
using WatchECommerce.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class AdvertisementBannerController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public AdvertisementBannerController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var addBanner = await _dbContext.AdvertisementBanners.Where(s => !s.IsDeleted).OrderByDescending(s => s.Id).ToListAsync();
            return View(addBanner);
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdvertisementBannerCreateViewModel model)
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

            var unicalFileName = await model.Image.GenerateFile(Constants.AdvertisementBannerPath);

            var adBanner = new AdvertisementBanner
            {
                ImageUrl = unicalFileName,
                TopTitle = model.TopTitle,
                MiddleTitle = model.MiddleTitle,
                BottomTitle = model.BottomTitle,
            };
            await _dbContext.AdvertisementBanners.AddAsync(adBanner);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var addBanner = await _dbContext.AdvertisementBanners.FindAsync(id);
            if (addBanner is null) return BadRequest();

            var addBannerUpdateViewModel = new AdvertisementBannerUpdateViewModel
            {
                ImageUrl = addBanner.ImageUrl,
                TopTitle = addBanner.TopTitle,
                MiddleTitle = addBanner.MiddleTitle,
                BottomTitle = addBanner.BottomTitle
            };
            return View(addBannerUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AdvertisementBannerUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var addBanner = await _dbContext.AdvertisementBanners.FindAsync(id);
            if (addBanner is null) return BadRequest();
            if (addBanner.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new AdvertisementBannerUpdateViewModel
                    {

                        ImageUrl = addBanner.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "An image must be selected..!");
                    return View(new AdvertisementBannerUpdateViewModel
                    {

                        ImageUrl = addBanner.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(new AdvertisementBannerUpdateViewModel
                    {

                        ImageUrl = addBanner.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "bg", addBanner.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.AdvertisementBannerPath);
                addBanner.ImageUrl = unicalFileName;
            }

            addBanner.TopTitle = model.TopTitle;
            addBanner.MiddleTitle = model.MiddleTitle;
            addBanner.BottomTitle = model.BottomTitle;
           
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var addBanner = await _dbContext.AdvertisementBanners.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (addBanner is null) return BadRequest();
            if (addBanner.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "bg", addBanner.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.AdvertisementBanners.Remove(addBanner);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

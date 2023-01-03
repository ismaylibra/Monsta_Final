using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
   
    public class BannerController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public BannerController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var banners = await _dbContext.Banners.Where(b => !b.IsDeleted).OrderByDescending(b => b.Id).ToListAsync();
            return View(banners);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Image.IsImage())
            {
                ModelState.AddModelError("Image", "Şəkil seçilməlidir..!");
                return View(model);
            }

            if (!model.Image.IsAllowedSize(20))
            {
                ModelState.AddModelError("Image", "Şəklin ölçüsü maksimum 20mb ola bilər..!");
                return View(model);
            }

            var unicalFileName = await model.Image.GenerateFile(Constants.BannerPath);

            var banner = new Banner
            {
                ImageUrl = unicalFileName,
                Title = model.Title,
                Subtitle = model.Subtitle,
                SellingContent = model.SellingContent
            };
            await _dbContext.Banners.AddAsync(banner);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();

            var banner = await _dbContext.Banners.FindAsync(id);

            if (banner is null) return BadRequest();

            if (banner.Id != id) return BadRequest();

            var bannerUpdateViewModel = new BannerUpdateViewModel
            {
                ImageUrl = banner.ImageUrl,
                Title = banner.Title,
                Subtitle = banner.Subtitle,
                SellingContent = banner.SellingContent
            };
            return View(bannerUpdateViewModel);



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BannerUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();

            var banner = await _dbContext.Banners.FindAsync(id);

            if (banner is null) return BadRequest();

            if (banner.Id != id) return BadRequest();

            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new BannerUpdateViewModel
                    {

                        ImageUrl = banner.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "Sekil secilmelidir");
                    return View(new BannerUpdateViewModel
                    {

                        ImageUrl = banner.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Sekilin hecmi max 20mb ola biler");
                    return View(new BannerUpdateViewModel
                    {

                        ImageUrl = banner.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "bg", banner.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.BannerPath);
                banner.ImageUrl = unicalFileName;
            }

            banner.Subtitle = model.Subtitle;
            banner.Title = model.Title;
            banner.SellingContent = model.SellingContent;
         
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var banner = await _dbContext.Banners.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (banner is null) return BadRequest();
            if (banner.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "bg", banner.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.Banners.Remove(banner);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

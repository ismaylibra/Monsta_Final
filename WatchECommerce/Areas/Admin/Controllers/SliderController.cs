using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class SliderController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public SliderController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var slider = await _dbContext.Sliders.Where(s => !s.IsDeleted).OrderByDescending(s=>s.Id).ToListAsync();
            return View(slider);
        }

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderCreateViewModel model)
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

            var unicalFileName = await model.Image.GenerateFile(Constants.SliderPath);

            var slider = new Slider
            {
                ImageUrl = unicalFileName,
                Title = model.Title,
                Subtitle = model.Subtitle,
                DiscountContent = model.DiscountContent,
                StartPrice = model.StartPrice
            };
            await _dbContext.Sliders.AddAsync(slider);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var slider = await _dbContext.Sliders.FindAsync(id);
            if (slider is null) return BadRequest();

            var sliderUpdateViewModel = new SliderUpdateViewModel
            {
                ImageUrl = slider.ImageUrl,
                Title = slider.Title,
                Subtitle = slider.Subtitle,
                DiscountContent = slider.DiscountContent,
                StartPrice = slider.StartPrice
            };
            return View(sliderUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SliderUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var slider = await _dbContext.Sliders.FindAsync(id);
            if (slider is null) return BadRequest();
            if (slider.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new SliderUpdateViewModel
                    {

                        ImageUrl = slider.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "Sekil secilmelidir");
                    return View(new SliderUpdateViewModel
                    {

                        ImageUrl = slider.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Sekilin hecmi max 20mb ola biler");
                    return View(new SliderUpdateViewModel
                    {

                        ImageUrl = slider.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "slider", slider.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.SliderPath);
                slider.ImageUrl = unicalFileName;
            }

            slider.Subtitle = model.Subtitle;
            slider.Title = model.Title;
            slider.StartPrice = model.StartPrice;
            slider.DiscountContent = model.DiscountContent;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var slider = await _dbContext.Sliders.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (slider is null) return BadRequest();
            if (slider.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "slider", slider.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.Sliders.Remove(slider);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

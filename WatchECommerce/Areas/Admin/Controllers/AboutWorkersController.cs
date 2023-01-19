using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class AboutWorkersController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public AboutWorkersController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var workers = await _dbContext.AboutWorkers.Where(w => !w.IsDeleted).ToListAsync();
            return View(workers);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutWorkersCreateViewModel model)
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

            var aboutWorker = new AboutWorkers
            {
                ImageUrl = unicalFileName,
                FullName = model.FullName,
                Description = model.Description,
                Profession= model.Profession
            };
            await _dbContext.AboutWorkers.AddAsync(aboutWorker);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var worker = await _dbContext.AboutWorkers.FindAsync(id);
            if (worker is null) return BadRequest();

            var workerUpdateViewModel = new AboutWorkersUpdateViewModel
            {
                ImageUrl = worker.ImageUrl,
                Description = worker.Description,
                FullName = worker.FullName,
                Profession = worker.Profession
            };
            return View(workerUpdateViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AboutWorkersUpdateViewModel model, int? id)
        {
            if (id is null) return NotFound();
            var worker = await _dbContext.AboutWorkers.FindAsync(id);
            if (worker is null) return BadRequest();
            if (worker.Id != id) return BadRequest();
            if (model.Image != null)
            {

                if (!ModelState.IsValid)
                {
                    return View(new AboutWorkersUpdateViewModel
                    {

                        ImageUrl = worker.ImageUrl,

                    });
                }

                if (!model.Image.IsImage())
                {
                    ModelState.AddModelError("Image", "An image must be selected..!");
                    return View(new AboutWorkersUpdateViewModel
                    {

                        ImageUrl = worker.ImageUrl,

                    });
                }

                if (!model.Image.IsAllowedSize(50))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(new AboutWorkersUpdateViewModel
                    {

                        ImageUrl = worker.ImageUrl,

                    });
                }


                var path = Path.Combine(Constants.RootPath, "img", "about", worker.ImageUrl);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var unicalFileName = await model.Image.GenerateFile(Constants.AboutPath);
                worker.ImageUrl = unicalFileName;
            }

            worker.FullName = model.FullName;
            worker.Profession = model.Profession;
            worker.Description = model.Description;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();
            var worker = await _dbContext.AboutWorkers.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (worker is null) return BadRequest();
            if (worker.Id != id) return BadRequest();
            var path = Path.Combine(Constants.RootPath, "img", "about", worker.ImageUrl);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _dbContext.AboutWorkers.Remove(worker);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public ProductCategoryController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.ProductCategories.Where(c => !c.IsDeleted).OrderByDescending(bc => bc.Id).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existCategory = await _dbContext.ProductCategories.Where(c => !c.IsDeleted).ToListAsync();

            if (existCategory.Any(c => c.Name.ToLower().Equals(model.Name.ToLower())))
            {
                ModelState.AddModelError("Name", "There is a category with this name..! ");
                return View();
            }
            var newCategory = new ProductCategory
            {
                Name = model.Name
            };

            await _dbContext.ProductCategories.AddAsync(newCategory);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var category = await _dbContext.ProductCategories.FindAsync(id);
            if (category.Id != id) return BadRequest();

            var existCategory = new ProductCategoryUpdateViewModel
            {
                Name = category.Name
            };
            return View(existCategory);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductCategoryUpdateViewModel model)
        {

            if (id is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var category = await _dbContext.ProductCategories.FindAsync(id);

            if (category is null) return NotFound();
            var isExistName = await _dbContext.ProductCategories.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != id);

            if (isExistName)
            {
                ModelState.AddModelError("Name", "There is a category with this name..!");
                return View(model);
            }
            category.Name = model.Name;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var category = await _dbContext.ProductCategories.FindAsync(id);

            if (category is null) return NotFound();

            _dbContext.ProductCategories.Remove(category);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

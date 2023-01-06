﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{
    public class ProductBrandController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public ProductBrandController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var brands = await _dbContext.ProductBrands.Where(c => !c.IsDeleted).OrderByDescending(bc => bc.Id).ToListAsync();
            return View(brands);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductBrandCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existBrand = await _dbContext.ProductBrands.Where(c => !c.IsDeleted).ToListAsync();

            if (existBrand.Any(c => c.Name.ToLower().Equals(model.Name.ToLower())))
            {
                ModelState.AddModelError("Name", "There is a category with this name..! ");
                return View();
            }
            var newBrand = new ProductBrand
            {
                Name = model.Name
            };

            await _dbContext.ProductBrands.AddAsync(newBrand);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return NotFound();
            var brand = await _dbContext.ProductBrands.FindAsync(id);
            if (brand.Id != id) return BadRequest();

            var existBrand = new ProductBrandUpdateViewModel
            {
                Name = brand.Name
            };
            return View(existBrand);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductBrandUpdateViewModel model)
        {

            if (id is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var brand = await _dbContext.ProductBrands.FindAsync(id);

            if (brand is null) return NotFound();
            var isExistName = await _dbContext.ProductBrands.AnyAsync(c => c.Name.ToLower() == model.Name.ToLower() && c.Id != id);

            if (isExistName)
            {
                ModelState.AddModelError("Name", "There is a brand with this name..!");
                return View(model);
            }
            brand.Name = model.Name;

            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var brand = await _dbContext.ProductBrands.FindAsync(id);

            if (brand is null) return NotFound();

            _dbContext.ProductBrands.Remove(brand);

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}

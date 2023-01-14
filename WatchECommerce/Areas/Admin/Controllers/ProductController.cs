using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Watch.BLL.Data;
using Watch.BLL.Extensions;
using Watch.Core.Entities;
using Watch.DAL.DAL;
using WatchECommerce.Areas.Admin.ViewModels;

namespace WatchECommerce.Areas.Admin.Controllers
{

    public class ProductController : BaseController
    {
        private readonly WatchDbContext _dbContext;

        public ProductController(WatchDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _dbContext.Products.Where(p => !p.IsDeleted).Include(x=>x.ProductImages).OrderByDescending(p=>p.Id).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var brands = await _dbContext.Brands.Where(c => !c.IsDeleted).ToListAsync();
            var colors = await _dbContext.Colors.Where(c => !c.IsDeleted).ToListAsync();
            var categories = await _dbContext.ProductCategories.Where(c => !c.IsDeleted).ToListAsync();

            var brandListItem = new List<SelectListItem>
            {
                new SelectListItem("Select Brand Of Product" , "0",false)
            };
            var categoryListItem = new List<SelectListItem>
            {
                new SelectListItem("Select Category Of Product" , "0",false)
            };

            var colorListItem = new List<SelectListItem>
            {
                new SelectListItem("Select Color Of Product" , "0", false)
            };

            brands.ForEach(c => brandListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));
            colors.ForEach(c => colorListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));
            categories.ForEach(c => categoryListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var model = new ProductCreateViewModel
            {
                Brands = brandListItem,
                Colors = colorListItem,
                Categories = categoryListItem
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if(!ModelState.IsValid) return View(model);
            var createdProduct = new Product
            {

                Name = model.Name,
                Price=model.Price,
                ShortDescription = model.ShortDescription,
                MainDescription = model.MainDescription,
                ProductImages = new List<ProductImage>(),
                BrandId =model.BrandId

                
            };
            var productImage = new List<ProductImage>();
            foreach (var item in model.Images)
            {
                if (!item.IsImage())
                {
                    ModelState.AddModelError("Image", "Image must be selected..!");
                    return View(model);
                }

                if (!item.IsAllowedSize(20))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(model);
                }
                var unicalFileName = await item.GenerateFile(Constants.ProductImagePath);
                productImage.Add(new ProductImage
                {
                    Name = unicalFileName,
                    ProductId = createdProduct.Id
                });
            }

            createdProduct.ProductImages.AddRange(productImage);
           
         
            List<ProductColor> productColors = new List<ProductColor>();

            foreach (var colorId in model.ColorIds)
            {
                if (!await _dbContext.Colors.AnyAsync(c => c.Id == colorId))
                {
                    ModelState.AddModelError("", "There was no such color..!");
                }
                productColors.Add(new ProductColor
                {
                    ColorId = colorId
                });
            }
            var colors = await _dbContext.Colors.Where(s => !s.IsDeleted).ToListAsync();
            var colorSelectListItem = new List<SelectListItem>();
            colors.ForEach(c => colorSelectListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));
            createdProduct.ProductColors = productColors;
            model.Colors = colorSelectListItem;
            




            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var categoryId in model.CategoryIds)
            {
                if (!await _dbContext.ProductCategories.AnyAsync(c => c.Id == categoryId))
                {
                    ModelState.AddModelError("", "There was no such category..!");
                }
                categoryProducts.Add(new CategoryProduct
                {
                    CategoryId = categoryId
                });
            }

            var categories = await _dbContext.ProductCategories.Where(s => !s.IsDeleted).ToListAsync();
            var categoriesSelectListItem = new List<SelectListItem>();
            categories.ForEach(c => categoriesSelectListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));
            createdProduct.CategoryProducts = categoryProducts;
            model.Categories = categoriesSelectListItem;


            var brands = await _dbContext.Brands.Where(c => !c.IsDeleted).Include(c => c.Products).ToListAsync();
            if (!ModelState.IsValid) return View(model);

            var brandListItem = new List<SelectListItem>
             {
                 new SelectListItem("Select Brand Of Product", "0")
             };

            brands.ForEach(c => brandListItem.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var viewModel = new ProductCreateViewModel
            {
                Brands = brandListItem
               


            };
            await _dbContext.Products.AddAsync(createdProduct);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null) return BadRequest();
            var product = await _dbContext.Products
                .Where(p=>p.Id==id)
                .Include(p => p.ProductImages)
                .Include(p=>p.CategoryProducts)
                .ThenInclude(c=>c.Category)
                .Include(p => p.ProductColors)
                .ThenInclude(c=>c.Color)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync();
            if (product is null) return NotFound();
            if (product.Id != id) return BadRequest();
            var categories = await _dbContext.ProductCategories 
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            if (categories is null) return NotFound();
   
            var brands = await _dbContext.Brands
                .Where(b=>!b.IsDeleted)
                .ToListAsync();
            if (brands is null) return NotFound();
            var colors = await _dbContext.Colors
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            if (colors is null) return NotFound();

            var selectedCategories = new List<SelectListItem>();

            categories.ForEach(c => selectedCategories.Add(new SelectListItem(c.Name, c.Id.ToString())));   

            var selectedBrands = new List<SelectListItem>();

            brands.ForEach(c => selectedBrands.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var selectedColors = new List<SelectListItem>();

            colors.ForEach(c => selectedColors.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var productUpdateViewModel = new ProductUpdateViewModel
            {
                Name = product.Name,
                MainDescription = product.MainDescription,
                ShortDescription = product.ShortDescription,
                Price = product.Price,
                Brands = selectedBrands,
                Categories = selectedCategories,
                Colors=selectedColors,
                ProductImages = product.ProductImages

            };

            return View(productUpdateViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Update(int? id, ProductUpdateViewModel model)
        {

            if (id is null) return BadRequest();

            var product = await _dbContext.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductImages)
                .Include(p => p.CategoryProducts).ThenInclude(p=>p.Category)
                .Include(p => p.ProductColors).ThenInclude(p=>p.Color)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync();

            if (product is null) return NotFound();

            var categories = await _dbContext.ProductCategories
             .Where(c => !c.IsDeleted)
             .ToListAsync();
            if (categories is null) return NotFound();

            var brands = await _dbContext.Brands
                .Where(b => !b.IsDeleted)
                .ToListAsync();
            if (brands is null) return NotFound();
            var colors = await _dbContext.Colors
                .Where(c => !c.IsDeleted)
                .ToListAsync();
            if (colors is null) return NotFound();

            var selectedCategories = new List<SelectListItem>();

            categories.ForEach(c => selectedCategories.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var selectedBrands = new List<SelectListItem>();

            brands.ForEach(c => selectedBrands.Add(new SelectListItem(c.Name, c.Id.ToString())));

            var selectedColors = new List<SelectListItem>();

            colors.ForEach(c => selectedColors.Add(new SelectListItem(c.Name, c.Id.ToString())));

            model.Brands = selectedBrands;
            model.Colors = selectedColors;
            model.Categories = selectedCategories;
            model.ProductImages = product.ProductImages;

            if (!ModelState.IsValid) return View(model);


            var productImage = new List<ProductImage>();
            if(model.Images is not null)
            {

            foreach (var item in model.Images)
            {
                if (!item.IsImage())
                {
                    ModelState.AddModelError("Image", "Image must be selected..!");
                    return View(model);
                }

                if (!item.IsAllowedSize(20))
                {
                    ModelState.AddModelError("Image", "Image size can be maximum 20mb..!");
                    return View(model);
                }
                var unicalFileName = await item.GenerateFile(Constants.ProductImagePath);
                productImage.Add( new ProductImage
                {
                    Name = unicalFileName,
                    ProductId = product.Id
                });
            }
            }


            product.ProductImages.AddRange(productImage);

            if (model.RemovedImageIds is not null)
            {
                RemoveImageIds(model.RemovedImageIds);

            }

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();
            if (model.CategoryIds.Count > 0)
            {
                foreach (int categoryId in model.CategoryIds)
                {
                    if (!await _dbContext.ProductCategories.AnyAsync(c => c.Id == categoryId))
                    {
                        ModelState.AddModelError("", "You chose the wrong category..!");
                        return View(model);
                    }
                    categoryProducts.Add(new CategoryProduct
                    {
                        CategoryId = categoryId

                    });
                }
                product.CategoryProducts = categoryProducts;

            }
            else
            {
                ModelState.AddModelError("", "At least 1 category must be elected..!");
                return View(model);
            }

            List<ProductColor> productColors = new List<ProductColor>();
            if (model.ColorIds.Count > 0)
            {
                foreach (int colorId in model.ColorIds)
                {
                    if (!await _dbContext.Colors.AnyAsync(c => c.Id == colorId))
                    {
                        ModelState.AddModelError("", "You chose the wrong color..!");
                        return View(model);
                    }
                    productColors.Add(new ProductColor
                    {
                        ColorId = colorId
                    });
                }
                product.ProductColors = productColors;

            }
            else
            {
                ModelState.AddModelError("", "At least 1 color must be elected..!");
                return View(model);
            }


          

           
            product.Name = model.Name;
            product.ShortDescription = model.ShortDescription;
            product.MainDescription = model.MainDescription;
            product.Price = model.Price;
            product.BrandId = model.BrandId;
            
            
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        private async void RemoveImageIds(string imageIds)
        {

            var removedImageIds = imageIds
                .Split(",")
                .ToList()
                .Select(imageId => Int32.Parse(imageId));

            var productImages = await _dbContext.ProductImages.Where(prIm => removedImageIds.Contains(prIm.Id)).ToListAsync();

            _dbContext.ProductImages.RemoveRange(productImages);

            foreach (var productImage in productImages)
            {
                var imagePath = Path.Combine(Constants.ProductImagePath, productImage.Name);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }


    }
}

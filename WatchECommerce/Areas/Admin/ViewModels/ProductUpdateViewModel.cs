using Microsoft.AspNetCore.Mvc.Rendering;
using Watch.Core.Entities;

namespace WatchECommerce.Areas.Admin.ViewModels
{
	public class ProductUpdateViewModel
	{
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string MainDescription { get; set; }
        public List<SelectListItem>? Brands { get; set; }
        public int BrandId { get; set; }
        public string? RemovedImageIds { get; set; }
        public IFormFile[] Images { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<SelectListItem>? Colors { get; set; }
        public List<int> ColorIds { get; set; }
    }
}

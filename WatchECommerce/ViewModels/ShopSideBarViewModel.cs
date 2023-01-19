using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class ShopSideBarViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Color> Colors { get; set; } = new List<Color>();
    }
}

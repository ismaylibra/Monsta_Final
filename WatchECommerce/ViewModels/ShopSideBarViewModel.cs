using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class ShopSideBarViewModel
    {
        public List<Product> Products { get; set; }
        public List<ProductCategory> Categories { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Color> Colors { get; set; }
    }
}

using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class ShopSideBarBrandViewModel
    {
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

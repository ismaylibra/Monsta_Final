using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class ShopSideBarCategoryViewModel
    {
        public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

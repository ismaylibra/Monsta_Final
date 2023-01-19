using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class ShopSideBarColorViewModel
    {
        public List<Color> Colors { get; set; } = new List<Color>();
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

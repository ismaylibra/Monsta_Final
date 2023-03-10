using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Banner> Banners { get; set; }
        public List<Blog> Blogs { get; set; }
        public List<Product> Products { get; set; }
        public List<AdvertisementBanner> AdvertisementBanners { get; set; }
    }
}

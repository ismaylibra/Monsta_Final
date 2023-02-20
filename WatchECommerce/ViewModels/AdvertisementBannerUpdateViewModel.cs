namespace WatchECommerce.ViewModels
{
    public class AdvertisementBannerUpdateViewModel
    {
        public string? TopTitle { get; set; }
        public string? MiddleTitle { get; set; }
        public string? BottomTitle { get; set; }
        public string?  ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}

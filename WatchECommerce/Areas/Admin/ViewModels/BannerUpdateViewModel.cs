namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class BannerUpdateViewModel
    {
        public string? ImageUrl { get; set; }
        public IFormFile Image { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string SellingContent { get; set; }
    }
}

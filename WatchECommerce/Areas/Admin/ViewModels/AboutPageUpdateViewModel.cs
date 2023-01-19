namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class AboutPageUpdateViewModel
    {
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Slogan { get; set; }
    }
}

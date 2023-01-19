namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class AboutWorkersUpdateViewModel
    {
        public string Description { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
    }
}

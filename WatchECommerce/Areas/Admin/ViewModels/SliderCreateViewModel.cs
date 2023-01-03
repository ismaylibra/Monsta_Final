namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class SliderCreateViewModel
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string DiscountContent { get; set; }
        public IFormFile Image { get; set; }
        public string? ImageUrl { get; set; }
        public decimal StartPrice { get; set; }

    }
}

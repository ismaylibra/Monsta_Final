using Microsoft.AspNetCore.Mvc.Rendering;

namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class BlogUpdateViewModel
    {
        public int Id { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishTime { get; set; }
        public string ShortContent { get; set; }
        public string SpecifiedContent { get; set; }
        public string MainContent { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int CategoryId { get; set; }
    }
}

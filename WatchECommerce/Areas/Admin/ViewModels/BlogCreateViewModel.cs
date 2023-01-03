using Microsoft.AspNetCore.Mvc.Rendering;
using Watch.Core.Entities;

namespace WatchECommerce.Areas.Admin.ViewModels
{
    public class BlogCreateViewModel
    {
        public IFormFile Image { get; set; }
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

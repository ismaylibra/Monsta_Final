using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class BasketProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
       public string MainImageUrl { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}

using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
      
        public int Count { get; set; }

    }
}

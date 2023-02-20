using Watch.Core.Entities;

namespace WatchECommerce.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public int Count { get; set; }
        public List<OrderItem> Items { get; set; }
        public string ImageUrl { get; set; }
    }
}

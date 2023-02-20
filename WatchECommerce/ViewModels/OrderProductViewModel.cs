namespace WatchECommerce.ViewModels
{
    public class OrderProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}

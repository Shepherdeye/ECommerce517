namespace ECommerce517.ViewModels
{
    public class ProductFilterVM
    {
        public string? ProductName { get; set; }
        public bool IsHot { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? CategoryId { get; set; }
    }
}

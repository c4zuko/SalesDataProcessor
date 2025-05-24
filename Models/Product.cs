namespace SalesDataProcessor.Models
{
    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

namespace SalesDataProcessor.Models
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public DateTime DateOfSale { get; set; }
        public string Region { get; set; }
        public string PaymentMethod { get; set; }
        public decimal ShippingCost { get; set; }

        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}

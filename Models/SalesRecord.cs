namespace SalesDataProcessor.Models
{
    public class SalesRecord
    {
        public int SalesRecordId { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string Region { get; set; }
        public DateTime DateOfSale { get; set; }
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingCost { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
    }
}

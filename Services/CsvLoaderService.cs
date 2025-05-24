using CsvHelper;
using SalesDataProcessor.Data;
using SalesDataProcessor.Models;
using System.Globalization;

namespace SalesDataProcessor.Services
{
    public class CsvLoaderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CsvLoaderService> _logger;

        public CsvLoaderService(AppDbContext context, ILogger<CsvLoaderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LoadCsvAsync(string filePath)
        {
            try
            {
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<SalesRecord>().ToList();

                foreach (var record in records)
                {
                    if (!_context.Customers.Any(c => c.CustomerId == record.CustomerId))
                    {
                        _context.Customers.Add(new Customer
                        {
                            CustomerId = record.CustomerId,
                            Name = record.CustomerName,
                            Email = record.CustomerEmail,
                            Address = record.CustomerAddress
                        });
                    }

                    if (!_context.Products.Any(p => p.ProductId == record.ProductId))
                    {
                        _context.Products.Add(new Product
                        {
                            ProductId = record.ProductId,
                            Name = record.ProductName,
                            Category = record.Category
                        });
                    }

                    if (!_context.Orders.Any(o => o.OrderId == record.OrderId))
                    {
                        _context.Orders.Add(new Order
                        {
                            OrderId = record.OrderId,
                            Region = record.Region,
                            DateOfSale = record.DateOfSale,
                            PaymentMethod = record.PaymentMethod,
                            CustomerId = record.CustomerId
                        });
                    }

                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = record.OrderId,
                        ProductId = record.ProductId,
                        QuantitySold = record.QuantitySold,
                        UnitPrice = record.UnitPrice,
                        Discount = record.Discount,
                        ShippingCost = record.ShippingCost
                    });
                }

                await _context.SaveChangesAsync();

                _context.DataRefreshLogs.Add(new DataRefreshLog
                {
                    Timestamp = DateTime.UtcNow,
                    Success = true,
                    Details = $"Loaded {records.Count} records"
                });

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load CSV");
                _context.DataRefreshLogs.Add(new DataRefreshLog
                {
                    Timestamp = DateTime.UtcNow,
                    Success = false,
                    Details = ex.Message
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}

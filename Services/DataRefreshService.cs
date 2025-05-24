using SalesDataProcessor.Data;
using SalesDataProcessor.Models;
using System.Globalization;
using System.Text;

namespace SalesDataProcessor.Services
{
    public class DataRefreshService : BackgroundService
    {
        private readonly ILogger<DataRefreshService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(10);

        public DataRefreshService(
            ILogger<DataRefreshService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataRefreshService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RefreshSalesData(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during data refresh.");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }

            _logger.LogInformation("DataRefreshService stopped.");
        }

        private async Task RefreshSalesData(CancellationToken cancellationToken)
        {
            var filePath = _configuration["SalesData:CsvPath"] ?? "sales.csv";

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"CSV file not found at: {filePath}");
                return;
            }

            var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8, cancellationToken);

            if (lines.Length <= 1) return; // Only header, no data

            var records = new List<SalesRecord>();

            foreach (var line in lines.Skip(1)) // Skip header
            {
                var parts = line.Split(',');

                if (parts.Length < 15) continue;

                try
                {
                    var record = new SalesRecord
                    {
                        OrderId = parts[0],
                        ProductId = parts[1],
                        CustomerId = parts[2],
                        ProductName = parts[3],
                        Category = parts[4],
                        Region = parts[5],
                        DateOfSale = DateTime.ParseExact(parts[6], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        QuantitySold = int.Parse(parts[7]),
                        UnitPrice = decimal.Parse(parts[8]),
                        Discount = decimal.Parse(parts[9]),
                        ShippingCost = decimal.Parse(parts[10]),
                        PaymentMethod = parts[11],
                        CustomerName = parts[12],
                        CustomerEmail = parts[13],
                        CustomerAddress = parts[14]
                    };

                    records.Add(record);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to parse line: {line}. Error: {ex.Message}");
                    continue;
                }
            }

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Optional: Clear old data
            dbContext.SalesRecords.RemoveRange(dbContext.SalesRecords);
            await dbContext.SalesRecords.AddRangeAsync(records, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Refreshed {records.Count} sales records from CSV.");
        }
    }
}

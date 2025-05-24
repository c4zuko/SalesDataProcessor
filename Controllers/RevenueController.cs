using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesDataProcessor.Data;
using SalesDataProcessor.Services;

namespace SalesDataProcessor.Controllers
{
    [Route("api/revenue")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RevenueController(AppDbContext context) => _context = context;

        [HttpGet("total")]
        public IActionResult GetTotalRevenue(DateTime start, DateTime end)
        {
            var revenue = _context.OrderItems
                .Include(o => o.Order)
                .Where(o => o.Order.DateOfSale >= start && o.Order.DateOfSale <= end)
                .Sum(o => o.QuantitySold * o.UnitPrice * (1 - o.Discount));

            return Ok(new { totalRevenue = revenue });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshData([FromServices] CsvLoaderService loader)
        {
            string path = "Data/sales.csv"; // Update path if needed
            await loader.LoadCsvAsync(path);
            return Ok("Data refreshed.");
        }

        [HttpGet("by-product")]
        public IActionResult GetRevenueByProduct(DateTime start, DateTime end)
        {
            var revenueByProduct = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .Where(o => o.Order.DateOfSale >= start && o.Order.DateOfSale <= end)
                .GroupBy(o => o.Product.Name)
                .Select(g => new
                {
                    Product = g.Key,
                    Revenue = g.Sum(o => o.QuantitySold * o.UnitPrice * (1 - o.Discount))
                })
                .ToList();

            return Ok(revenueByProduct);
        }

        [HttpGet("by-category")]
        public IActionResult GetRevenueByCategory(DateTime start, DateTime end)
        {
            var revenueByCategory = _context.OrderItems
                .Include(o => o.Order)
                .Include(o => o.Product)
                .Where(o => o.Order.DateOfSale >= start && o.Order.DateOfSale <= end)
                .GroupBy(o => o.Product.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Revenue = g.Sum(o => o.QuantitySold * o.UnitPrice * (1 - o.Discount))
                })
                .ToList();

            return Ok(revenueByCategory);
        }

        [HttpGet("by-region")]
        public IActionResult GetRevenueByRegion(DateTime start, DateTime end)
        {
            var revenueByRegion = _context.OrderItems
                .Include(o => o.Order)
                .Where(o => o.Order.DateOfSale >= start && o.Order.DateOfSale <= end)
                .GroupBy(o => o.Order.Region)
                .Select(g => new
                {
                    Region = g.Key,
                    Revenue = g.Sum(o => o.QuantitySold * o.UnitPrice * (1 - o.Discount))
                })
                .ToList();

            return Ok(revenueByRegion);
        }
    }
}

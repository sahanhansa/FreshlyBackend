using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using FreshlyBackendNew.Data;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Http;
using System.Threading;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Profile/order-count/{laundryId}
        [HttpGet("order-count/{laundryId}")]
        public async Task<IActionResult> GetOrderCount(Guid laundryId)
        {
            var allowedStatusIds = new[]
            {
                Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0"),
            };

            int count = await _context.Orders.CountAsync(o => o.LaundryId == laundryId && allowedStatusIds.Contains(o.StatusId ?? Guid.Empty));
            return Ok(new { laundryId, orderCount = count });
        }

        // GET: api/Profile/average-rating/{laundryId}
        [HttpGet("average-rating/{laundryId}")]
        public async Task<IActionResult> GetAverageRating(Guid laundryId)
        {
            var ratings = await (from f in _context.Feedbacks
                                 join o in _context.Orders on f.OrderId equals o.OrderId
                                 where o.LaundryId == laundryId && f.Rating.HasValue
                                 select f.Rating.Value).ToListAsync();

            double averageRating = ratings.Count > 0 ? ratings.Average() : 0;
            return Ok(new { laundryId, averageRating, maxRating = 5 });
        }

        // GET: api/Profile/revenue/{laundryId}
        [HttpGet("revenue/{laundryId}")]
        public async Task<IActionResult> GetRevenue(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

            // Get all order IDs for this laundry and status
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            if (orderIds.Count == 0)
                return Ok(new { laundryId, revenue = 0 });

            // Join OrderDetails to LaundryItemServices for price, using only ItemId, ServiceId, GarmentTypeId
            var revenue = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                      od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                      lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                      (od, lis) => new { od, lis })
                .SumAsync(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));

            return Ok(new { laundryId, revenue });
        }

        // GET: api/Profile/stats/daily/{laundryId}?date=yyyy-MM-dd
        [HttpGet("stats/daily/{laundryId}")]
        public async Task<IActionResult> GetDailyStats(Guid laundryId, [FromQuery] DateTime date)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId
                    && o.StatusId == statusId
                    && o.PlacedAt.HasValue
                    && o.PlacedAt.Value.Date == date.Date)
                .Select(o => o.OrderId)
                .ToListAsync();

            var orderCount = orders.Count;

            var revenue = await _context.OrderDetails
                .Where(od => orders.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .SumAsync(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));

            return Ok(new { laundryId, date = date.Date, orderCount, revenue });
        }

        // GET: api/Profile/stats/monthly/{laundryId}?year=yyyy&month=MM
        [HttpGet("stats/monthly/{laundryId}")]
        public async Task<IActionResult> GetMonthlyStats(Guid laundryId, [FromQuery] int year, [FromQuery] int month)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId
                    && o.StatusId == statusId
                    && o.PlacedAt.HasValue
                    && o.PlacedAt.Value.Year == year
                    && o.PlacedAt.Value.Month == month)
                .Select(o => o.OrderId)
                .ToListAsync();

            var orderCount = orders.Count;

            var revenue = await _context.OrderDetails
                .Where(od => orders.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .SumAsync(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));

            return Ok(new { laundryId, year, month, orderCount, revenue });
        }

        // GET: api/Profile/stats/yearly/{laundryId}?year=yyyy
        [HttpGet("stats/yearly/{laundryId}")]
        public async Task<IActionResult> GetYearlyStats(Guid laundryId, [FromQuery] int year)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId
                    && o.StatusId == statusId
                    && o.PlacedAt.HasValue
                    && o.PlacedAt.Value.Year == year)
                .Select(o => o.OrderId)
                .ToListAsync();

            var orderCount = orders.Count;

            var revenue = await _context.OrderDetails
                .Where(od => orders.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .SumAsync(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));

            return Ok(new { laundryId, year, orderCount, revenue });
        }

        // GET: api/Profile/top-garment-types/{laundryId}
        [HttpGet("top-garment-types/{laundryId}")]
        public async Task<IActionResult> GetTopGarmentTypes(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

            // Get all completed order IDs for this laundry
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            // Get garment type usage counts
            var garmentCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId != null)
                .GroupBy(od => od.GarmentTypeId)
                .Select(g => new {
                    GarmentTypeId = g.Key,
                    Count = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();

            // Get garment type names
            var garmentTypeIds = garmentCounts.Select(g => g.GarmentTypeId).ToList();
            var garmentTypeNames = await _context.GarmentTypes
                .Where(gt => garmentTypeIds.Contains(gt.GarmentTypeId))
                .ToDictionaryAsync(gt => gt.GarmentTypeId, gt => gt.GarmentTypeName);

            // Calculate total garments for percentage
            int totalGarments = garmentCounts.Sum(g => g.Count);

            var result = garmentCounts.Select(g => new {
                garmentTypeId = g.GarmentTypeId,
                garmentTypeName = garmentTypeNames.ContainsKey(g.GarmentTypeId ?? Guid.Empty) ? garmentTypeNames[g.GarmentTypeId ?? Guid.Empty] : "Unknown",
                count = g.Count,
                percentage = totalGarments > 0 ? Math.Round((double)g.Count * 100 / totalGarments, 2) : 0
            });

            return Ok(result);
        }

        // GET: api/Profile/top-services/{laundryId}
        [HttpGet("top-services/{laundryId}")]
        public async Task<IActionResult> GetTopServices(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orderIds = await _context.Orders.Where(o => o.LaundryId == laundryId && o.StatusId == statusId).Select(o => o.OrderId).ToListAsync();
            var serviceCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.ServiceId != null)
                .GroupBy(od => od.ServiceId)
                .Select(g => new { ServiceId = g.Key, Count = g.Sum(od => od.Quantity ?? 0) })
                .OrderByDescending(g => g.Count).Take(5).ToListAsync();
            var serviceIds = serviceCounts.Select(g => g.ServiceId).ToList();
            var serviceNames = await _context.Services.Where(s => serviceIds.Contains(s.ServiceId)).ToDictionaryAsync(s => s.ServiceId, s => s.ServiceName);
            int total = serviceCounts.Sum(g => g.Count);
            var result = serviceCounts.Select(g => new {
                serviceId = g.ServiceId,
                serviceName = serviceNames.ContainsKey(g.ServiceId ?? Guid.Empty) ? serviceNames[g.ServiceId ?? Guid.Empty] : "Unknown",
                count = g.Count,
                percentage = total > 0 ? Math.Round((double)g.Count * 100 / total, 2) : 0
            });
            return Ok(result);
        }

        // GET: api/Profile/order-trends/{laundryId}?year=yyyy
        [HttpGet("order-trends/{laundryId}")]
        public async Task<IActionResult> GetOrderTrends(Guid laundryId, [FromQuery] int year)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId && o.PlacedAt.HasValue && o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .ToListAsync();
            var monthly = Enumerable.Range(1, 12).Select(m => {
                var monthOrders = orders.Where(o => o.PlacedAt.Value.Month == m).Select(o => o.OrderId).ToList();
                var orderCount = monthOrders.Count;
                var revenue = orderDetails.Where(x => monthOrders.Contains(x.od.OrderId ?? Guid.Empty)).Sum(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));
                return new { month = m, orderCount, revenue };
            });
            return Ok(monthly);
        }

        // GET: api/Profile/top-customers/{laundryId}
        [HttpGet("top-customers/{laundryId}")]
        public async Task<IActionResult> GetTopCustomers(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orders = await _context.Orders.Where(o => o.LaundryId == laundryId && o.StatusId == statusId).ToListAsync();
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .ToListAsync();
            var customerGroups = orders.GroupBy(o => o.CustomerId).Select(g => new {
                customerId = g.Key,
                orderCount = g.Count(),
                revenue = orderDetails.Where(x => g.Select(o => o.OrderId).Contains(x.od.OrderId ?? Guid.Empty)).Sum(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0))
            }).OrderByDescending(g => g.orderCount).Take(5).ToList();
            var customerIds = customerGroups.Select(g => g.customerId).ToList();
            var customerNames = await _context.Customers.Where(c => customerIds.Contains(c.CustomerId)).ToDictionaryAsync(c => c.CustomerId, c => c.FirstName + " " + c.LastName);
            var result = customerGroups.Select(g => new {
                customerId = g.customerId,
                customerName = g.customerId.HasValue && customerNames.ContainsKey(g.customerId.Value) ? customerNames[g.customerId.Value] : "Unknown",
                orderCount = g.orderCount,
                revenue = g.revenue
            });
            return Ok(result);
        }

        // GET: api/Profile/customer-retention/{laundryId}?months=6
        [HttpGet("customer-retention/{laundryId}")]
        public async Task<IActionResult> GetCustomerRetention(Guid laundryId, [FromQuery] int months = 6)
        {
            var since = DateTime.UtcNow.AddMonths(-months);
            var orders = await _context.Orders.Where(o => o.LaundryId == laundryId && o.PlacedAt >= since).ToListAsync();
            var customerGroups = orders.GroupBy(o => o.CustomerId).ToList();
            int repeat = customerGroups.Count(g => g.Count() > 1);
            int single = customerGroups.Count(g => g.Count() == 1);
            int total = repeat + single;
            double repeatPct = total > 0 ? Math.Round((double)repeat * 100 / total, 2) : 0;
            double singlePct = total > 0 ? Math.Round((double)single * 100 / total, 2) : 0;
            return Ok(new { repeatCustomers = repeat, repeatPercentage = repeatPct, newCustomers = single, newPercentage = singlePct });
        }

        // GET: api/Profile/completion-rate/{laundryId}
        [HttpGet("completion-rate/{laundryId}")]
        public async Task<IActionResult> GetCompletionRate(Guid laundryId)
        {
            var completedStatusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var totalOrders = await _context.Orders.CountAsync(o => o.LaundryId == laundryId);
            var completedOrders = await _context.Orders.CountAsync(o => o.LaundryId == laundryId && o.StatusId == completedStatusId);
            double rate = totalOrders > 0 ? Math.Round((double)completedOrders * 100 / totalOrders, 2) : 0;
            return Ok(new { laundryId, completionRate = rate });
        }

        // GET: api/Profile/average-order-value/{laundryId}
        [HttpGet("average-order-value/{laundryId}")]
        public async Task<IActionResult> GetAverageOrderValue(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orders = await _context.Orders.Where(o => o.LaundryId == laundryId && o.StatusId == statusId).ToListAsync();
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od, lis })
                .ToListAsync();
            double totalRevenue = orderDetails.Sum(x => (double)(x.lis.Price ?? 0) * (x.od.Quantity ?? 0));
            int orderCount = orders.Count;
            double avg = orderCount > 0 ? Math.Round(totalRevenue / orderCount, 2) : 0;
            return Ok(new { laundryId, averageOrderValue = avg });
        }

        // GET: api/Profile/garment-type-trends/{laundryId}?year=yyyy
        [HttpGet("garment-type-trends/{laundryId}")]
        public async Task<IActionResult> GetGarmentTypeTrends(Guid laundryId, [FromQuery] int year)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId && o.PlacedAt.HasValue && o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId != null)
                .ToListAsync();
            var garmentTypeIds = orderDetails.Select(od => od.GarmentTypeId).Distinct().ToList();
            var garmentTypeNames = await _context.GarmentTypes.Where(gt => garmentTypeIds.Contains(gt.GarmentTypeId)).ToDictionaryAsync(gt => gt.GarmentTypeId, gt => gt.GarmentTypeName);
            var trends = garmentTypeIds.Select(gtId => new {
                garmentTypeId = gtId,
                garmentTypeName = garmentTypeNames.ContainsKey(gtId ?? Guid.Empty) ? garmentTypeNames[gtId ?? Guid.Empty] : "Unknown",
                monthly = Enumerable.Range(1, 12).Select(m => {
                    var monthOrderIds = orders.Where(o => o.PlacedAt.Value.Month == m).Select(o => o.OrderId).ToList();
                    int count = orderDetails.Where(od => monthOrderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId == gtId).Sum(od => od.Quantity ?? 0);
                    return new { month = m, count };
                })
            });
            return Ok(trends);
        }

        // GET: api/Profile/feedback-distribution/{laundryId}
        [HttpGet("feedback-distribution/{laundryId}")]
        public async Task<IActionResult> GetFeedbackDistribution(Guid laundryId)
        {
            var orderIds = await _context.Orders.Where(o => o.LaundryId == laundryId).Select(o => o.OrderId).ToListAsync();
            var feedbacks = await _context.Feedbacks.Where(f => orderIds.Contains(f.OrderId ?? Guid.Empty) && f.Rating.HasValue).ToListAsync();
            var distribution = Enumerable.Range(1, 5).Select(rating => new {
                rating,
                count = feedbacks.Count(f => f.Rating == rating)
            });
            return Ok(distribution);
        }

        [HttpGet("insights-report/{laundryId}")]
        public async Task<IActionResult> GetInsightsReport(Guid laundryId, [FromQuery] int year)
        {
            // 1. Fetch data from services (reuse logic from existing endpoints)
            var topGarments = await GetTopGarmentTypesData(laundryId);
            var topServices = await GetTopServicesData(laundryId);
            var orderTrends = await GetOrderTrendsData(laundryId, year);

            // 2. Generate chart images using QuickChart.io
            var garmentChartUrl = GenerateQuickChartUrl("bar", topGarments.Select(x => x.garmentTypeName), topGarments.Select(x => x.count), "Top Garment Types");
            var serviceChartUrl = GenerateQuickChartUrl("bar", topServices.Select(x => x.serviceName), topServices.Select(x => x.count), "Top Services");
            var trendsChartUrl = GenerateQuickChartUrl("line", Enumerable.Range(1, 12).Select(m => m.ToString()), orderTrends.Select(x => x.orderCount), "Order Trends");

            var garmentChartImage = await DownloadImageAsync(garmentChartUrl);
            var serviceChartImage = await DownloadImageAsync(serviceChartUrl);
            var trendsChartImage = await DownloadImageAsync(trendsChartUrl);

            // 3. Generate PDF with QuestPDF
            var pdfBytes = InsightsPdfGenerator.Generate(garmentChartImage, serviceChartImage, trendsChartImage);

            return File(pdfBytes, "application/pdf", "LaundryInsights.pdf");
        }

        // Helper: Generate QuickChart.io URL
        private string GenerateQuickChartUrl(string type, IEnumerable<string> labels, IEnumerable<int> data, string title)
        {
            var labelsJson = System.Text.Json.JsonSerializer.Serialize(labels);
            var dataJson = System.Text.Json.JsonSerializer.Serialize(data);
            var chartConfig = $@"{{type:'{type}',data:{{labels:{labelsJson},datasets:[{{label:'{title}',data:{dataJson}}}]}},options:{{plugins:{{legend:{{display:false}}}},title:{{display:true,text:'{title}'}}}}}}";
            return $"https://quickchart.io/chart?c={System.Net.WebUtility.UrlEncode(chartConfig)}&width=600&height=300";
        }

        // Helper: Download image as byte[]
        private async Task<byte[]> DownloadImageAsync(string url)
        {
            using var http = new HttpClient();
            return await http.GetByteArrayAsync(url);
        }

        // Helper: PDF generator
        public static class InsightsPdfGenerator
        {
            public static byte[] Generate(byte[] garmentChart, byte[] serviceChart, byte[] trendsChart)
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                return QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Laundry Insights Report").FontSize(20).Bold().AlignCenter();
                        page.Content().Column(col =>
                        {
                            col.Item().Text("Top Garment Types").FontSize(16).Bold();
                            col.Item().Image(garmentChart);
                            col.Item().Text("");
                            col.Item().Text("Top Services").FontSize(16).Bold();
                            col.Item().Image(serviceChart);
                            col.Item().Text("");
                            col.Item().Text("Order Trends").FontSize(16).Bold();
                            col.Item().Image(trendsChart);
                        });
                    });
                }).GeneratePdf();
            }
        }

        // Data fetchers (reuse your existing logic or call your service methods directly)
        private async Task<List<(string garmentTypeName, int count)>> GetTopGarmentTypesData(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .Select(o => o.OrderId)
                .ToListAsync();
            var garmentCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId != null)
                .GroupBy(od => od.GarmentTypeId)
                .Select(g => new {
                    GarmentTypeId = g.Key,
                    Count = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();
            var garmentTypeIds = garmentCounts.Select(g => g.GarmentTypeId).ToList();
            var garmentTypeNames = await _context.GarmentTypes
                .Where(gt => garmentTypeIds.Contains(gt.GarmentTypeId))
                .ToDictionaryAsync(gt => gt.GarmentTypeId, gt => gt.GarmentTypeName);
            return garmentCounts.Select(g => (
                garmentTypeNames.ContainsKey(g.GarmentTypeId ?? Guid.Empty) ? garmentTypeNames[g.GarmentTypeId ?? Guid.Empty] : "Unknown",
                g.Count)).ToList();
        }
        
        private async Task<List<(string serviceName, int count)>> GetTopServicesData(Guid laundryId)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .Select(o => o.OrderId)
                .ToListAsync();
            var serviceCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.ServiceId != null)
                .GroupBy(od => od.ServiceId)
                .Select(g => new {
                    ServiceId = g.Key,
                    Count = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToListAsync();
            var serviceIds = serviceCounts.Select(g => g.ServiceId).ToList();
            var serviceNames = await _context.Services
                .Where(s => serviceIds.Contains(s.ServiceId))
                .ToDictionaryAsync(s => s.ServiceId, s => s.ServiceName);
            return serviceCounts.Select(g => (
                serviceNames.ContainsKey(g.ServiceId ?? Guid.Empty) ? serviceNames[g.ServiceId ?? Guid.Empty] : "Unknown",
                g.Count)).ToList();
        }
        
        private async Task<List<(int month, int orderCount)>> GetOrderTrendsData(Guid laundryId, int year)
        {
            var statusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId && o.PlacedAt.HasValue && o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();
            var monthly = Enumerable.Range(1, 12).Select(m => (
                month: m,
                orderCount: orders.Count(o => o.PlacedAt.Value.Month == m)
            )).ToList();
            return monthly;
        }
    }
}
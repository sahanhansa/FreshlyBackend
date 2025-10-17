using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using FreshlyBackendNew.Data;
using FreshlyBackendNew.Models;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net.Http;

namespace FreshlyBackendNew.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly Guid CompletedStatusId = Guid.Parse("b8dfb7de-5f5e-11f0-8064-0022481a06a0");

        public ProfileController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Profile/order-count/{laundryId}
        [HttpGet("order-count/{laundryId}")]
        public async Task<IActionResult> GetOrderCount(Guid laundryId)
        {
            int count = await _context.Orders
                .CountAsync(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId);
            
            return Ok(new { laundryId, orderCount = count });
        }

        // GET: api/Profile/average-rating/{laundryId}
        [HttpGet("average-rating/{laundryId}")]
        public async Task<IActionResult> GetAverageRating(Guid laundryId)
        {
            var averageRating = await _context.Feedbacks
                .Where(f => f.LaundryId == laundryId && f.Rating.HasValue)
                .AverageAsync(f => (double?)f.Rating) ?? 0;

            return Ok(new { laundryId, averageRating, maxRating = 5 });
        }

        // GET: api/Profile/revenue/{laundryId}
        [HttpGet("revenue/{laundryId}")]
        public async Task<IActionResult> GetRevenue(Guid laundryId)
        {
            var revenue = await CalculateRevenueAsync(laundryId, CompletedStatusId);
            return Ok(new { laundryId, revenue });
        }

        // GET: api/Profile/stats/daily/{laundryId}?date=yyyy-MM-dd
        [HttpGet("stats/daily/{laundryId}")]
        public async Task<IActionResult> GetDailyStats(Guid laundryId, [FromQuery] DateTime date)
        {
            var (orderCount, revenue) = await GetStatsAsync(
                laundryId,
                o => o.PlacedAt.HasValue && o.PlacedAt.Value.Date == date.Date
            );

            return Ok(new { laundryId, date = date.Date, orderCount, revenue });
        }

        // GET: api/Profile/stats/monthly/{laundryId}?year=yyyy&month=MM
        [HttpGet("stats/monthly/{laundryId}")]
        public async Task<IActionResult> GetMonthlyStats(Guid laundryId, [FromQuery] int year, [FromQuery] int month)
        {
            var (orderCount, revenue) = await GetStatsAsync(
                laundryId,
                o => o.PlacedAt.HasValue && o.PlacedAt.Value.Year == year && o.PlacedAt.Value.Month == month
            );

            return Ok(new { laundryId, year, month, orderCount, revenue });
        }

        // GET: api/Profile/stats/yearly/{laundryId}?year=yyyy
        [HttpGet("stats/yearly/{laundryId}")]
        public async Task<IActionResult> GetYearlyStats(Guid laundryId, [FromQuery] int year)
        {
            var (orderCount, revenue) = await GetStatsAsync(
                laundryId,
                o => o.PlacedAt.HasValue && o.PlacedAt.Value.Year == year
            );

            return Ok(new { laundryId, year, orderCount, revenue });
        }

        // GET: api/Profile/top-garment-types/{laundryId}
        [HttpGet("top-garment-types/{laundryId}")]
        public async Task<IActionResult> GetTopGarmentTypes(Guid laundryId)
        {
            var result = await GetTopItemsAsync(
                laundryId,
                od => od.GarmentTypeId,
                _context.GarmentTypes,
                gt => gt.GarmentTypeId,
                gt => gt.GarmentTypeName,
                "garmentTypeId",
                "garmentTypeName"
            );

            return Ok(result);
        }

        // GET: api/Profile/top-services/{laundryId}
        [HttpGet("top-services/{laundryId}")]
        public async Task<IActionResult> GetTopServices(Guid laundryId)
        {
            var result = await GetTopItemsAsync(
                laundryId,
                od => od.ServiceId,
                _context.Services,
                s => s.ServiceId,
                s => s.ServiceName,
                "serviceId",
                "serviceName"
            );

            return Ok(result);
        }

        // GET: api/Profile/order-trends/{laundryId}?year=yyyy
        [HttpGet("order-trends/{laundryId}")]
        public async Task<IActionResult> GetOrderTrends(Guid laundryId, [FromQuery] int year)
        {
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && 
                           o.StatusId == CompletedStatusId && 
                           o.PlacedAt.HasValue && 
                           o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();

            var orderIds = orders.Select(o => o.OrderId).ToList();

            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => new { od.OrderId, Revenue = (double)(lis.Price ?? 0) * (od.Quantity ?? 0) })
                .ToListAsync();

            var monthly = Enumerable.Range(1, 12).Select(m =>
            {
                var monthOrderIds = orders
                    .Where(o => o.PlacedAt!.Value.Month == m)
                    .Select(o => o.OrderId)
                    .ToList();

                return new
                {
                    month = m,
                    orderCount = monthOrderIds.Count,
                    revenue = orderDetails
                        .Where(x => monthOrderIds.Contains(x.OrderId ?? Guid.Empty))
                        .Sum(x => x.Revenue)
                };
            });

            return Ok(monthly);
        }

        // GET: api/Profile/top-customers/{laundryId}
        [HttpGet("top-customers/{laundryId}")]
        public async Task<IActionResult> GetTopCustomers(Guid laundryId)
        {
            var result = await (
                from o in _context.Orders
                where o.LaundryId == laundryId && o.StatusId == CompletedStatusId
                join c in _context.Customers on o.CustomerId equals c.CustomerId
                join od in _context.OrderDetails on o.OrderId equals od.OrderId
                join lis in _context.LaundryItemServices on new { od.ItemId, od.ServiceId, od.GarmentTypeId }
                    equals new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId }
                group new { od.Quantity, lis.Price } by new { o.CustomerId, c.FirstName, c.LastName } into g
                select new
                {
                    customerId = g.Key.CustomerId,
                    customerName = $"{g.Key.FirstName} {g.Key.LastName}",
                    orderCount = g.Count(),
                    revenue = g.Sum(x => (double)(x.Price ?? 0) * (x.Quantity ?? 0))
                })
                .OrderByDescending(x => x.orderCount)
                .Take(5)
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/Profile/customer-retention/{laundryId}?months=6
        [HttpGet("customer-retention/{laundryId}")]
        public async Task<IActionResult> GetCustomerRetention(Guid laundryId, [FromQuery] int months = 6)
        {
            var since = DateTime.UtcNow.AddMonths(-months);
            
            var customerGroups = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.PlacedAt >= since)
                .GroupBy(o => o.CustomerId)
                .Select(g => new { CustomerId = g.Key, Count = g.Count() })
                .ToListAsync();

            int repeat = customerGroups.Count(g => g.Count > 1);
            int single = customerGroups.Count(g => g.Count == 1);
            int total = repeat + single;

            double repeatPct = total > 0 ? Math.Round((double)repeat * 100 / total, 2) : 0;
            double singlePct = total > 0 ? Math.Round((double)single * 100 / total, 2) : 0;

            return Ok(new 
            { 
                repeatCustomers = repeat, 
                repeatPercentage = repeatPct, 
                newCustomers = single, 
                newPercentage = singlePct 
            });
        }

        // GET: api/Profile/completion-rate/{laundryId}
        [HttpGet("completion-rate/{laundryId}")]
        public async Task<IActionResult> GetCompletionRate(Guid laundryId)
        {
            var stats = await _context.Orders
                .Where(o => o.LaundryId == laundryId)
                .GroupBy(o => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Completed = g.Count(o => o.StatusId == CompletedStatusId)
                })
                .FirstOrDefaultAsync();

            double rate = stats != null && stats.Total > 0 
                ? Math.Round((double)stats.Completed * 100 / stats.Total, 2) 
                : 0;

            return Ok(new { laundryId, completionRate = rate });
        }

        // GET: api/Profile/average-order-value/{laundryId}
        [HttpGet("average-order-value/{laundryId}")]
        public async Task<IActionResult> GetAverageOrderValue(Guid laundryId)
        {
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            if (orders.Count == 0)
                return Ok(new { laundryId, averageOrderValue = 0.0 });

            var totalRevenue = await _context.OrderDetails
                .Where(od => orders.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => (double)(lis.Price ?? 0) * (od.Quantity ?? 0))
                .SumAsync();

            double avg = Math.Round(totalRevenue / orders.Count, 2);

            return Ok(new { laundryId, averageOrderValue = avg });
        }

        // GET: api/Profile/garment-type-trends/{laundryId}?year=yyyy
        [HttpGet("garment-type-trends/{laundryId}")]
        public async Task<IActionResult> GetGarmentTypeTrends(Guid laundryId, [FromQuery] int year)
        {
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && 
                           o.StatusId == CompletedStatusId && 
                           o.PlacedAt.HasValue && 
                           o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();

            var orderIds = orders.Select(o => o.OrderId).ToList();

            var orderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId != null)
                .Select(od => new { od.OrderId, od.GarmentTypeId, od.Quantity })
                .ToListAsync();

            var garmentTypeIds = orderDetails.Select(od => od.GarmentTypeId).Distinct().ToList();
            var garmentTypeNames = await _context.GarmentTypes
                .Where(gt => garmentTypeIds.Contains(gt.GarmentTypeId))
                .ToDictionaryAsync(gt => gt.GarmentTypeId, gt => gt.GarmentTypeName);

            var trends = garmentTypeIds.Select(gtId => new
            {
                garmentTypeId = gtId,
                garmentTypeName = garmentTypeNames.ContainsKey(gtId ?? Guid.Empty) 
                    ? garmentTypeNames[gtId ?? Guid.Empty] 
                    : "Unknown",
                monthly = Enumerable.Range(1, 12).Select(m =>
                {
                    var monthOrderIds = orders
                        .Where(o => o.PlacedAt!.Value.Month == m)
                        .Select(o => o.OrderId)
                        .ToHashSet();

                    int count = orderDetails
                        .Where(od => monthOrderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId == gtId)
                        .Sum(od => od.Quantity ?? 0);

                    return new { month = m, count };
                })
            });

            return Ok(trends);
        }

        // GET: api/Profile/feedback-distribution/{laundryId}
        [HttpGet("feedback-distribution/{laundryId}")]
        public async Task<IActionResult> GetFeedbackDistribution(Guid laundryId)
        {
            var feedbacks = await _context.Feedbacks
                .Join(_context.Orders,
                    f => f.OrderId,
                    o => o.OrderId,
                    (f, o) => new { f.Rating, o.LaundryId })
                .Where(x => x.LaundryId == laundryId && x.Rating.HasValue)
                .Select(x => x.Rating!.Value)
                .ToListAsync();

            var distribution = Enumerable.Range(1, 5).Select(rating => new
            {
                rating,
                count = feedbacks.Count(f => f == rating)
            });

            return Ok(distribution);
        }

        // GET: api/Profile/insights-report/{laundryId}
        [HttpGet("insights-report/{laundryId}")]
        public async Task<IActionResult> GetInsightsReport(Guid laundryId, [FromQuery] int year)
        {
            var topGarments = await GetTopGarmentTypesData(laundryId);
            var topServices = await GetTopServicesData(laundryId);
            var orderTrends = await GetOrderTrendsData(laundryId, year);

            var garmentChartUrl = GenerateQuickChartUrl("bar", 
                topGarments.Select(x => x.garmentTypeName), 
                topGarments.Select(x => x.count), 
                "Top Garment Types");
            
            var serviceChartUrl = GenerateQuickChartUrl("bar", 
                topServices.Select(x => x.serviceName), 
                topServices.Select(x => x.count), 
                "Top Services");
            
            var trendsChartUrl = GenerateQuickChartUrl("line", 
                Enumerable.Range(1, 12).Select(m => m.ToString()), 
                orderTrends.Select(x => x.orderCount), 
                "Order Trends");

            var garmentChartImage = await DownloadImageAsync(garmentChartUrl);
            var serviceChartImage = await DownloadImageAsync(serviceChartUrl);
            var trendsChartImage = await DownloadImageAsync(trendsChartUrl);

            var pdfBytes = InsightsPdfGenerator.Generate(garmentChartImage, serviceChartImage, trendsChartImage);

            return File(pdfBytes, "application/pdf", "LaundryInsights.pdf");
        }

        // Helper methods
        private async Task<double> CalculateRevenueAsync(Guid laundryId, Guid statusId)
        {
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            if (orderIds.Count == 0)
                return 0;

            return await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => (double)(lis.Price ?? 0) * (od.Quantity ?? 0))
                .SumAsync();
        }

        private async Task<(int orderCount, double revenue)> GetStatsAsync(
            Guid laundryId,
            System.Linq.Expressions.Expression<Func<Order, bool>> dateFilter)
        {
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId)
                .Where(dateFilter)
                .Select(o => o.OrderId)
                .ToListAsync();

            var orderCount = orders.Count;
            var revenue = orderCount > 0 ? await CalculateRevenueForOrdersAsync(orders) : 0;

            return (orderCount, revenue);
        }

        private async Task<double> CalculateRevenueForOrdersAsync(List<Guid> orderIds)
        {
            return await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .Join(_context.LaundryItemServices,
                    od => new { od.ItemId, od.ServiceId, od.GarmentTypeId },
                    lis => new { lis.ItemId, lis.ServiceId, lis.GarmentTypeId },
                    (od, lis) => (double)(lis.Price ?? 0) * (od.Quantity ?? 0))
                .SumAsync();
        }

        private async Task<object> GetTopItemsAsync<TEntity, TKey>(
            Guid laundryId,
            System.Linq.Expressions.Expression<Func<OrderDetail, TKey>> keySelector,
            DbSet<TEntity> entitySet,
            Func<TEntity, TKey> entityKeySelector,
            Func<TEntity, string?> entityNameSelector,
            string idPropertyName,
            string namePropertyName) where TEntity : class
        {
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            var keySelectorCompiled = keySelector.Compile();

            var itemCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty))
                .ToListAsync();

            var grouped = itemCounts
                .Where(od => keySelectorCompiled(od) != null)
                .GroupBy(od => keySelectorCompiled(od))
                .Select(g => new
                {
                    Key = g.Key,
                    Count = g.Sum(od => od.Quantity ?? 0)
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            var keys = grouped.Select(g => g.Key).ToList();
            var entities = await entitySet.ToListAsync();
            var namesDictionary = entities
                .Where(e => keys.Contains(entityKeySelector(e)))
                .ToDictionary(e => entityKeySelector(e), e => entityNameSelector(e));

            int totalItems = grouped.Sum(g => g.Count);

            var result = grouped.Select(g =>
            {
                var dict = new Dictionary<string, object>
                {
                    [idPropertyName] = g.Key!,
                    [namePropertyName] = namesDictionary.ContainsKey(g.Key!) ? namesDictionary[g.Key!] ?? "Unknown" : "Unknown",
                    ["count"] = g.Count,
                    ["percentage"] = totalItems > 0 ? Math.Round((double)g.Count * 100 / totalItems, 2) : 0
                };
                return dict;
            });

            return result;
        }

        private string GenerateQuickChartUrl(string type, IEnumerable<string> labels, IEnumerable<int> data, string title)
        {
            var labelsJson = System.Text.Json.JsonSerializer.Serialize(labels);
            var dataJson = System.Text.Json.JsonSerializer.Serialize(data);
            var chartConfig = $@"{{type:'{type}',data:{{labels:{labelsJson},datasets:[{{label:'{title}',data:{dataJson}}}]}},options:{{plugins:{{legend:{{display:false}}}},title:{{display:true,text:'{title}'}}}}}}";
            return $"https://quickchart.io/chart?c={System.Net.WebUtility.UrlEncode(chartConfig)}&width=600&height=300";
        }

        private async Task<byte[]> DownloadImageAsync(string url)
        {
            var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.GetByteArrayAsync(url);
        }

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

        private async Task<List<(string garmentTypeName, int count)>> GetTopGarmentTypesData(Guid laundryId)
        {
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            var garmentCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.GarmentTypeId != null)
                .GroupBy(od => od.GarmentTypeId)
                .Select(g => new
                {
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
                garmentTypeNames.ContainsKey(g.GarmentTypeId ?? Guid.Empty) 
                    ? garmentTypeNames[g.GarmentTypeId ?? Guid.Empty] ?? "Unknown"
                    : "Unknown",
                g.Count)).ToList();
        }
        
        private async Task<List<(string serviceName, int count)>> GetTopServicesData(Guid laundryId)
        {
            var orderIds = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == CompletedStatusId)
                .Select(o => o.OrderId)
                .ToListAsync();

            var serviceCounts = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId ?? Guid.Empty) && od.ServiceId != null)
                .GroupBy(od => od.ServiceId)
                .Select(g => new
                {
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
                serviceNames.ContainsKey(g.ServiceId ?? Guid.Empty) 
                    ? serviceNames[g.ServiceId ?? Guid.Empty] ?? "Unknown"
                    : "Unknown",
                g.Count)).ToList();
        }
        
        private async Task<List<(int month, int orderCount)>> GetOrderTrendsData(Guid laundryId, int year)
        {
            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && 
                           o.StatusId == CompletedStatusId && 
                           o.PlacedAt.HasValue && 
                           o.PlacedAt.Value.Year == year)
                .Select(o => new { o.OrderId, o.PlacedAt })
                .ToListAsync();

            var monthly = Enumerable.Range(1, 12).Select(m => (
                month: m,
                orderCount: orders.Count(o => o.PlacedAt!.Value.Month == m)
            )).ToList();

            return monthly;
        }
    }
}
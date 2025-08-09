using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Data;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Services.Interfaces;
using System.Diagnostics;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;

namespace FreshlyBackendNew.Services.Implementations
{
    public class DriverProfileService : IDriverProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOrderDetailService _orderDetailService;
        public DriverProfileService(ApplicationDbContext context, IFileStorageService fileStorageService, IOrderDetailService orderDetailService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _orderDetailService = orderDetailService;
            _orderDetailService = orderDetailService;
        }

        public async Task<ProfileDetailsByIdDto?> GetDriverProfileDetailsAsync(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var dto = new ProfileDetailsByIdDto
            {
                DriverID = driver.DriverId,
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                LicenseNumber = driver.LicenseNo ?? "",
                Email = driver.Email ?? "",
                VehicleNumber = driver.VehicleNo ?? "",
                ProfilePhoto = driver.ProfileImage ?? "",

            };

            var contacts = await _context.Contacts
                .Where(d => d.UserId == dto.DriverID)
                .Select(d => d.ContactNumber)
                .ToListAsync();

            var address = await _context.Addresses
                .Where(c => c.AddressId == driver.AddressId)
                 .Select(d => new
                 {
                     d.HouseNo,
                     d.Street,
                     d.City,
                     d.PostalCode
                 })
                .FirstOrDefaultAsync();


            dto.ContactNumber = contacts.ToArray();
            dto.HomeAddress = $"{address.HouseNo}, {address.Street}, {address.City}";
            dto.Location = address.City;
            dto.PostalCode = address.PostalCode;

            return dto;
        }

        public async Task<DriverEditDto> GetDriverEdit(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var dto = new DriverEditDto
            {
                DriverID = driver.DriverId,
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                Email = driver.Email ?? "",
            };

            var contacts = await _context.Contacts
                .Where(d => d.UserId == dto.DriverID)
                .Select(d => d.ContactNumber)
                .FirstOrDefaultAsync();

            var address = await _context.Addresses
                .Where(c => c.AddressId == driver.AddressId)
                 .Select(d => new
                 {
                     d.HouseNo,
                     d.Street,
                     d.City,
                     d.PostalCode
                 })
                .FirstOrDefaultAsync();


            dto.ContactNumber = contacts;
            dto.HouseNo = address.HouseNo;
            dto.Street = address.Street;
            dto.City = address.City;
            dto.PostalCode = address.PostalCode;
            dto.ProfilePhoto = driver.ProfileImage;

            return dto;
        }

        public async Task<DriverHomaDto> DriverHomePage(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var orders = await _context.Orders.ToListAsync();

            var statuses = await _context.Statuses.ToListAsync();

            var statusPlaced = statuses.FirstOrDefault(s => s.StatusName == "order placed")?.StatusID
                ?? throw new InvalidOperationException("Status 'order placed' not found.");

            var statusPickedUp = statuses.FirstOrDefault(s => s.StatusName == "order picked up")?.StatusID
                ?? throw new InvalidOperationException("Status 'order picked up' not found.");

            var finishedProcessing = statuses.FirstOrDefault(s => s.StatusName == "finished processing")?.StatusID
                ?? throw new InvalidOperationException("Status 'finished processing' not found.");

            var outDelivery = statuses.FirstOrDefault(s => s.StatusName == "out for delivery")?.StatusID
                ?? throw new InvalidOperationException("Status 'out for delivery' not found.");


            var allPickups = orders.Count(o =>
                o.PickupDriverId == driverId &&
                (o.StatusId != statusPlaced && o.StatusId != statusPickedUp)
            );

            var pendingPickups = orders.Count(o =>
                o.PickupDriverId == driverId &&
                (o.StatusId == statusPlaced || o.StatusId == statusPickedUp)
            );

            var allDeliveries = orders.Count(o =>
                o.DeliveryDriverId == driverId &&
                (o.StatusId != finishedProcessing && o.StatusId != outDelivery)
            );

            var pendingDeliveries = orders.Count(o =>
                o.DeliveryDriverId == driverId &&
                (o.StatusId == finishedProcessing || o.StatusId == outDelivery)
            );

            var dto = new DriverHomaDto
            {
                FullName = $"{driver.FirstName} {driver.LastName}",
                AllPickups = allPickups,
                PendingPickups = pendingPickups,
                AllDelivery = allDeliveries,
                PendingDelivery = pendingDeliveries
            };

            return dto;
        }

        public async Task<DriverReportDto> DriverReportDash(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return null;

            var orders = await _context.Orders.ToListAsync();
            var statuses = await _context.Statuses.ToListAsync();

            var statusPlaced = statuses.FirstOrDefault(s => s.StatusName == "order placed")?.StatusID
                ?? throw new InvalidOperationException("Status 'order placed' not found.");

            var statusPickedUp = statuses.FirstOrDefault(s => s.StatusName == "order picked up")?.StatusID
                ?? throw new InvalidOperationException("Status 'order picked up' not found.");

            var finishedProcessing = statuses.FirstOrDefault(s => s.StatusName == "finished processing")?.StatusID
                ?? throw new InvalidOperationException("Status 'finished processing' not found.");

            var outDelivery = statuses.FirstOrDefault(s => s.StatusName == "out for delivery")?.StatusID
                ?? throw new InvalidOperationException("Status 'out for delivery' not found.");

            var allPickups = orders.Count(o =>
                o.PickupDriverId == driverId &&
                o.StatusId != statusPlaced && o.StatusId != statusPickedUp
            );

            var pendingPickups = orders.Count(o =>
                o.PickupDriverId == driverId &&
                (o.StatusId == statusPlaced || o.StatusId == statusPickedUp)
            );

            var allDeliveries = orders.Count(o =>
                o.DeliveryDriverId == driverId &&
                o.StatusId != finishedProcessing && o.StatusId != outDelivery
            );

            var pendingDeliveries = orders.Count(o =>
                o.DeliveryDriverId == driverId &&
                (o.StatusId == finishedProcessing || o.StatusId == outDelivery)
            );


            var mostEngagedLaundry = orders
    .GroupBy(o => o.LaundryId)
    .Select(group => new
    {
        LaundryId = group.Key,
        Count = group.Count()
    })
    .OrderByDescending(x => x.Count)
    .FirstOrDefault();
            var laundry = await _context.Laundries
        .Where(d => d.LaundryId == mostEngagedLaundry.LaundryId)
        .Select(d => new
        {
            d.LaundryId,
            d.LaundryName
        })
        .FirstOrDefaultAsync();

            var mostEngagedCustomer = orders
    .GroupBy(o => o.CustomerId)
    .Select(group => new
    {
        CustomerId = group.Key,
        Count = group.Count()
    })
    .OrderByDescending(x => x.Count)
    .FirstOrDefault();

            var customer = await _context.Customers
                .Where(c => c.CustomerId == mostEngagedCustomer.CustomerId)
                .Select(c => new
                {
                    c.CustomerId,
                    c.FirstName,
                    c.LastName
                })
                .FirstOrDefaultAsync();

            return new DriverReportDto
            {
                TotalPickups = allPickups + pendingPickups,
                TotalDelivery = allDeliveries + pendingDeliveries,
                PendingOrders = pendingDeliveries + pendingPickups,
                CompletedDelivery = allDeliveries,
                CompletedPickups = allPickups,
                MostEngagedLaundryId = laundry.LaundryId,
                MostEngagedLaundryName = laundry.LaundryName,
                MostEngagedCustomerId = customer.CustomerId,
                MostEngagedCustomerName = customer.FirstName + " " + customer.LastName

            };
        }

        public async Task<decimal> DriverReportRevenue(Guid driverId)
        {
            var driver = await _context.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DriverId == driverId);

            if (driver == null)
                return 0;

            var orders = await _context.Orders.ToListAsync();
            var statuses = await _context.Statuses.ToListAsync();

            var basket = statuses.FirstOrDefault(s => s.StatusName == "order in basket")?.StatusID
                ?? throw new InvalidOperationException("Status 'order placed' not found.");

            var statusPlaced = statuses.FirstOrDefault(s => s.StatusName == "order placed")?.StatusID
                ?? throw new InvalidOperationException("Status 'order placed' not found.");

            var statusPickedUp = statuses.FirstOrDefault(s => s.StatusName == "order picked up")?.StatusID
                ?? throw new InvalidOperationException("Status 'order picked up' not found.");

            var finishedProcessing = statuses.FirstOrDefault(s => s.StatusName == "finished processing")?.StatusID
                ?? throw new InvalidOperationException("Status 'finished processing' not found.");

            var Delivered = statuses.FirstOrDefault(s => s.StatusName == "delivered")?.StatusID
                ?? throw new InvalidOperationException("Status 'out for delivery' not found.");


            var deliveryOrderIds = orders
                .Where(o => o.DeliveryDriverId == driverId && o.PaymentMethod == "COD" && o.StatusId == Delivered)
                .Select(o => o.OrderId)
                .ToList();

            var pickupOrderIds = orders
                .Where(o => o.PickupDriverId == driverId && o.PaymentMethod == "COD" && (o.StatusId != statusPlaced && o.StatusId != statusPlaced && o.StatusId != basket))
                .Select(o => o.OrderId)
                .ToList();

            decimal totalRevenue = 0;

            foreach (var orderId in deliveryOrderIds.Concat(pickupOrderIds))
            {
                var details = await _orderDetailService.GetOrderDetailsAsync(orderId);
                totalRevenue += details.TotalAmount;
            }

            return totalRevenue;
        }

        public async Task<string> UpdateProfile(DriverEditDto dto)
        {
            if (dto.File != null)
            {
                var driver = await _context.Drivers
                    .FirstOrDefaultAsync(d => d.DriverId == dto.DriverID);
                if (driver == null)
                    throw new InvalidOperationException($"Driver not found for ID: {dto.DriverID}");

                if (!string.IsNullOrEmpty(driver.ProfileImage))
                {
                    var updatedImage = await _fileStorageService.UpdateImageAsync(driver.ProfileImage, dto.File);
                    driver.ProfileImage = updatedImage;
                }
                else
                {
                    var uploadedImage = await _fileStorageService.UploadImageAsync(dto.File);
                    driver.ProfileImage = uploadedImage;
                }

                int result = await _context.SaveChangesAsync();
                return result > 0 ? "success" : "error";
            }
            else
            {
                var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.DriverId == dto.DriverID);
                if (driver == null)
                    throw new InvalidOperationException($"Driver not found for ID: {dto.DriverID}");

                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == driver.AddressId);
                if (address == null)
                    throw new InvalidOperationException($"Address not found for AddressId: {driver.AddressId}");

                var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.UserId == dto.DriverID);
                if (contact == null)
                    throw new InvalidOperationException($"Contact not found for UserId: {dto.DriverID}");

                driver.FirstName = dto.FirstName;
                driver.LastName = dto.LastName;
                driver.Email = dto.Email;

                address.HouseNo = dto.HouseNo;
                address.Street = dto.Street;
                address.City = dto.City;
                address.PostalCode = dto.PostalCode;

                contact.ContactNumber = dto.ContactNumber;

                int result = await _context.SaveChangesAsync();
                return result > 0 ? "success" : "error";
            }
        }

        public async Task<string> UpdatePassword(UpdatePasswordDto dto)
        {
            try
            {
                var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.DriverId == dto.DriverId);

                if (driver == null || !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, driver.Password))
                {
                    return null;
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                driver.Password = hashedPassword;

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return "success";
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdatePassword EXCEPTION: {ex}");
                throw; // rethrow to bubble up to your controller
            }
        }

        public async Task<byte[]> GeneratePdfReport(Guid orderId)
        {
            try
            {
                if (orderId == Guid.Empty)
                    throw new ArgumentNullException(nameof(orderId));

                // Fetch data
                var deliveryDetails = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join add in _context.Addresses on cust.AddressId equals add.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    join note in _context.DriverNotes on ord.OrderId equals note.OrderId into noteGroup
                    from note in noteGroup.DefaultIfEmpty()
                    where ord.OrderId == orderId
                    select new DeliveryDetailsByIdDto
                    {
                        OrderId = ord.OrderId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,
                        DeliverDriver = ord.DeliveryDriverId,
                        note = note.Note,
                        PaymenthMethod = ord.PaymentMethod,
                        IsPaid = ord.IsPaid,
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId)
                            .Select(c => c.ContactNumber)
                            .ToList(),
                        OrderItems = _context.OrderDetails
                            .Where(o => o.OrderId == ord.OrderId)
                            .Join(_context.Items,
                                o => o.ItemId,
                                i => i.ItemId,
                                (o, i) => new OrderedItemsDto
                                {
                                    ItemName = i.Name,
                                    Quantity = (int)o.Quantity
                                })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (deliveryDetails == null)
                    throw new InvalidOperationException("No order found.");

                var orderDetails = await _orderDetailService.GetOrderDetailsAsync(deliveryDetails.OrderId);
                deliveryDetails.TotalAmount = orderDetails.TotalAmount;

                using var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Colors
                var primary = XColor.FromArgb(37, 99, 235);  // Blue
                var accent = XColor.FromArgb(16, 185, 129);  // Emerald
                var danger = XColor.FromArgb(239, 68, 68);   // Red
                var warn = XColor.FromArgb(245, 158, 11);    // Amber
                var dark = XColor.FromArgb(31, 41, 55);      // Dark Gray
                var light = XColor.FromArgb(249, 250, 251);  // Light Gray

                // Fonts
                var titleFont = new XFont("Segoe UI", 20, XFontStyle.Bold);
                var subtitleFont = new XFont("Segoe UI", 10, XFontStyle.Regular);
                var labelFont = new XFont("Segoe UI", 10, XFontStyle.Bold);
                var valueFont = new XFont("Segoe UI", 10, XFontStyle.Regular);
                var tableHeaderFont = new XFont("Segoe UI", 10, XFontStyle.Bold);
                var tableCellFont = new XFont("Segoe UI", 10, XFontStyle.Regular);

                double margin = 40;
                double yPos = 0;

                // --- HEADER ---
                gfx.DrawRectangle(new XSolidBrush(primary), 0, 0, page.Width, 80);
                gfx.DrawRectangle(new XSolidBrush(accent), 0, 75, page.Width, 4);

                // Add logo left
                double logoX = margin;
                double logoY = 15;
                double logoHeight = await AddLogoToPdf(gfx, page, logoX, logoY);

                // Title
                gfx.DrawString("Freshly Delivery Report", titleFont, XBrushes.White,
                    new XRect(logoX + 60, logoY, page.Width - (logoX + 60) - margin, 25), XStringFormats.TopLeft);
                gfx.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}", subtitleFont, XBrushes.White,
                    new XRect(logoX + 60, logoY + 28, page.Width - (logoX + 60) - margin, 20), XStringFormats.TopLeft);

                yPos = 100;

                // --- STATUS ---
                var statusColor = deliveryDetails.Status.ToLower() == "delivered" ? accent :
                                  deliveryDetails.Status.ToLower() == "pending" ? warn : danger;

                var statusRect = new XRect(page.Width - margin - 120, yPos, 120, 25);
                gfx.DrawRectangle(new XSolidBrush(statusColor), statusRect);
                gfx.DrawString(deliveryDetails.Status.ToUpper(), new XFont("Segoe UI", 9, XFontStyle.Bold),
                    XBrushes.White, statusRect, XStringFormats.Center);

                yPos += 40;

                double contentWidth = page.Width - 2 * margin;

                // --- INFO CARD ---
                DrawCard(gfx, margin, yPos, contentWidth, 200, light);
                yPos += 15;

                double col1X = margin + 20;
                double col2X = page.Width / 2 + 10;
                double colWidth = (contentWidth / 2) - 40;

                double leftY = yPos;
                double rightY = yPos;

                leftY = DrawField(gfx, "Order ID", deliveryDetails.OrderId.ToString(), col1X, leftY, colWidth, labelFont, valueFont, dark);
                leftY = DrawField(gfx, "Customer", deliveryDetails.CustomerName, col1X, leftY, colWidth, labelFont, valueFont, dark);
                leftY = DrawField(gfx, "Address", deliveryDetails.Address, col1X, leftY, colWidth, labelFont, valueFont, dark);
                leftY = DrawField(gfx, "Payment Method", deliveryDetails.PaymenthMethod, col1X, leftY, colWidth, labelFont, valueFont, dark);

                rightY = DrawField(gfx, "Laundry", deliveryDetails.LaundryName, col2X, rightY, colWidth, labelFont, valueFont, dark);
                rightY = DrawField(gfx, "Contact(s)", string.Join(", ", deliveryDetails.Contact), col2X, rightY, colWidth, labelFont, valueFont, dark);
                rightY = DrawField(gfx, "Driver ID", deliveryDetails.DeliverDriver?.ToString(), col2X, rightY, colWidth, labelFont, valueFont, dark);
                rightY = DrawField(gfx, "Total Amount", $"Rs. {deliveryDetails.TotalAmount:N2}", col2X, rightY, colWidth, labelFont, valueFont, dark);

                yPos = Math.Max(leftY, rightY) + 10;

                // Payment Status with circle
                var paidColor = deliveryDetails.IsPaid == true ? accent : danger;
                gfx.DrawEllipse(new XSolidBrush(paidColor), col1X, yPos, 12, 12);
                gfx.DrawString(deliveryDetails.IsPaid == true ? "Paid" : "Unpaid", valueFont, new XSolidBrush(dark),
                    new XRect(col1X + 18, yPos, colWidth, 12), XStringFormats.TopLeft);

                yPos += 35;

                // Note
                if (!string.IsNullOrWhiteSpace(deliveryDetails.note))
                {
                    DrawCard(gfx, margin, yPos, contentWidth, 60, XColor.FromArgb(254, 243, 199));
                    yPos += 10;
                    gfx.DrawString("Driver Note:", labelFont, new XSolidBrush(dark),
                        new XRect(col1X, yPos, contentWidth, 15), XStringFormats.TopLeft);
                    yPos += 15;
                    gfx.DrawString(deliveryDetails.note, valueFont, new XSolidBrush(dark),
                        new XRect(col1X, yPos, contentWidth - 40, 40), XStringFormats.TopLeft);
                    yPos += 50;
                }

                // Ordered Items Table
                if (deliveryDetails.OrderItems?.Any() == true)
                {
                    double tableWidth = contentWidth - 40;
                    double itemCol = tableWidth * 0.7;
                    double qtyCol = tableWidth * 0.3;
                    double rowH = 25;

                    gfx.DrawString("Ordered Items", labelFont, new XSolidBrush(dark),
                        new XRect(margin, yPos, contentWidth, 20), XStringFormats.TopLeft);

                    yPos += 20;

                    var thRect = new XRect(margin + 20, yPos, tableWidth, rowH);
                    gfx.DrawRectangle(new XSolidBrush(primary), thRect);
                    gfx.DrawString("Item", tableHeaderFont, XBrushes.White,
                        new XRect(margin + 30, yPos + 6, itemCol, rowH), XStringFormats.TopLeft);
                    gfx.DrawString("Qty", tableHeaderFont, XBrushes.White,
                        new XRect(margin + 30 + itemCol, yPos + 6, qtyCol, rowH), XStringFormats.TopLeft);

                    yPos += rowH;

                    bool alt = false;
                    foreach (var item in deliveryDetails.OrderItems)
                    {
                        var bg = alt ? light : XColors.White;
                        var rowRect = new XRect(margin + 20, yPos, tableWidth, rowH);
                        gfx.DrawRectangle(new XSolidBrush(bg), rowRect);
                        gfx.DrawRectangle(new XPen(XColor.FromArgb(229, 231, 235)), rowRect);

                        gfx.DrawString(item.ItemName, tableCellFont, new XSolidBrush(dark),
                            new XRect(margin + 30, yPos + 5, itemCol, rowH), XStringFormats.TopLeft);
                        gfx.DrawString(item.Quantity.ToString(), tableCellFont, new XSolidBrush(dark),
                            new XRect(margin + 30 + itemCol, yPos + 5, qtyCol, rowH), XStringFormats.TopLeft);

                        yPos += rowH;
                        alt = !alt;
                    }
                }

                using var ms = new MemoryStream();
                document.Save(ms, false);
                return ms.ToArray();
            }
            catch
            {
                throw;
            }
        }

        // --- Helper to draw card ---
        private static void DrawCard(XGraphics gfx, double x, double y, double w, double h, XColor bg)
        {
            gfx.DrawRectangle(new XSolidBrush(bg), new XRect(x, y, w, h));
            gfx.DrawRectangle(new XPen(XColor.FromArgb(229, 231, 235)), new XRect(x, y, w, h));
        }

        // --- Helper for fields ---
        private static double DrawField(XGraphics gfx, string label, string value, double x, double y, double w, XFont lf, XFont vf, XColor c)
        {
            gfx.DrawString(label, lf, new XSolidBrush(c), new XRect(x, y, w, 15), XStringFormats.TopLeft);
            gfx.DrawString(value, vf, new XSolidBrush(c), new XRect(x, y + 15, w, 15), XStringFormats.TopLeft);
            return y + 35;
        }

        // --- Logo helper ---
        private async Task<double> AddLogoToPdf(XGraphics gfx, PdfPage page, double x, double y)
        {
            try
            {
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                if (!File.Exists(logoPath)) return 0;

                var logoBytes = await File.ReadAllBytesAsync(logoPath);
                using var ms = new MemoryStream(logoBytes);
                var img = XImage.FromStream(() => ms);

                double w = 40;
                double h = img.PixelHeight * (w / img.PixelWidth);

                gfx.DrawImage(img, x, y, w, h);
                return h;
            }
            catch
            {
                return 0;
            }
        }

    }
}

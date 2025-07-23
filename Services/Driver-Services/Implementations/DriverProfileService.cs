using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Data;
using Microsoft.EntityFrameworkCore;
using FreshlyBackendNew.Services.Interfaces;
using System.Diagnostics;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;

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
                ProfilePhoto=driver.ProfileImage ?? "",
                
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
                MostEngagedLaundryId=laundry.LaundryId,
                MostEngagedLaundryName=laundry.LaundryName,
                MostEngagedCustomerId=customer.CustomerId,
                MostEngagedCustomerName=customer.FirstName+" "+customer.LastName

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
                .Where(o => o.DeliveryDriverId == driverId && o.PaymentMethod == "COD" && o.StatusId==Delivered)
                .Select(o => o.OrderId)
                .ToList();

            var pickupOrderIds = orders
                .Where(o => o.PickupDriverId == driverId && o.PaymentMethod == "COD" && (o.StatusId!=statusPlaced && o.StatusId!=statusPlaced  && o.StatusId!=basket))
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

    }
}

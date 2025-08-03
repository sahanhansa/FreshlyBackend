using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FreshlyBackendNew.Services.Implementations
{
    public class CompleteTasksService : ICompleteTasksService
    {
        private readonly ApplicationDbContext _context;

        public CompleteTasksService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all completed tasks with basic details.
        /// </summary>
        public async Task<List<CompleteTasksDetailsDto>> GetAllCompleteTasks(Guid driverId)
        {
            try
            {
                var statuses = await _context.Statuses.ToListAsync();

                var basket = statuses.FirstOrDefault(s => s.StatusName == "order in basket")?.StatusID
                    ?? throw new InvalidOperationException("Status 'order in basket' not found.");
                var statusPlaced = statuses.FirstOrDefault(s => s.StatusName == "order placed")?.StatusID
                    ?? throw new InvalidOperationException("Status 'order placed' not found.");

                var statusPickedUp = statuses.FirstOrDefault(s => s.StatusName == "order picked up")?.StatusID
                    ?? throw new InvalidOperationException("Status 'order picked up' not found.");

                var finishedProcessing = statuses.FirstOrDefault(s => s.StatusName == "finished processing")?.StatusID
                    ?? throw new InvalidOperationException("Status 'finished processing' not found.");

                var outDelivery = statuses.FirstOrDefault(s => s.StatusName == "out for delivery")?.StatusID
                    ?? throw new InvalidOperationException("Status 'out for delivery' not found.");

                var delivered = statuses.FirstOrDefault(s => s.StatusName == "delivered")?.StatusID
                    ?? throw new InvalidOperationException("Status 'delivered' not found.");

                var baseOrders = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join addr in _context.Addresses on cust.AddressId equals addr.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    select new
                    {
                        ord.OrderId,
                        cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = addr.HouseNo + " " + addr.Street + ", " + addr.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,
                        PickupDriverId = ord.PickupDriverId ?? null,
                        DeliveryDriverId = ord.DeliveryDriverId ?? ord.PickupDriverId ?? null,
                        StatusId = ord.StatusId,
                    }
                ).ToListAsync();

                if (baseOrders == null || !baseOrders.Any())
                {
                    return new List<CompleteTasksDetailsDto>(); // Return empty list, not null
                }

                var allPickupsRecords = baseOrders
                    .Where(o => o.PickupDriverId == driverId &&
                                (o.StatusId != statusPlaced && o.StatusId != statusPickedUp && o.StatusId != basket))
                    .ToList();

                var allDeliveriesRecords = baseOrders
                    .Where(o => o.DeliveryDriverId == driverId &&  o.StatusId == delivered)
                    .ToList();

                var pickupsResult = new List<CompleteTasksDetailsDto>();
                foreach (var order in allPickupsRecords)
                {
                    var contacts = await _context.Contacts
                        .Where(c => c.UserId == order.CustomerId && c.UserType == "Customer")
                        .Select(c => c.ContactNumber)
                        .ToListAsync();

                    // contacts will never be null, only empty if no records
                    pickupsResult.Add(new CompleteTasksDetailsDto
                    {
                        OrderId = order.OrderId,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        Address = order.Address,
                        LaundryName = order.LaundryName,
                        Status = order.Status,
                        Contact = contacts,
                        PickupDriverId = order.PickupDriverId


                    });
                }

                var deliveriesResult = new List<CompleteTasksDetailsDto>();
                foreach (var order in allDeliveriesRecords)
                {
                    var contacts = await _context.Contacts
                        .Where(c => c.UserId == order.CustomerId && c.UserType == "Customer")
                        .Select(c => c.ContactNumber)
                        .ToListAsync();

                    deliveriesResult.Add(new CompleteTasksDetailsDto
                    {
                        OrderId = order.OrderId,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        Address = order.Address,
                        LaundryName = order.LaundryName,
                        Status = order.Status,
                        Contact = contacts,
                        DeliveryDriverId=order.DeliveryDriverId
                        
                    });
                }

                var combinedResult = pickupsResult.Concat(deliveriesResult).ToList();

                return combinedResult;  // Return the actual combined data
            }
            catch (Exception ex)
            {
                // Optional: log exception here
                throw new Exception("An error occurred while retrieving completed tasks.", ex);
            }
        }


        /// <summary>
        /// Get detailed info for a specific completed task.
        /// </summary>
        public async Task<CompleteTasksDetailsByIdDto> GetAllCompleteTasksBYId(string orderID)
        {
            try
            {
                var order = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join addr in _context.Addresses on cust.AddressId equals addr.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where ord.OrderId.ToString() == orderID
                    select new
                    {
                        ord.OrderId,
                        cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = addr.HouseNo + " " + addr.Street + ", " + addr.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName
                    }
                ).FirstOrDefaultAsync();

                if (order == null)
                {
                    throw new Exception($"Order with ID {orderID} not found.");
                }

                var contacts = await _context.Contacts
                    .Where(c => c.UserId == order.CustomerId && c.UserType == "Customer")
                    .Select(c => c.ContactNumber)
                    .ToListAsync();

                var items = await _context.OrderDetails
                    .Where(o => o.OrderId == order.OrderId)
                    .Join(_context.Items,
                        o => o.ItemId,
                        i => i.ItemId,
                        (o, i) => new OrderedItemsDto
                        {
                            ItemName = i.Name,
                            Quantity = (int)o.Quantity
                        })
                    .ToListAsync();

                return new CompleteTasksDetailsByIdDto
                {
                    OrderId = order.OrderId,
                    CustomerName = order.CustomerName,
                    Address = order.Address,
                    LaundryName = order.LaundryName,
                    Status = order.Status,
                    Contact = contacts,
                    OrderItems = items
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving completed task details for Order ID: {orderID}", ex);
            }
        }
    }
}

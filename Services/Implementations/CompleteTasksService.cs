using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.DTOs.Driver_DTOs;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<CompleteTasksDetailsDto>> GetAllCompleteTasks()
        {
            try
            {
                // Get base order details first
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
                        Status = sta.StatusName
                    }
                ).ToListAsync();

                // Map contacts in-memory for each order
                var result = new List<CompleteTasksDetailsDto>();

                foreach (var order in baseOrders)
                {
                    var contacts = await _context.Contacts
                        .Where(c => c.UserId == order.CustomerId && c.UserType == "Customer")
                        .Select(c => c.ContactNumber)
                        .ToListAsync();

                    result.Add(new CompleteTasksDetailsDto
                    {
                        OrderId = order.OrderId,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        Address = order.Address,
                        LaundryName = order.LaundryName,
                        Status = order.Status,
                        Contact = contacts
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
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

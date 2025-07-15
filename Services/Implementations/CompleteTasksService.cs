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

        // Get all orders that are completed
        public async Task<List<CompleteTasksDetailsDto>> GetAllCompleteTasks()
        {
            try
            {
                var completedTasks = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join addr in _context.Addresses on cust.AddressId equals addr.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where sta.StatusName == "Pickup Complete" || sta.StatusName == "Delivery Complete"
                    select new CompleteTasksDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = addr.HouseNo + " " + addr.Street + ", " + addr.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId && c.UserType == "Customer")
                            .Select(c => c.ContactNumber)
                            .ToList()
                    }
                ).ToListAsync();

                return completedTasks;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving completed tasks.", ex);
            }
        }

        // Get details for a specific completed task by Order ID
        public async Task<CompleteTasksDetailsByIdDto> GetAllCompleteTasksBYId(string orderID)
        {
            try
            {
                var completedTaskDetails = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join addr in _context.Addresses on cust.AddressId equals addr.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where ord.OrderId.ToString() == orderID && (sta.StatusName == "Pickup Complete" || sta.StatusName == "Delivery Complete")
                    select new CompleteTasksDetailsByIdDto
                    {
                        OrderId = ord.OrderId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = addr.HouseNo + " " + addr.Street + ", " + addr.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId && c.UserType == "Customer")
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
                    }
                ).FirstOrDefaultAsync();

                return completedTaskDetails;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving completed task details for Order ID: {orderID}", ex);
            }
        }

      
    }
}


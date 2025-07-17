using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services
{
    public class AllDeliveryService : IAllDeliveryService
    {
        private readonly ApplicationDbContext _context;

        // Constructor with dependency injection for the database context
        public AllDeliveryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a list of all deliveries where the status is either "Delivery Complete" or "Delivery Pending"
        public async Task<List<DeliveryDetailsDto>> GetAllDeliveries()
        {
            try
            {
                var deliveries = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join add in _context.Addresses on cust.AddressId equals add.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where sta.StatusName == "Delivery Complete" || sta.StatusName == "Delivery Pending"
                    select new DeliveryDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        Status = sta.StatusName,
                        LaundryName = laun.LaundryName,

                        // Fetching all contact numbers related to the customer
                        Contact = _context.Contacts
                            .Where(c => c.ContactId == cust.CustomerId)
                            .Select(c => c.ContactNumber)
                            .ToList()
                    })
                    .ToListAsync();

                return deliveries;
            }
            catch (Exception ex)
            {
                // Optional: Log the error before throwing
                throw new Exception("An error occurred while retrieving all delivery orders.", ex);
            }
        }

        // Retrieves detailed delivery information for a specific order ID
        public async Task<DeliveryDetailsByIdDto> GetDeliveryDetailsBYId(string orderID)
        {
            try
            {
                var deliveryDetails = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join add in _context.Addresses on cust.AddressId equals add.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where ord.OrderId.ToString() == orderID &&
                          (sta.StatusName == "Delivery Complete" || sta.StatusName == "Delivery Pending")
                    select new DeliveryDetailsByIdDto
                    {
                        OrderId = ord.OrderId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,

                        // Fetching all contact numbers related to the customer
                        Contact = _context.Contacts
                            .Where(c => c.UserId== cust.CustomerId)
                            .Select(c => c.ContactNumber)
                            .ToList(),

                        // Fetching ordered items with quantity and item names
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

                return deliveryDetails;
            }
            catch (Exception ex)
            {
                // Optional: Log the error before throwing
                throw new Exception($"An error occurred while retrieving delivery details for Order ID: {orderID}", ex);
            }
        }
    }
}

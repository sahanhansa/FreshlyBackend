using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services
{
    public class AllPickupService : IAllPickupService
    {
        private readonly ApplicationDbContext _context;

        // Constructor injection of the database context
        public AllPickupService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a list of all pickups with relevant customer, laundry, address, and contact details
        public async Task<List<PickupDetailsDto>> GetAllPickups()
        {
            try
            {
                var pickups = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join add in _context.Addresses on cust.AddressId equals add.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where sta.StatusName == "Pickup Complete" || sta.StatusName == "Pickup Pending"
                    select new PickupDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        Status = sta.StatusName,
                        LaundryName = laun.LaundryName,

                        // Fetch all contact numbers associated with the customer
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId)
                            .Select(c => c.ContactNumber)
                            .ToList()
                    })
                    .ToListAsync();

                return pickups;
            }
            catch (Exception ex)
            {
                // You may log the error here or throw a custom exception
                throw new Exception("An error occurred while retrieving all orders.", ex);
            }
        }

        // Retrieves detailed pickup information for a specific order ID
        public async Task<PickupDetailsByIdDto> GetPickupDetailsBYId(string orderID)
        {
            try
            {
                var pickupDetails = await (
                    from ord in _context.Orders
                    join cust in _context.Customers on ord.CustomerId equals cust.CustomerId
                    join add in _context.Addresses on cust.AddressId equals add.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    where ord.OrderId.ToString() == orderID && (sta.StatusName == "Pickup Complete" || sta.StatusName == "Pickup Pending")
                    select new PickupDetailsByIdDto
                    {
                        OrderId = ord.OrderId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,

                        // Fetch all contact numbers associated with the customer
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId)
                            .Select(c => c.ContactNumber)
                            .ToList(),

                        // Fetch all items associated with the order
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

                return pickupDetails;
            }
            catch (Exception ex)
            {
                // You may log the error here or throw a custom exception
                throw new Exception($"An error occurred while retrieving order details for Order ID: {orderID}", ex);
            }
        }
    }
}

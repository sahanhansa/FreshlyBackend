using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
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
                    select new DeliveryDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        Status = sta.StatusName,
                        LaundryName = laun.LaundryName,
                        DeliverDriver = ord.DeliveryDriverId,

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
                    join note in _context.DriverNotes on ord.OrderId equals note.OrderId into noteGroup
                    from note in noteGroup.DefaultIfEmpty() // LEFT JOIN
                    where ord.OrderId.ToString() == orderID
                    select new DeliveryDetailsByIdDto
                    {
                        OrderId = ord.OrderId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = add.HouseNo + " " + add.Street + ", " + add.City,
                        LaundryName = laun.LaundryName,
                        Status = sta.StatusName,
                        DeliverDriver=ord.DeliveryDriverId,
                        note=note.Note,

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

        public async Task MarksToCustomerDeliver(MarkOrderDto markOrderDto)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);

                if (order == null)
                {
                    throw new Exception($"Order with ID {markOrderDto.OrderId} not found.");
                }

                var noteDetails = await _context.DriverNotes
                    .FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);

                if (noteDetails == null)
                {
                    noteDetails = new DriverNote
                    {
                        OrderId = markOrderDto.OrderId,
                        DriverId = markOrderDto.DriverId,
                        Note = markOrderDto.Note
                    };
                    _context.DriverNotes.Add(noteDetails);
                }
                else
                {
                    noteDetails.Note = markOrderDto.Note;
                }
                
                var statusDetails = await _context.Statuses
                    .FirstOrDefaultAsync(d => d.StatusName == "delivered");

                if (statusDetails == null)
                {
                    throw new Exception("Status 'delivered' not found.");
                }

                order.StatusId = statusDetails.StatusID;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the order as delivered.", ex);
            }
        }



        public async Task MarksToLaundryPick(MarkOrderDto markOrderDto)
        {
            try
            {
               

                var order = await _context.Orders
                    .FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);

                if (order == null)
                {
                    throw new Exception($"Order with ID  not found.");
                }

                var statusDetails = await _context.Statuses
                    .FirstOrDefaultAsync(d => d.StatusName == "out for delivery");

                if (statusDetails == null)
                {
                    throw new Exception("Status 'delivered' not found.");
                }


                order.StatusId = statusDetails.StatusID;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the order.", ex);
            }
        }


        public async Task MarksToLaundryTake(MarkOrderDto markOrderDto)
        {
            try
            {
                // Find order by ID
                var order = await _context.Orders.FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);

                if (order == null)
                {
                    // Optionally handle not found
                    throw new Exception($"Order with ID  not found.");
                }

                // Update pickup driver ID
                order.DeliveryDriverId = markOrderDto.DriverId;

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Optionally log or rethrow
                throw new Exception("An error occurred while marking the order.", ex);
            }
        }
    }
}

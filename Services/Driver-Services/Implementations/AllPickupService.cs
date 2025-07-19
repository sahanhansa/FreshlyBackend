using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
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
                    join addr in _context.Addresses on cust.AddressId equals addr.AddressId
                    join laun in _context.Laundries on ord.LaundryId equals laun.LaundryId
                    join sta in _context.Statuses on ord.StatusId equals sta.StatusID
                    select new PickupDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = cust.CustomerId,
                        CustomerName = cust.FirstName + " " + cust.LastName,
                        Address = addr.HouseNo + " " + addr.Street + ", " + addr.City,
                        Status = sta.StatusName,
                        LaundryName = laun.LaundryName,
                        PickupDriverId = ord.PickupDriverId,
                        
                        // You MUST project subquery separately because ToList() cannot be inside projection in EF Core.
                        // So use a nested query here:
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId && c.UserType == "Customer")
                            .Select(c => c.ContactNumber)
                            .ToList()
                    }
                ).ToListAsync();

                return pickups;
            }
            catch (Exception ex)
            {
                // Log ex if needed
                throw new Exception("An error occurred while retrieving pickup details.", ex);
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
    join note in _context.DriverNotes on ord.OrderId equals note.OrderId into noteGroup
    from note in noteGroup.DefaultIfEmpty() // LEFT JOIN
    where ord.OrderId.ToString() == orderID
    select new PickupDetailsByIdDto
    {
        OrderId = ord.OrderId,
        CustomerName = cust.FirstName + " " + cust.LastName,
        Address = add.HouseNo + " " + add.Street + ", " + add.City,
        LaundryName = laun.LaundryName,
        Status = sta.StatusName,
        PickupDriverId = ord.PickupDriverId,
        note = note.Note, // Will be NULL if no note found
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

                return pickupDetails;

            }
            catch (Exception ex)
            {
                // You may log the error here or throw a custom exception
                throw new Exception($"An error occurred while retrieving order details for Order ID: {orderID}", ex);
            }
        }

       

        public async Task MarksToTake(MarkOrderDto markOrderDto)
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
                order.PickupDriverId = markOrderDto.DriverId;

                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Optionally log or rethrow
                throw new Exception("An error occurred while marking the order.", ex);
            }
        }

        public async Task MarksToDeliver(MarkOrderDto markOrderDto)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);
                if (order == null)
                {
                    throw new Exception($"Order with ID {markOrderDto.OrderId} not found.");
                }

                var noteDetails = await _context.DriverNotes.FirstOrDefaultAsync(d => d.OrderId == markOrderDto.OrderId);
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

                var status = await _context.Statuses.FirstOrDefaultAsync(d => d.StatusName == "order picked up ");
                if (status == null)
                {
                    throw new Exception("Status 'order picked up' not found.");
                }

                order.StatusId = status.StatusID;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the order.", ex);
            }
        }

    }
}

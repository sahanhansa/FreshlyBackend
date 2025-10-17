using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;

namespace FreshlyBackendNew.Services
{
    public class AllPickupService : IAllPickupService
    {
        private readonly ApplicationDbContext _context;

        public AllPickupService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ FIXED: GetAllPickups with proper status filter
        public async Task<List<PickupDetailsDto>> GetAllPickups()
        {
            try
            {
                // Get pickup-related status IDs
                var pickupStatuses = new[] { "order placed", "order picked up" };
                var pickupStatusIds = await _context.Statuses
                    .Where(s => s.StatusName != null && pickupStatuses.Contains(s.StatusName.ToLower()))
                    .Select(s => s.StatusID)
                    .ToListAsync();

                if (!pickupStatusIds.Any())
                    return new List<PickupDetailsDto>();

                // Project directly to DTO - much faster than loading entities
                var pickups = await _context.Orders
                    .AsNoTracking()
                    .Where(o => o.StatusId.HasValue && pickupStatusIds.Contains(o.StatusId.Value))
                    .Include(o => o.Customer)
                        .ThenInclude(c => c.Address)
                    .Include(o => o.Laundry)
                    .Include(o => o.Status)
                    .Select(ord => new PickupDetailsDto
                    {
                        OrderId = ord.OrderId,
                        CustomerId = ord.Customer.CustomerId,
                        CustomerName = ord.Customer.FirstName + " " + ord.Customer.LastName,
                        Address = ord.Customer.Address.HouseNo + " " + ord.Customer.Address.Street + ", " + ord.Customer.Address.City,
                        Status = ord.Status.StatusName,
                        LaundryName = ord.Laundry.LaundryName,
                        PickupDriverId = ord.PickupDriverId,
                        // ✅ FIXED: Get contacts separately since there's no navigation property
                        Contact = _context.Contacts
                            .Where(c => c.UserId == ord.CustomerId && c.UserType == "Customer")
                            .Select(c => c.ContactNumber)
                            .ToList()
                    })
                    .ToListAsync();

                return pickups;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving pickup details.", ex);
            }
        }

        // ✅ FIXED: GetPickupDetailsBYId
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
                        note = note != null ? note.Note : null,
                        // ✅ FIXED: Get contacts properly
                        Contact = _context.Contacts
                            .Where(c => c.UserId == cust.CustomerId && c.UserType == "Customer")
                            .Select(c => c.ContactNumber)
                            .ToList(),
                        // ✅ FIXED: Get order items properly
                        OrderItems = _context.OrderDetails
                            .Where(od => od.OrderId == ord.OrderId)
                            .Join(_context.Items,
                                  od => od.ItemId,
                                  i => i.ItemId,
                                  (od, i) => new OrderedItemsDto
                                  {
                                      ItemName = i.Name,
                                      Quantity = od.Quantity.HasValue ? (int)od.Quantity.Value : 0
                                  })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (pickupDetails == null)
                    throw new Exception($"Order with ID {orderID} not found.");

                return pickupDetails;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving order details for Order ID: {orderID}", ex);
            }
        }

        public async Task MarksToTake(MarkOrderDto markOrderDto)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == markOrderDto.OrderId);

                if (order == null)
                    throw new Exception($"Order with ID {markOrderDto.OrderId} not found.");

                order.PickupDriverId = markOrderDto.DriverId;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while marking the order.", ex);
            }
        }

        public async Task MarksToDeliver(MarkOrderDto markOrderDto)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == markOrderDto.OrderId);
                
                if (order == null)
                    throw new Exception($"Order with ID {markOrderDto.OrderId} not found.");

                // Handle driver note
                var noteDetails = await _context.DriverNotes
                    .FirstOrDefaultAsync(n => n.OrderId == markOrderDto.OrderId);
                
                if (noteDetails == null)
                {
                    noteDetails = new DriverNote
                    {
                        OrderId = markOrderDto.OrderId,
                        DriverId = markOrderDto.DriverId,
                        Note = markOrderDto.Note
                    };
                    await _context.DriverNotes.AddAsync(noteDetails);
                }
                else
                {
                    noteDetails.Note = markOrderDto.Note;
                }

                // Update order status
                var status = await _context.Statuses
                    .FirstOrDefaultAsync(s => s.StatusName == "order picked up");
                
                if (status == null)
                    throw new Exception("Status 'order picked up' not found.");

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

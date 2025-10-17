using FreshlyBackendNew.Common;
using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.DTOs.Order_DTOs;
using OrderDTOAddressDTO = FreshlyBackendNew.DTOs.Order_DTOs.AddressDTO; // Alias to resolve ambiguity
using DTOAddressDTO = FreshlyBackendNew.DTOs.AddressDTO; // Standard AddressDTO for CustomerDTO
using FreshlyBackendNew.Services.Interfaces;
using FreshlyBackendNew.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Implementations
{
    public partial class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        // GetNewOrdersAsync is in OptimizedOrderService.cs
        
        public async Task<List<OrderDTO>> GetProcessingOrdersAsync(Guid laundryId)
        {
            var processingStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName != null && s.StatusName.ToLower() == "processing in laundry");

            if (processingStatus == null)
                return [];

            var orders = await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == processingStatus.StatusID)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .ToListAsync();

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt?.ToString("yyyy-MM-dd"),
                    PlacedTime = o.PlacedAt?.ToString("HH:mm:ss"),
                    PickupDate = o.PickupAt?.ToString("yyyy-MM-dd"),
                    PickupTime = o.PickupAt?.ToString("HH:mm:ss")
                };

                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new DTOAddressDTO
                        {
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        };
                    }
                }

                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                if (o.Status != null)
                {
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName
                    };
                }

                result.Add(dto);
            }

            return result;
        }


        public async Task<List<OrderDTO>> GetCompletedOrdersAsync(Guid laundryId)
        {
    var deliveredStatus = await _context.Statuses
        .FirstOrDefaultAsync(s => s.StatusName != null && s.StatusName.ToLower() == "delivered");

    if (deliveredStatus == null)
        return [];

    var orders = await _context.Orders
        .Where(o => o.LaundryId == laundryId && o.StatusId == deliveredStatus.StatusID)
        .Include(o => o.Customer)
            .ThenInclude(c => c.Address)
        .Include(o => o.Laundry)
        .Include(o => o.Status)
        .ToListAsync();

    var result = new List<OrderDTO>();

    foreach (var o in orders)
    {
        var dto = new OrderDTO
        {
            OrderId = o.OrderId,
            PlacedDate = o.PlacedAt?.ToString("yyyy-MM-dd"),
            PlacedTime = o.PlacedAt?.ToString("HH:mm:ss"),
            PickupDate = o.PickupAt?.ToString("yyyy-MM-dd"),
            PickupTime = o.PickupAt?.ToString("HH:mm:ss"),
            Customer = o.Customer != null ? new CustomerDTO
            {
                CustomerId = o.Customer.CustomerId,
                FirstName = o.Customer.FirstName,
                LastName = o.Customer.LastName,
                Email = o.Customer.Email,
                Username = o.Customer.Username,
                Address = o.Customer.Address != null ? new DTOAddressDTO
                {
                    HouseNo = o.Customer.Address.HouseNo,
                    Street = o.Customer.Address.Street,
                    City = o.Customer.Address.City,
                    PostalCode = o.Customer.Address.PostalCode,
                    FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                } : null
            } : null,
            Laundry = o.Laundry != null ? new LaundryDTO
            {
                LaundryId = o.Laundry.LaundryId,
                LaundryName = o.Laundry.LaundryName
            } : null,
            Status = o.Status != null ? new StatusDTO
            {
                StatusID = o.Status.StatusID,
                StatusName = o.Status.StatusName
            } : null
        };

        result.Add(dto);
    }

    return result;
}

        public async Task<List<OrderDTO>> GetAllOrdersAsync(Guid laundryId)
        {
            // If an empty GUID is passed, return all orders regardless of laundryId
            IQueryable<Order> query = _context.Orders;
            
            if (laundryId != Guid.Empty)
            {
                query = query.Where(o => o.LaundryId == laundryId);
            }

            var orders = await query
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .ToListAsync();

            if (!orders.Any())
                return new List<OrderDTO>();

            // Get all order IDs for bulk operations
            var orderIds = orders.Select(o => o.OrderId).ToList();

            // Bulk load all order details
            var allOrderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId.Value))
                .ToListAsync();

            // Bulk load all pricing data for all laundries
            var allLaundryIds = orders.Select(o => o.LaundryId).Where(id => id.HasValue).Distinct().ToList();
            var pricingData = await _context.LaundryItemServices
                .Where(lis => allLaundryIds.Contains(lis.LaundryId))
                .Select(lis => new
                {
                    lis.LaundryId,
                    lis.ItemId,
                    lis.ServiceId,
                    lis.Price
                })
                .ToListAsync();

            // Create lookup dictionaries for fast access
            var orderDetailsLookup = allOrderDetails
                .GroupBy(od => od.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pricingLookup = pricingData
                .GroupBy(p => new { p.LaundryId, p.ItemId, p.ServiceId })
                .ToDictionary(g => g.Key, g => g.First().Price ?? 0);

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null,
                    PlacedDateTime = o.PlacedAt
                };

                // Calculate total cost using cached data
                decimal totalCost = 0;
                if (orderDetailsLookup.TryGetValue(o.OrderId, out var orderDetails))
                {
                foreach (var detail in orderDetails)
                {
                        var key = new { LaundryId = o.LaundryId, ItemId = detail.ItemId, ServiceId = detail.ServiceId };
                        if (pricingLookup.TryGetValue(key, out var price))
                        {
                    totalCost += price * (detail.Quantity ?? 0);
                        }
                    }
                }
                dto.TotalCost = totalCost;

                // Customer
                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username,
                        CustomerFName = o.Customer.FirstName ?? string.Empty,
                        CustomerLName = o.Customer.LastName ?? string.Empty
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new DTOAddressDTO
                        {
                            AddressId = o.Customer.Address.AddressId,
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        };
                    }
                }

                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                if (o.Status != null)
                {
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName,
                        StatusDisplayName = o.Status.StatusName ?? string.Empty
                    };
                }

                result.Add(dto);
            }

            return result;
        }
        
        public async Task<OrderDTO> GetOrderByIdAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return null;

            var dto = new OrderDTO
            {
                OrderId = order.OrderId,
                PlacedDate = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                PlacedTime = order.PlacedAt.HasValue ? order.PlacedAt.Value.ToString("HH:mm:ss") : null,
                PickupDate = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                PickupTime = order.PickupAt.HasValue ? order.PickupAt.Value.ToString("HH:mm:ss") : null,
                PlacedDateTime = order.PlacedAt
            };

            // Calculate total cost for the order
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == order.OrderId).ToListAsync();
            decimal totalCost = 0;
            foreach (var detail in orderDetails)
            {
                var price = await _context.LaundryItemServices
                    .Where(lis => lis.LaundryId == order.LaundryId && lis.ItemId == detail.ItemId && lis.ServiceId == detail.ServiceId)
                    .Select(lis => lis.Price ?? 0)
                    .FirstOrDefaultAsync();
                totalCost += price * (detail.Quantity ?? 0);
            }
            dto.TotalCost = totalCost;

            // Customer
            if (order.Customer != null)
            {
                dto.Customer = new CustomerDTO
                {
                    CustomerId = order.Customer.CustomerId,
                    FirstName = order.Customer.FirstName,
                    LastName = order.Customer.LastName,
                    Email = order.Customer.Email,
                    Username = order.Customer.Username,
                    CustomerFName = order.Customer.FirstName ?? string.Empty,
                    CustomerLName = order.Customer.LastName ?? string.Empty
                };

                if (order.Customer.Address != null)
                {
                    dto.Customer.Address = new DTOAddressDTO
                    {
                        AddressId = order.Customer.Address.AddressId,
                        HouseNo = order.Customer.Address.HouseNo,
                        Street = order.Customer.Address.Street,
                        City = order.Customer.Address.City,
                        PostalCode = order.Customer.Address.PostalCode,
                        FullAddress = $"{order.Customer.Address.HouseNo ?? ""}, {order.Customer.Address.Street ?? ""}, {order.Customer.Address.City ?? ""}, {order.Customer.Address.PostalCode ?? ""}"
                    };
                }
            }

            if (order.Laundry != null)
            {
                dto.Laundry = new LaundryDTO
                {
                    LaundryId = order.Laundry.LaundryId,
                    LaundryName = order.Laundry.LaundryName
                };
            }

            if (order.Status != null)
            {
                dto.Status = new StatusDTO
                {
                    StatusID = order.Status.StatusID,
                    StatusName = order.Status.StatusName,
                    StatusDisplayName = order.Status.StatusName ?? string.Empty
                };
            }

            return dto;
        }
        
        public async Task<Result<OrderDTO>> CreateOrderAsync(OrderDTO orderDto)
        {
            try
            {
                if (orderDto == null)
                    return Result<OrderDTO>.Failure("Order data is required");

                if (orderDto.Customer == null || orderDto.Customer.CustomerId == Guid.Empty)
                    return Result<OrderDTO>.Failure("Customer information is required");

                if (orderDto.Laundry == null || orderDto.Laundry.LaundryId == Guid.Empty)
                    return Result<OrderDTO>.Failure("Laundry information is required");

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = orderDto.Customer.CustomerId,
                    LaundryId = orderDto.Laundry.LaundryId,
                    PlacedAt = orderDto.PlacedDateTime ?? DateTime.UtcNow,
                    PickupAt = string.IsNullOrEmpty(orderDto.PickupDate) && string.IsNullOrEmpty(orderDto.PickupTime)
                        ? null
                        : DateTime.TryParse($"{orderDto.PickupDate} {orderDto.PickupTime}", out DateTime pickupDateTime)
                            ? pickupDateTime
                            : (DateTime?)null,
                    PaymentMethod = orderDto.PaymentMethod ?? "COD",
                    IsPaid = orderDto.IsPaid ?? false
                };

                // Set status
                if (orderDto.Status != null && orderDto.Status.StatusID != Guid.Empty)
                {
                    order.StatusId = orderDto.Status.StatusID;
                }
                else
                {
                    var defaultStatus = await _context.Statuses
                        .FirstOrDefaultAsync(s => s.StatusName != null && 
                            s.StatusName.ToLower() == "order placed");
                    
                    if (defaultStatus != null)
                    {
                        order.StatusId = defaultStatus.StatusID;
                    }
                }

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                var createdOrder = await GetOrderByIdAsync(order.OrderId);
                
                _logger.LogInformation("Order {OrderId} created successfully", order.OrderId);
                
                return Result<OrderDTO>.Success(createdOrder);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while creating order");
                return Result<OrderDTO>.Failure("Failed to save order to database. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return Result<OrderDTO>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
        
        public async Task<bool> UpdateOrderAsync(Guid id, OrderDTO orderDto)
        {
            if (orderDto == null || id != orderDto.OrderId)
            {
                return false;
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            // Parse dates if provided
            if (!string.IsNullOrEmpty(orderDto.PlacedDate) && !string.IsNullOrEmpty(orderDto.PlacedTime))
            {
                if (DateTime.TryParse($"{orderDto.PlacedDate} {orderDto.PlacedTime}", out DateTime placedDateTime))
                {
                    order.PlacedAt = placedDateTime;
                }
            }
            else if (orderDto.PlacedDateTime.HasValue)
            {
                order.PlacedAt = orderDto.PlacedDateTime.Value;
            }

            if (!string.IsNullOrEmpty(orderDto.PickupDate) && !string.IsNullOrEmpty(orderDto.PickupTime))
            {
                if (DateTime.TryParse($"{orderDto.PickupDate} {orderDto.PickupTime}", out DateTime pickupDateTime))
                {
                    order.PickupAt = pickupDateTime;
                }
            }

            // Update foreign keys if new values are provided in the DTO
            if (orderDto.Customer != null && orderDto.Customer.CustomerId != Guid.Empty)
            {
                order.CustomerId = orderDto.Customer.CustomerId;
            }

            if (orderDto.Laundry != null && orderDto.Laundry.LaundryId != Guid.Empty)
            {
                order.LaundryId = orderDto.Laundry.LaundryId;
            }

            if (orderDto.Status != null && orderDto.Status.StatusID != Guid.Empty)
            {
                order.StatusId = orderDto.Status.StatusID;
            }

            _context.Entry(order).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return await OrderExistsAsync(id);
            }
        }
        
        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }

            // Check if there are any related OrderDetails and delete them first
            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == id).ToListAsync();
            if (orderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(orderDetails);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
        
        private async Task<bool> OrderExistsAsync(Guid id)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == id);
        }

        //lasini-get relavant customer address
        public async Task<OrderDTOAddressDTO> GetCustomerAddressAsync(Guid customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null || customer.Address == null)
                return null;

            // Fetch contact numbers for this customer
            var contactNumbers = await _context.Contacts
                .Where(c => c.UserId == customerId && c.UserType == "Customer")
                .Select(c => c.ContactNumber)
                .ToListAsync();

            return new OrderDTOAddressDTO
            {
                AddressId = customer.Address.AddressId,
                HouseNo = customer.Address.HouseNo,
                Street = customer.Address.Street,
                City = customer.Address.City,
                PostalCode = customer.Address.PostalCode,
                ContactNumbers = contactNumbers
            };
        }

        //lasini-confirm new order
        public async Task<bool> ConfirmOrderAsync(ConfirmOrderDTO dto)
        {
            // Begin transaction to ensure data consistency
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get status IDs
                var orderPlacedStatusId = await _context.Statuses
                    .Where(s => s.StatusName == "Order Placed")
                    .Select(s => s.StatusID)
                    .FirstOrDefaultAsync();

                if (orderPlacedStatusId == Guid.Empty)
                {
                    // Status not found, create it
                    var newStatus = new Status { StatusID = Guid.NewGuid(), StatusName = "Order Placed" };
                    _context.Statuses.Add(newStatus);
                    await _context.SaveChangesAsync();
                    orderPlacedStatusId = newStatus.StatusID;
                }

                // Load the temporary order with customer and laundry details
                var tempOrder = await _context.TemporaryOrders
                    .Include(o => o.Customer)
                    .Include(o => o.Laundry)
                    .FirstOrDefaultAsync(o => o.TemporaryOrderId == dto.TemporaryOrderId);

                if (tempOrder == null)
                    return false;

                // Get all temporary order details
                var tempDetails = await _context.TemporaryOrderDetails
                    .Where(d => d.TemporaryOrderId == dto.TemporaryOrderId)
                    .ToListAsync();

                if (!tempDetails.Any())
                    return false; // No items to confirm

                // Update address if provided            
                if (dto.Address != null) // dto.Address is now UpdatedAddressDTO
                {
                    var address = await _context.Addresses.FindAsync(dto.Address.AddressId);
                    if (address != null)
                    {
                        address.HouseNo = dto.Address.HouseNo ?? address.HouseNo;
                        address.Street = dto.Address.Street ?? address.Street;
                        address.City = dto.Address.City ?? address.City;
                        address.PostalCode = dto.Address.PostalCode ?? address.PostalCode;
                    }
                }

                // Update contacts if provided
                if (dto.Contacts != null)
                {
                    foreach (var contactDto in dto.Contacts)
                    {
                        if (contactDto.ContactId.HasValue)
                        {
                            // Existing contact: update or delete
                            var contact = await _context.Contacts.FindAsync(contactDto.ContactId.Value);
                            if (contact != null && contact.UserId == tempOrder.CustomerId && contact.UserType == "customer")
                            {
                                if (contactDto.IsDeleted)
                                {
                                    _context.Contacts.Remove(contact);
                                }
                                else
                                {
                                    contact.ContactNumber = contactDto.ContactNumber;
                                }
                            }
                        }
                        else if (!contactDto.IsDeleted)
                        {
                            // New contact: add
                            _context.Contacts.Add(new Contact
                            {
                                ContactId = Guid.NewGuid(),
                                ContactNumber = contactDto.ContactNumber,
                                UserId = tempOrder.CustomerId,
                                UserType = "customer"
                            });
                        }
                    }
                }

                // Create new order
                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = tempOrder.CustomerId,
                    LaundryId = tempOrder.LaundryId,
                    //PlacedAt = DateTime.UtcNow,
                    PlacedAt = DateTime.UtcNow.AddTicks(-(DateTime.UtcNow.Ticks % TimeSpan.TicksPerSecond)),
                    PickupAt = dto.PickupAt,
                    StatusId = orderPlacedStatusId,
                    PaymentMethod = "COD", // or get from dto if you support online payment
                    IsPaid = false         // or get from dto if you support online payment

                };
                _context.Orders.Add(order);

                // Copy details from temporary order to order details
                foreach (var tempDetail in tempDetails)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ItemId = tempDetail.ItemId,
                        ServiceId = tempDetail.ServiceId,
                        Quantity = tempDetail.Quantity,
                        GarmentTypeId = tempDetail.GarmentTypeId
                    });
                }

                // Update temporary order status to "Order Placed"
                tempOrder.StatusId = orderPlacedStatusId;

                // Remove all temporary order details (as specified in requirements)
                _context.TemporaryOrderDetails.RemoveRange(tempDetails);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

       
public async Task<List<OrderDTO>> GetFilteredOrdersAsync(Guid laundryId)
{
    var validStatuses = new List<string>
    {
        "order picked up",
        "processing in laundry",
        "finished processing",
        "out for delivery",
        "delivered"
    };

    // Optimized query with all necessary includes and joins
    var ordersQuery = _context.Orders
        .Include(o => o.Customer)
            .ThenInclude(c => c.Address)
        .Include(o => o.Laundry)
        .Include(o => o.Status)
        .Where(o => o.LaundryId == laundryId && 
                    o.Status != null &&
                    validStatuses.Contains(o.Status.StatusName.ToLower()));

    var orders = await ordersQuery.ToListAsync();

    if (!orders.Any())
        return new List<OrderDTO>();

    // Get all order IDs for bulk operations
    var orderIds = orders.Select(o => o.OrderId).ToList();

    // Bulk load all order details
    var allOrderDetails = await _context.OrderDetails
        .Where(od => orderIds.Contains(od.OrderId.Value))
        .ToListAsync();

    // Bulk load all pricing data
    var pricingData = await _context.LaundryItemServices
        .Where(lis => lis.LaundryId == laundryId)
        .Select(lis => new
        {
            lis.ItemId,
            lis.ServiceId,
            lis.Price
        })
        .ToListAsync();

    // Create lookup dictionaries for fast access
    var orderDetailsLookup = allOrderDetails
        .GroupBy(od => od.OrderId)
        .ToDictionary(g => g.Key, g => g.ToList());

    var pricingLookup = pricingData
        .GroupBy(p => new { p.ItemId, p.ServiceId })
        .ToDictionary(g => g.Key, g => g.First().Price ?? 0);

    var result = new List<OrderDTO>();
    foreach (var o in orders)
    {
        var dto = new OrderDTO
        {
            OrderId = o.OrderId,
            PlacedDate = o.PlacedAt?.ToString("yyyy-MM-dd"),
            PlacedTime = o.PlacedAt?.ToString("HH:mm:ss"),
            PickupDate = o.PickupAt?.ToString("yyyy-MM-dd"),
            PickupTime = o.PickupAt?.ToString("HH:mm:ss"),
            PlacedDateTime = o.PlacedAt
        };

        // Calculate total cost using cached data
        decimal totalCost = 0;
        if (orderDetailsLookup.TryGetValue(o.OrderId, out var orderDetails))
        {
        foreach (var detail in orderDetails)
        {
                var key = new { ItemId = detail.ItemId, ServiceId = detail.ServiceId };
                if (pricingLookup.TryGetValue(key, out var price))
                {
            totalCost += price * (detail.Quantity ?? 0);
        }
            }
        }
        dto.TotalCost = totalCost;

        // Customer
        if (o.Customer != null)
        {
            dto.Customer = new CustomerDTO
            {
                CustomerId = o.Customer.CustomerId,
                FirstName = o.Customer.FirstName,
                LastName = o.Customer.LastName,
                Email = o.Customer.Email,
                Username = o.Customer.Username,
                CustomerFName = o.Customer.FirstName ?? string.Empty,
                CustomerLName = o.Customer.LastName ?? string.Empty,
                Address = o.Customer.Address == null ? null : new DTOAddressDTO
                {
                    AddressId = o.Customer.Address.AddressId,
                    HouseNo = o.Customer.Address.HouseNo,
                    Street = o.Customer.Address.Street,
                    City = o.Customer.Address.City,
                    PostalCode = o.Customer.Address.PostalCode,
                    FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                }
            };
        }

        // Laundry
        if (o.Laundry != null)
        {
            dto.Laundry = new LaundryDTO
            {
                LaundryId = o.Laundry.LaundryId,
                LaundryName = o.Laundry.LaundryName
            };
        }

        // Status
        if (o.Status != null)
        {
            dto.Status = new StatusDTO
            {
                StatusID = o.Status.StatusID,
                StatusName = o.Status.StatusName,
                StatusDisplayName = o.Status.StatusName ?? string.Empty
            };
        }

        result.Add(dto);
    }

    return result;
}

public async Task<SortedOrderIdsResponseDTO> GetSortedOrderIdsAsync(Guid laundryId)
{
    var orderIds = await _context.Orders
        .Where(o => o.LaundryId == laundryId)
        .OrderByDescending(o => o.PlacedAt)
        .Select(o => o.OrderId)
        .ToListAsync();

    return new SortedOrderIdsResponseDTO { OrderIds = orderIds };
}

        // New optimized methods
        public async Task<PaginatedOrderResponseDTO> GetFilteredOrdersPaginatedAsync(Guid laundryId, int pageNumber, int pageSize, string? statusFilter = null, string? searchTerm = null)
        {
            var validStatuses = new List<string>
            {
                "order picked up",
                "processing in laundry",
                "finished processing",
                "out for delivery",
                "delivered"
            };

            // Build base query
            var baseQuery = _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .Where(o => o.LaundryId == laundryId && 
                            o.Status != null &&
                            validStatuses.Contains(o.Status.StatusName.ToLower()));

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter.ToLower() != "all")
            {
                baseQuery = baseQuery.Where(o => o.Status.StatusName.ToLower() == statusFilter.ToLower());
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                baseQuery = baseQuery.Where(o => 
                    (o.Customer != null && 
                     (o.Customer.FirstName != null && o.Customer.FirstName.Contains(searchTerm)) ||
                     (o.Customer.LastName != null && o.Customer.LastName.Contains(searchTerm)) ||
                     (o.Customer.Email != null && o.Customer.Email.Contains(searchTerm))) ||
                    o.OrderId.ToString().Contains(searchTerm));
            }

            // Get total count for pagination
            var totalCount = await baseQuery.CountAsync();

            // Apply pagination
            var orders = await baseQuery
                .OrderByDescending(o => o.PlacedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!orders.Any())
                return new PaginatedOrderResponseDTO
                {
                    Orders = new List<OrderDTO>(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize),
                    HasPreviousPage = pageNumber > 1
                };

            // Bulk load related data
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var allOrderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId.Value))
                .ToListAsync();

            var pricingData = await _context.LaundryItemServices
                .Where(lis => lis.LaundryId == laundryId)
                .Select(lis => new
                {
                    lis.ItemId,
                    lis.ServiceId,
                    lis.Price
                })
                .ToListAsync();

            // Create lookup dictionaries
            var orderDetailsLookup = allOrderDetails
                .GroupBy(od => od.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pricingLookup = pricingData
                .GroupBy(p => new { p.ItemId, p.ServiceId })
                .ToDictionary(g => g.Key, g => g.First().Price ?? 0);

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt?.ToString("yyyy-MM-dd"),
                    PlacedTime = o.PlacedAt?.ToString("HH:mm:ss"),
                    PickupDate = o.PickupAt?.ToString("yyyy-MM-dd"),
                    PickupTime = o.PickupAt?.ToString("HH:mm:ss"),
                    PlacedDateTime = o.PlacedAt
                };

                // Calculate total cost using cached data
                decimal totalCost = 0;
                if (orderDetailsLookup.TryGetValue(o.OrderId, out var orderDetails))
                {
                    foreach (var detail in orderDetails)
                    {
                        var key = new { ItemId = detail.ItemId, ServiceId = detail.ServiceId };
                        if (pricingLookup.TryGetValue(key, out var price))
                        {
                            totalCost += price * (detail.Quantity ?? 0);
                        }
                    }
                }
                dto.TotalCost = totalCost;

                // Customer
                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username,
                        CustomerFName = o.Customer.FirstName ?? string.Empty,
                        CustomerLName = o.Customer.LastName ?? string.Empty,
                        Address = o.Customer.Address == null ? null : new DTOAddressDTO
                        {
                            AddressId = o.Customer.Address.AddressId,
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        }
                    };
                }

                // Laundry
                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                // Status
                if (o.Status != null)
                {
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName,
                        StatusDisplayName = o.Status.StatusName ?? string.Empty
                    };
                }

                result.Add(dto);
            }

            return new PaginatedOrderResponseDTO
            {
                Orders = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = pageNumber > 1
            };
        }

        public async Task<PaginatedOrderResponseDTO> GetAllOrdersPaginatedAsync(Guid laundryId, int pageNumber, int pageSize, string? searchTerm = null)
        {
            // Build base query
            IQueryable<Order> baseQuery = _context.Orders;
            
            if (laundryId != Guid.Empty)
            {
                baseQuery = baseQuery.Where(o => o.LaundryId == laundryId);
            }

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                baseQuery = baseQuery.Where(o => 
                    (o.Customer != null && 
                     (o.Customer.FirstName != null && o.Customer.FirstName.Contains(searchTerm)) ||
                     (o.Customer.LastName != null && o.Customer.LastName.Contains(searchTerm)) ||
                     (o.Customer.Email != null && o.Customer.Email.Contains(searchTerm))) ||
                o.OrderId.ToString().Contains(searchTerm));
            }

            // Get total count for pagination
            var totalCount = await baseQuery.CountAsync();

            // Apply pagination and includes
            var orders = await baseQuery
                .Include(o => o.Customer)
                .ThenInclude(c => c.Address)
                .Include(o => o.Laundry)
                .Include(o => o.Status)
                .OrderByDescending(o => o.PlacedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!orders.Any())
                return new PaginatedOrderResponseDTO
                {
                    Orders = new List<OrderDTO>(),
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize),
                    HasPreviousPage = pageNumber > 1
                };

            // Bulk load related data
            var orderIds = orders.Select(o => o.OrderId).ToList();
            var allOrderDetails = await _context.OrderDetails
                .Where(od => orderIds.Contains(od.OrderId.Value))
                .ToListAsync();

            var allLaundryIds = orders.Select(o => o.LaundryId).Where(id => id.HasValue).Distinct().ToList();
            var pricingData = await _context.LaundryItemServices
                .Where(lis => allLaundryIds.Contains(lis.LaundryId))
                .Select(lis => new
                {
                    lis.LaundryId,
                    lis.ItemId,
                    lis.ServiceId,
                    lis.Price
                })
                .ToListAsync();

            // Create lookup dictionaries
            var orderDetailsLookup = allOrderDetails
                .GroupBy(od => od.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pricingLookup = pricingData
                .GroupBy(p => new { p.LaundryId, p.ItemId, p.ServiceId })
                .ToDictionary(g => g.Key, g => g.First().Price ?? 0);

            var result = new List<OrderDTO>();
            foreach (var o in orders)
            {
                var dto = new OrderDTO
                {
                    OrderId = o.OrderId,
                    PlacedDate = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("yyyy-MM-dd") : null,
                    PlacedTime = o.PlacedAt.HasValue ? o.PlacedAt.Value.ToString("HH:mm:ss") : null,
                    PickupDate = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("yyyy-MM-dd") : null,
                    PickupTime = o.PickupAt.HasValue ? o.PickupAt.Value.ToString("HH:mm:ss") : null,
                    PlacedDateTime = o.PlacedAt
                };

                // Calculate total cost using cached data
                decimal totalCost = 0;
                if (orderDetailsLookup.TryGetValue(o.OrderId, out var orderDetails))
                {
                    foreach (var detail in orderDetails)
                    {
                        var key = new { LaundryId = o.LaundryId, ItemId = detail.ItemId, ServiceId = detail.ServiceId };
                        if (pricingLookup.TryGetValue(key, out var price))
                        {
                            totalCost += price * (detail.Quantity ?? 0);
                        }
                    }
                }
                dto.TotalCost = totalCost;

                // Customer
                if (o.Customer != null)
                {
                    dto.Customer = new CustomerDTO
                    {
                        CustomerId = o.Customer.CustomerId,
                        FirstName = o.Customer.FirstName,
                        LastName = o.Customer.LastName,
                        Email = o.Customer.Email,
                        Username = o.Customer.Username,
                        CustomerFName = o.Customer.FirstName ?? string.Empty,
                        CustomerLName = o.Customer.LastName ?? string.Empty
                    };

                    if (o.Customer.Address != null)
                    {
                        dto.Customer.Address = new DTOAddressDTO
                        {
                            AddressId = o.Customer.Address.AddressId,
                            HouseNo = o.Customer.Address.HouseNo,
                            Street = o.Customer.Address.Street,
                            City = o.Customer.Address.City,
                            PostalCode = o.Customer.Address.PostalCode,
                            FullAddress = $"{o.Customer.Address.HouseNo ?? ""}, {o.Customer.Address.Street ?? ""}, {o.Customer.Address.City ?? ""}, {o.Customer.Address.PostalCode ?? ""}"
                        };
                    }
                }

                if (o.Laundry != null)
                {
                    dto.Laundry = new LaundryDTO
                    {
                        LaundryId = o.Laundry.LaundryId,
                        LaundryName = o.Laundry.LaundryName
                    };
                }

                if (o.Status != null)
                {
                    dto.Status = new StatusDTO
                    {
                        StatusID = o.Status.StatusID,
                        StatusName = o.Status.StatusName,
                        StatusDisplayName = o.Status.StatusName ?? string.Empty
                    };
                }

                result.Add(dto);
            }

            return new PaginatedOrderResponseDTO
            {
                Orders = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = pageNumber < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = pageNumber > 1
            };
        }

        public async Task<int> GetTotalOrderCountAsync(Guid laundryId, string? statusFilter = null)
        {
            var validStatuses = new List<string>
            {
                "order picked up",
                "processing in laundry",
                "finished processing",
                "out for delivery",
                "delivered"
            };

            var query = _context.Orders.Where(o => o.LaundryId == laundryId);

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter.ToLower() != "all")
            {
                query = query.Where(o => o.Status != null && o.Status.StatusName.ToLower() == statusFilter.ToLower());
            }
            else
            {
                query = query.Where(o => o.Status != null && validStatuses.Contains(o.Status.StatusName.ToLower()));
            }

            return await query.CountAsync();
        }

        public async Task<int> GetOrderCountByStatusAsync(Guid laundryId, Guid statusId)
        {
            return await _context.Orders
                .Where(o => o.LaundryId == laundryId && o.StatusId == statusId)
                .CountAsync();
        }
    }
}
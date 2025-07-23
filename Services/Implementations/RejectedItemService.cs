using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FreshlyBackendNew.Services
{
    public class RejectedItemService : IRejectedItemService
    {
        private readonly ApplicationDbContext _context;

        public RejectedItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        //lasini
        public async Task<List<RejectedItemWithGarmentDTO>> GetRejectedItemsByOrderIdAsync(Guid orderId)
        {
            var rejectedItems = await _context.RejectedItems
                .Where(r => r.OrderId == orderId) // Filter by OrderId instead of LaundryId
        .Include(r => r.GarmentType)      // Include GarmentType details
                .ToListAsync();

            return rejectedItems.Select(MapToGarmentDTO).ToList();
        }
        private RejectedItemWithGarmentDTO MapToGarmentDTO(RejectedItem entity)
        {
            return new RejectedItemWithGarmentDTO
            {
                RejectedItemId = entity.RejectedItemId,
                OrderId = entity.OrderId,
                ItemId = entity.ItemId,
                ServiceId = entity.ServiceId,
                LaundryId = entity.LaundryId,
                GarmentTypeId = entity.GarmentTypeId,
                GarmentTypeName = entity.GarmentType?.GarmentTypeName,
                ItemName = entity.ItemName,
                Quantity = entity.Quantity,
                Reason = entity.Reason,
                RejectedBy = entity.RejectedBy,
                RejectedAt = entity.RejectedAt
            };
        }

        public async Task<RejectedItemDTO?> GetRejectedItemByIdAsync(Guid rejectedItemId)
        {
            var rejectedItem = await _context.RejectedItems
                .Include(r => r.Order)
                .FirstOrDefaultAsync(r => r.RejectedItemId == rejectedItemId);

            if (rejectedItem == null) return null;

            return MapToDTO(rejectedItem);
        }

        public async Task<RejectedItemDTO> AddRejectedItemAsync(RejectedItemDTO dto)
        {
            var rejectedItem = new RejectedItem
            {
                RejectedItemId = Guid.NewGuid(), // Always generate in backend
                OrderId = dto.OrderId,
                ItemId = dto.ItemId,
                ServiceId = dto.ServiceId, // ServiceId must exist in the correct table (see model/DB schema)
                LaundryId = dto.LaundryId,
                ItemName = dto.ItemName,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                RejectedBy = dto.RejectedBy,
                RejectedAt = dto.RejectedAt == default ? DateTime.UtcNow : dto.RejectedAt
            };

            _context.RejectedItems.Add(rejectedItem);

            // Update OrderDetail quantity
            if (dto.OrderId.HasValue && dto.ItemId.HasValue && dto.ServiceId.HasValue && dto.Quantity.HasValue)
            {
                var orderDetail = await _context.OrderDetails.FirstOrDefaultAsync(od =>
                    od.OrderId == dto.OrderId &&
                    od.ItemId == dto.ItemId &&
                    od.ServiceId == dto.ServiceId);

                if (orderDetail != null && orderDetail.Quantity.HasValue)
                {
                    orderDetail.Quantity -= dto.Quantity.Value;
                    if (orderDetail.Quantity <= 0)
                    {
                        _context.OrderDetails.Remove(orderDetail);
                    }
                    else
                    {
                        _context.OrderDetails.Update(orderDetail);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return MapToDTO(rejectedItem);
        }

        public async Task<IEnumerable<RejectedItemDTO>> GetRejectedItemsByLaundryIdAsync(Guid laundryId)
        {
            var rejectedItems = await _context.RejectedItems
                .Include(r => r.Order)
                .Where(r => r.LaundryId == laundryId)
                .ToListAsync();
            return rejectedItems.Select(MapToDTO);
        }

        private RejectedItemDTO MapToDTO(RejectedItem entity)
        {
            return new RejectedItemDTO
            {
                RejectedItemId = entity.RejectedItemId,
                OrderId = entity.OrderId,
                ItemId = entity.ItemId,
                ServiceId = entity.ServiceId,
                LaundryId = entity.LaundryId,
                ItemName = entity.ItemName,
                Quantity = entity.Quantity,
                Reason = entity.Reason,
                RejectedBy = entity.RejectedBy,
                RejectedAt = entity.RejectedAt,
                StatusId = entity.Order != null ? entity.Order.StatusId : null
            };
        }
    }
    }


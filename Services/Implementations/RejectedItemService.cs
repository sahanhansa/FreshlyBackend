using FreshlyBackendNew.Data;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.Models;
using FreshlyBackendNew.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services
{
    public class RejectedItemService : IRejectedItemService
    {
        private readonly ApplicationDbContext _context;

        public RejectedItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RejectedItemDTO?> GetRejectedItemByIdAsync(Guid rejectedItemId)
        {
            var rejectedItem = await _context.RejectedItems
                .FirstOrDefaultAsync(r => r.RejectedItemId == rejectedItemId);

            if (rejectedItem == null) return null;

            return MapToDTO(rejectedItem);
        }

        public async Task<RejectedItemDTO> AddRejectedItemAsync(RejectedItemDTO dto)
        {
            var rejectedItem = new RejectedItem
            {
                RejectedItemId = dto.RejectedItemId != Guid.Empty ? dto.RejectedItemId : Guid.NewGuid(),
                OrderId = dto.OrderId,
                ItemId = dto.ItemId,
                ServiceId = dto.ServiceId,
                LaundryId = dto.LaundryId,
                ItemName = dto.ItemName,
                Quantity = dto.Quantity,
                Reason = dto.Reason,
                RejectedBy = dto.RejectedBy,
                RejectedAt = dto.RejectedAt == default ? DateTime.UtcNow : dto.RejectedAt
            };

            _context.RejectedItems.Add(rejectedItem);
            await _context.SaveChangesAsync();

            return MapToDTO(rejectedItem);
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
                RejectedAt = entity.RejectedAt
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreshlyBackendNew.Services.Interfaces
{
    public interface IOrderCalculationService
    {
        Task<decimal> CalculateTotalCostAsync(Guid orderId);
        Task<Dictionary<Guid, decimal>> CalculateTotalCostBulkAsync(IEnumerable<Guid> orderIds);
    }
}

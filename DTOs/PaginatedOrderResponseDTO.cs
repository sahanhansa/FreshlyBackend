using System.Collections.Generic;

namespace FreshlyBackendNew.DTOs
{
    public class PaginatedOrderResponseDTO
    {
        public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
} 
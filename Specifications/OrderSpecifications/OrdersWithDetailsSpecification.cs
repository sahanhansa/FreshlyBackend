using FreshlyBackendNew.Models;

namespace FreshlyBackendNew.Specifications.OrderSpecifications
{
    public class OrdersWithDetailsSpecification : BaseSpecification<Order>
    {
        public OrdersWithDetailsSpecification(Guid laundryId, string statusFilter = null)
        {
            AddCriteria(o => o.LaundryId == laundryId && 
                (string.IsNullOrEmpty(statusFilter) || o.Status.StatusName == statusFilter));

            AddInclude(o => o.Customer);
            AddInclude(o => o.Laundry);
            AddInclude(o => o.Status);
            AddOrderByDescending(o => o.PlacedAt);
        }

        public OrdersWithDetailsSpecification(Guid laundryId, int skip, int take, string statusFilter = null)
            : this(laundryId, statusFilter)
        {
            ApplyPaging(skip, take);
        }
    }

    public class CompletedOrdersForLaundrySpec : BaseSpecification<Order>
    {
        public CompletedOrdersForLaundrySpec(Guid laundryId, Guid completedStatusId)
        {
            AddCriteria(o => o.LaundryId == laundryId && o.StatusId == completedStatusId);

            AddInclude(o => o.Customer);
        }
    }
}
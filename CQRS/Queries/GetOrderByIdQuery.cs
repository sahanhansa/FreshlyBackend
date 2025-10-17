using MediatR;
using FreshlyBackendNew.Common;
using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.CQRS.Queries
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderDTO>>;
}
using MediatR;
using AutoMapper;
using FreshlyBackendNew.Common;
using FreshlyBackendNew.DTOs;
using FreshlyBackendNew.UnitOfWork;
using FreshlyBackendNew.CQRS.Queries;

namespace FreshlyBackendNew.CQRS.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<OrderDTO>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            
            if (order == null)
                return Result<OrderDTO>.Failure("Order not found");

            var dto = _mapper.Map<OrderDTO>(order);
            return Result<OrderDTO>.Success(dto);
        }
    }
}
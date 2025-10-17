using FluentValidation;
using FreshlyBackendNew.DTOs;

namespace FreshlyBackendNew.Validators
{
    public class CreateOrderDTOValidator : AbstractValidator<OrderDTO>
    {
        public CreateOrderDTOValidator()
        {
            RuleFor(x => x.Customer.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required");

            RuleFor(x => x.Laundry.LaundryId)
                .NotEmpty().WithMessage("Laundry ID is required");

            When(x => x.PickupDate != null, () =>
            {
                RuleFor(x => DateTime.Parse(x.PickupDate))
                    .GreaterThan(DateTime.UtcNow).WithMessage("Pickup time must be in the future");
            });
        }
    }
}
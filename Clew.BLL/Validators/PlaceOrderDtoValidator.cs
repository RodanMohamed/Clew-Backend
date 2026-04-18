using FluentValidation;

namespace Clew.BLL
{
    public class PlaceOrderDtoValidator : AbstractValidator<PlaceOrderDto>
    {
        public PlaceOrderDtoValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .MaximumLength(50).WithMessage("Payment method must not exceed 50 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotNull().WithMessage("Shipping address is required.");

            RuleFor(x => x.ShippingAddress)
                .SetValidator(new ShippingAddressDtoValidator())
                .When(x => x.ShippingAddress != null);
        }
    }
}
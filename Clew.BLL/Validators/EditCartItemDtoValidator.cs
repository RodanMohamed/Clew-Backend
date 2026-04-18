using FluentValidation;

namespace Clew.BLL
{
    public class EditCartItemDtoValidator : AbstractValidator<EditCartItemDto>
    {
        public EditCartItemDtoValidator()
        {
            RuleFor(x => x.CartItemId)
                .NotEmpty().WithMessage("Cart item id is required.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is required.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
        }
    }
}
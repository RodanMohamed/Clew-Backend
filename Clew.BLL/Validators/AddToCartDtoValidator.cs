using FluentValidation;

namespace Clew.BLL
{
    public class AddToCartDtoValidator : AbstractValidator<AddToCartDto>
    {
        public AddToCartDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product id is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

            RuleFor(x => x.Color)
                .MaximumLength(50).WithMessage("Color must not exceed 50 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Color));
        }
    }
}
using FluentValidation;

namespace Clew.BLL
{
    public class ShippingAddressDtoValidator : AbstractValidator<ShippingAddressDto>
    {
        public ShippingAddressDtoValidator()
        {
            RuleFor(x => x.StreetAddress)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(250).WithMessage("Street address must not exceed 250 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.ZipCode)
                .NotEmpty().WithMessage("Zip code is required.")
                .MaximumLength(20).WithMessage("Zip code must not exceed 20 characters.");

            RuleFor(x => x.FullAddress)
                .MaximumLength(500).WithMessage("Full address must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.FullAddress));
        }
    }
}
using FluentValidation;
using Clew.Common;

namespace Clew.BLL
{
    public class ProductFilterParametersValidator : AbstractValidator<ProductFilterParameters>
    {
        public ProductFilterParametersValidator()
        {
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum price cannot be negative.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue)
                .WithMessage("Maximum price cannot be negative.");

            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
                .WithMessage("Minimum price cannot be greater than maximum price.");
        }
    }
}
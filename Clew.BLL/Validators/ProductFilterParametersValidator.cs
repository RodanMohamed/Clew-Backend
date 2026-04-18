using FluentValidation;
using Clew.Common;

namespace Clew.BLL
{
    public class ProductFilterParametersValidator : AbstractValidator<ProductFilterParameters>
    {
        public ProductFilterParametersValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50.");

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
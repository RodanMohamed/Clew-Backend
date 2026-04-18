using FluentValidation;

namespace Clew.BLL
{
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(150).WithMessage("Product name must not exceed 150 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Material)
                .NotEmpty().WithMessage("Material is required.")
                .MaximumLength(100).WithMessage("Material must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category id is required.");

            RuleFor(x => x.Image)
                .MaximumLength(500).WithMessage("Image path must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Image));
        }
    }
}
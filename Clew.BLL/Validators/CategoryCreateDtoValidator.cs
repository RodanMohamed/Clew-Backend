using FluentValidation;

namespace Clew.BLL
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

            RuleFor(x => x.Image)
                .MaximumLength(500).WithMessage("Image path must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Image));
        }
    }
}
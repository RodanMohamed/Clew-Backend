using FluentValidation;

namespace Clew.BLL
{
    public class ImageUploadDtoValidator : AbstractValidator<ImageUploadDto>
    {
        public ImageUploadDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.");

            RuleFor(x => x.File.Length)
                .GreaterThan(0).WithMessage("File is empty.")
                .When(x => x.File != null);
        }
    }
}
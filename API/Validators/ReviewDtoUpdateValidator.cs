using FluentValidation;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Validators
{
    public class ReviewDtoUpdateValidator : AbstractValidator<ReviewDtoUpdate>
    {
        public ReviewDtoUpdateValidator()
        {
            RuleFor(x => x.Rate).NotEmpty().NotNull().InclusiveBetween(1f,5f);
            RuleFor(x => x.Text).MaximumLength(512);
            RuleFor(x => x.ProductId).NotEmpty().NotNull();
        }
    }
}
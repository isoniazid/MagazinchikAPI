using FluentValidation;
using MagazinchikAPI.DTO.Banner;

namespace MagazinchikAPI.Validators
{
    public class BannerDtoCreateValidator : AbstractValidator<BannerDtoCreate>
    {
        public BannerDtoCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().MaximumLength(255).MinimumLength(2);
        }
    }
}
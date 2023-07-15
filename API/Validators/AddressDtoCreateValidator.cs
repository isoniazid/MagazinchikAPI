using FluentValidation;
using MagazinchikAPI.DTO.Address;

namespace MagazinchikAPI.Validators
{
    public class AddressDtoCreateValidator : AbstractValidator<AddressDtoCreate>
    {
        public AddressDtoCreateValidator()
        {
            RuleFor(x => x.Flat).NotEmpty().NotNull().MaximumLength(255);
            RuleFor(x => x.House).NotEmpty().NotNull().MaximumLength(255).MinimumLength(2);
            RuleFor(x => x.Street).NotEmpty().NotNull().MaximumLength(255).MinimumLength(2);
            RuleFor(x => x.City).NotEmpty().NotNull().MaximumLength(255).MinimumLength(2);
        }
    }
}
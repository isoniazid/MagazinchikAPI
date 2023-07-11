using FluentValidation;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Validators
{
    public class CathegoryDtoCreateValidator : AbstractValidator<CathegoryDtoCreate>
    {
        public CathegoryDtoCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
            
        }
    }
}
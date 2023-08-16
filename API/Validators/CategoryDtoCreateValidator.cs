using FluentValidation;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Validators
{
    public class CategoryDtoCreateValidator : AbstractValidator<CategoryDtoCreate>
    {
        public CategoryDtoCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).MinimumLength(3);
            
        }
    }
}
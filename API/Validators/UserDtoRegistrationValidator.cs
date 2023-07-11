namespace MagazinchikAPI.Validators
{
    using FluentValidation;
    using MagazinchikAPI.DTO.User;

    public class UserDtoRegistrationValidator : AbstractValidator<UserDtoRegistration>
    {
        public UserDtoRegistrationValidator()
        {
            RuleFor(dto => dto.Name).NotEmpty().MinimumLength(1).MaximumLength(32);
            RuleFor(dto => dto.Password).NotEmpty().MinimumLength(4).MaximumLength(32);
            RuleFor(dto => dto.Email).EmailAddress();
        }
    }
}
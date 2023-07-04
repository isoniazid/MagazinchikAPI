namespace MagazinchikAPI.Validators
{
    using FluentValidation;
    using MagazinchikAPI.DTO.User;
    public class UserDtoLoginValidator : AbstractValidator<UserDtoLogin>
    {
        public UserDtoLoginValidator()
        {
            RuleFor(dto => dto.Email).EmailAddress();
            RuleFor(dto => dto.Password).NotEmpty().MinimumLength(4).MaximumLength(32);
        }
    }
}
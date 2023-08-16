using FluentValidation;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Validators
{
    public class ProductDtoCreateValidator : AbstractValidator<ProductDtoCreate>
    {
        public ProductDtoCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}

/* public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public long CategoryId {get; set;} */
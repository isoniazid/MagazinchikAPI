using MagazinchikAPI.DTO.Review;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class ProductEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/product/get_all", GetAll).Produces<List<DTO.ProductDtoBaseInfo>>();

            app.MapPost("api/product/create", Create).WithTags("Admin");

            app.MapPost("api/product/leave_review", LeaveReview)
            .Produces<ReviewDtoCreateResult>(StatusCodes.Status200OK).WithTags("User")
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapPut("api/product/update_review", UpdateReview).WithTags("User")
            .Produces<ReviewDtoCreateResult>(200)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404);

            app.MapPost("api/product/add_to_favourite", AddToFavourite).WithTags("User")
            .Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400).Produces(200);

            app.MapDelete("api/product/remove_from_favourite", RemoveFromFavourite).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPost("api/product/add_to_cart", AddToCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapDelete("api/product/remove_from_cart", RemoveFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);

            app.MapPut("api/product/decrease_from_cart", DecreaseFromCart).WithTags("User")
            .Produces(200).Produces<APIErrorMessage>(404).Produces<APIErrorMessage>(401);
            
            
        }


        [Authorize]
        public async Task<IResult> LeaveReview(IProductService service, [FromBody] DTO.ReviewDtoCreate dto, HttpContext context)
        {
            return Results.Ok(await service.LeaveReview(dto, context));
        }


        [Authorize]
        public async Task<IResult> UpdateReview(IProductService service, [FromBody] DTO.ReviewDtoCreate dto, HttpContext context)
        {
            return Results.Ok(await service.UpdateReview(dto, context));
        }

        public async Task<IResult> GetAll(IProductService service)
        {
            return Results.Ok(await service.GetAll());
        }

        public async Task<IResult> Create(IProductService service, [FromBody] DTO.ProductDtoCreate dto)
        {
            await service.Create(dto);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> AddToFavourite(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromFavourite(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromFavourite(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> AddToCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.AddToCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> RemoveFromCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.RemoveFromCart(productId, context);
            return Results.Ok();
        }

        [Authorize]
        public async Task<IResult> DecreaseFromCart(IProductService service, [FromQuery] long productId, HttpContext context)
        {
            await service.DecreaseFromCart(productId, context);
            return Results.Ok();
        }


    }
}
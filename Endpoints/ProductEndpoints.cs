using MagazinchikAPI.DTO.Review;
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
            .Produces(401);

            app.MapPost("api/product/add_to_favourite", AddToFavourite).WithTags("User")
            .Produces(404).Produces(401).Produces(400).Produces(200);
        }


        [Authorize]
        public async Task<IResult> LeaveReview(IProductService service, [FromBody] DTO.ReviewDtoCreate dto, HttpContext context)
        {
            return Results.Ok(await service.LeaveReview(dto, context));
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


    }
}
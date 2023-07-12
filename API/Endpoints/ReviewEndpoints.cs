using MagazinchikAPI.DTO.Review;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class ReviewEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("api/review/all_for_product", GetReviewsForProduct).WithTags("Common")
           .Produces<DTO.Page<ReviewDtoBaseInfo>>().Produces<APIErrorMessage>(400);

            app.MapGet("api/review/ratelist", GetProductRateList).WithTags("Common")
            .Produces<ReviewDtoRateList>().Produces<APIErrorMessage>(404);

            app.MapPost("api/review/create", LeaveReview).WithTags("User")
            .Produces<ReviewDtoCreateResult>(StatusCodes.Status200OK)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(400);

            app.MapPut("api/review/update", UpdateReview).WithTags("User")
            .Produces<ReviewDtoCreateResult>(200)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(401).Produces<APIErrorMessage>(404);
        }


        [Authorize]
        public async Task<IResult> LeaveReview(IReviewService service, [FromBody] DTO.ReviewDtoCreate dto, HttpContext context)
        {
            return Results.Ok(await service.LeaveReview(dto, context));
        }


        [Authorize]
        public async Task<IResult> UpdateReview(IReviewService service, [FromBody] DTO.ReviewDtoUpdate dto, HttpContext context)
        {
            return Results.Ok(await service.UpdateReview(dto, context));
        }

        public IResult GetReviewsForProduct(IReviewService service, [FromQuery] long productId, [FromQuery] int limit, [FromQuery] int offset)
        {
            return Results.Ok(service.GetReviewsForProduct(productId, limit, offset));
        }

        public IResult GetProductRateList(IReviewService service, [FromQuery] long productId)
        {
            return Results.Ok(service.GetProductRateList(productId));
        }
    }
}
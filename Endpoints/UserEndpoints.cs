using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class UserEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapGet("user/product/get_all", GetAll).Produces<List<DTO.ProductDtoBaseInfo>>();
            app.MapPost("user/product/create", Create);
            app.MapPost("/user/product/review",  (x) => throw new NotImplementedException());
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


    }
}
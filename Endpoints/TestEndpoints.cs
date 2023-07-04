using MagazinchikAPI.Services;
using MagazinchikAPI.DTO;

namespace MagazinchikAPI.Endpoints
{
    public class TestEndpoints
    {
      /*   public void Define(WebApplication app)
        {
            app.MapGet("/test", HelloWorld);
            //app.MapGet("/test/getallProducts", GetProduct).Produces<List<ProductDto>>(StatusCodes.Status200OK);
            app.MapPost("/test/addProduct", Test);
        }

        public string HelloWorld()
        {
            return "Hello World!";
        }
        
        public async Task<IResult> Test([FromBody] CartProductDto dto)
        {
            return Results.Ok(dto);
        }

        public async Task<IResult> GetProduct(IProductService service)
        {
            return Results.Ok(await service.GetAll());
        }


        public async Task<IResult> AddProduct(IProductService service, [FromBody] ProductDto dto)
        {
            await service.Create(dto);
            return Results.Ok();
        }  */
    }
}
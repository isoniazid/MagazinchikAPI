using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.Category;
using MagazinchikAPI.Infrastructure.ExceptionHandler;
using MagazinchikAPI.Services;

namespace MagazinchikAPI.Endpoints
{
    public class CategoryEndpoints
    {
        public void Define(WebApplication app)
        {
            app.MapPost("api/category/create", CreateCategory).WithTags("Admin")
            .Produces<CategoryDtoCreated>(200)
            .Produces<ValidatorErrorMessage>(StatusCodes.Status422UnprocessableEntity)
            .Produces<APIErrorMessage>(400)
            .Produces(403);


            app.MapGet("api/category/random", GetRandomCategories).WithTags("Category")
            .Produces<CategoryDtoCreated>(200)
            .Produces<APIErrorMessage>(400);

            app.MapGet("api/category/descendants", GetByIdDescendants).WithTags("Category")
            .Produces<CategoryDtoDescendants>(200)
            .Produces<APIErrorMessage>(404)
            .Produces<APIErrorMessage>(400);

            app.MapGet("api/category/parents", GetByIdParents).WithTags("Category")
            .Produces<CategoryDtoDescendants>(200)
            .Produces<APIErrorMessage>(404)
            .Produces<APIErrorMessage>(400);

            app.MapGet("api/category/all", GetAll).WithTags("Category")
            .Produces<CategoryDtoDescendants>(200);
        }


        [Authorize(Roles = "ADMIN")]
        public async Task<IResult> CreateCategory(ICategoryService service, [FromBody] CategoryDtoCreate dto)
        {
            return Results.Ok(await service.CreateCategory(dto));
        }

        public IResult GetRandomCategories(ICategoryService service, [FromQuery] int count)
        {
            return Results.Ok(service.GetRandomCategories(count));
        }

        public async Task<IResult> GetByIdDescendants(ICategoryService service, [FromQuery] long categoryId)
        {
            return Results.Ok(await service.GetByIdDescendants(categoryId));
        }

        public async Task<IResult> GetByIdParents(ICategoryService service, [FromQuery] long categoryId)
        {
            return Results.Ok(await service.GetByIdParents(categoryId));
        }

        public async Task<IResult> GetAll(ICategoryService service)
        {
            return Results.Ok(await service.GetAll());
        }
    }
}
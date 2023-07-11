using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;
using MagazinchikAPI.Services;
using MagazinchikAPI.Validators;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace Tests;

public class ProductServiceTests
{

    private readonly MagazinchikAPI.Services.ProductService _service;
    private readonly Mock<AutoMapper.IMapper> _mapperMock = new();
    private Mock<ApplicationDbContext> _contextMock = new();

    public ProductServiceTests()
    {
        _service = new ProductService(_contextMock.Object, _mapperMock.Object, new ReviewDtoCreateValidator(), new ReviewDtoUpdateValidator());
    }


    [Fact]
    public async Task GetBaseInfo_ShouldReturn404_IfDoesNotExist()
    {
        //Arrange

        _contextMock.Setup<DbSet<Product>>(x => x.Products)
        .ReturnsDbSet(new List<Product>());

        long inputValue = new Random().NextInt64(long.MaxValue);

        //Act
        Func<Task> act = () => _service.GetBaseInfo(inputValue);


        //Assert

        var exception = await Assert.ThrowsAsync<APIException>(act);
        Assert.Equal(404, exception.StatusCode);
    }
}
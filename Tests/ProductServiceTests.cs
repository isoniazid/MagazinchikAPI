using MagazinchikAPI.Infrastructure;
using MagazinchikAPI.Model;
using MagazinchikAPI.Services;
using MagazinchikAPI.Validators;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using AutoMapper;
using MagazinchikAPI.DTO;

namespace Tests;

public class ProductServiceTests
{

    private readonly MagazinchikAPI.Services.ProductService _service;
    private readonly IMapper _mapper;
    private Mock<ApplicationDbContext> _contextMock = new();

    public ProductServiceTests()
    {
        var mapperConfig = new MapperConfiguration(x => x.AddProfile(new ApplicationProfile()));
        _mapper = new Mapper(mapperConfig);
        _service = new ProductService(_contextMock.Object, _mapper, new ReviewDtoCreateValidator(), new ReviewDtoUpdateValidator());
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



    [Fact]
    public async Task GetBaseInfo_ShouldReturnProductDto_IfExists()
    {
        //Arrange
        var productId = new Random().NextInt64(long.MaxValue);

        var fakeProducts = new List<Product>
        {
            new Product()
            {
                Id = productId,
                Name = "TestProduct",
                Price = 123.0M,
                Description = "helloworld"
            }
        };

        var fakeResult = _mapper.Map<ProductDtoBaseInfo>(fakeProducts[0]);

        _contextMock.Setup<DbSet<Product>>(x => x.Products)
        .ReturnsDbSet(fakeProducts);



        //Act
        var result = await _service.GetBaseInfo(productId);


        //Assert
        Assert.IsType<MagazinchikAPI.DTO.ProductDtoBaseInfo>(result);
        Assert.Equal(result.Id, fakeResult.Id);
        Assert.Equal(result.Name, fakeResult.Name);
        Assert.Equal(result.Price, fakeResult.Price);
        Assert.Equal(result.Description, fakeResult.Description);
    }
}
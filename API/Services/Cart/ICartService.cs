using MagazinchikAPI.DTO;
using MagazinchikAPI.DTO.CartProduct;

namespace MagazinchikAPI.Services
{
    public interface ICartService
    {
        public Task AddToCart(long productId, HttpContext context);

         public Task RemoveFromCart(long productId, HttpContext context);

         public Task DecreaseFromCart(long productId, HttpContext context);

         public Task<Page<CartProductDtoBaseInfo>> GetAllForUser(HttpContext context, int limit, int offset);

    }
}
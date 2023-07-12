namespace MagazinchikAPI.Services
{
    public interface ICartService
    {
        public Task AddToCart(long productId, HttpContext context);

         public Task RemoveFromCart(long productId, HttpContext context);

         public Task DecreaseFromCart(long productId, HttpContext context);

    }
}
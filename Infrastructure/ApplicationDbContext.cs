using MagazinchikAPI.Model;

namespace MagazinchikAPI.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
        public DbSet<Activation> Activations => Set<Activation>();
        public DbSet<Address> Adresses => Set<Address>();
        public DbSet<CartProduct> CartProducts => Set<CartProduct>();
        public DbSet<Cathegory> Cathegories => Set<Cathegory>();
        public DbSet<Favourite> Favourites => Set<Favourite>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<User> Users => Set<User>();
    }
}
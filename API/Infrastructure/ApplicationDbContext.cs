using MagazinchikAPI.Model;

namespace MagazinchikAPI.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderProduct> OrderProducts => Set<OrderProduct>();
        public DbSet<Activation> Activations => Set<Activation>();
        public DbSet<CartProduct> CartProducts => Set<CartProduct>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Favourite> Favourites => Set<Favourite>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Address> Addresses => Set<Address>();
        public virtual DbSet<Product> Products => Set<Product>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Review> Reviews => Set<Review>();
        public virtual DbSet<User> Users => Set<User>();
    }
}
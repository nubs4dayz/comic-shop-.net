using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.DomainModels;
using Shop.Domain.Identity;

namespace Shop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<ShopApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comic> Comics { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ComicInShoppingCart> ComicInShoppingCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Comic>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ComicInShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ComicInShoppingCart>()
                .HasOne(z => z.CurrentComic)
                .WithMany(z => z.ComicInShoppingCart)
                .HasForeignKey(z => z.ComicId); ;

            builder.Entity<ComicInShoppingCart>()
                .HasOne(z => z.UserCart)
                .WithMany(z => z.ComicInShoppingCart)
                .HasForeignKey(z => z.ShoppingCartId);

            builder.Entity<ShoppingCart>()
                .HasOne<ShopApplicationUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);

            builder.Entity<ComicInOrder>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ComicInOrder>()
                .HasOne(z => z.Comic)
                .WithMany(z => z.ComicInOrders)
                .HasForeignKey(z => z.ComicId);

            builder.Entity<ComicInOrder>()
                .HasOne(z => z.Order)
                .WithMany(z => z.ComicInOrders)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
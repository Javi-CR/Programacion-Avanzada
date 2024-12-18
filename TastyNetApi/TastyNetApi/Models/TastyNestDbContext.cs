using Microsoft.EntityFrameworkCore;

namespace TastyNetApi.Models
{
    public class TastyNestDbContext : DbContext
    {
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public TastyNestDbContext(DbContextOptions<TastyNestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>()
                .Property(u => u.CreatedUser)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Recipe>()
                .Property(r => r.CreatedRecipes)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Favorite>()
                .Property(f => f.CreatedFavorites)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RecipeStep>()
                .HasOne(rs => rs.Recipe)
                .WithMany(r => r.RecipeSteps)
                .HasForeignKey(rs => rs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Recipe)
                .WithMany(r => r.Favorites)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Roles>()
                .HasIndex(r => r.RolName)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasIndex(e => e.Email)
                .IsUnique();
            modelBuilder.Entity<Users>()
                .HasIndex(e => e.IdentificationNumber)
                .IsUnique();

            modelBuilder.Entity<Users>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

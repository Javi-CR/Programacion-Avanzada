using Microsoft.EntityFrameworkCore;

namespace TastyNetApi.Models
{
    public class TastyNestDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<Favorite> Favorites { get; set; } // Aquí está el DbSet faltante

        public TastyNestDbContext(DbContextOptions<TastyNestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Recipe)
                .WithMany(r => r.Favorites)
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ingredient>()
                .HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId);

            modelBuilder.Entity<RecipeStep>()
                .HasOne(s => s.Recipe)
                .WithMany(r => r.RecipeSteps)
                .HasForeignKey(s => s.RecipeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

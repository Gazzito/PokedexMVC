using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  // Provides identity support with Entity Framework Core.
using Microsoft.EntityFrameworkCore;  // Core namespace for Entity Framework functionality.
using PokedexMVC.Models;  // Access to the application's models (e.g., Pokemon, Pack, Region).

namespace PokedexMVC.Data
{
    // ApplicationDbContext inherits from IdentityDbContext to include ASP.NET Core Identity functionality.
    public class ApplicationDbContext : IdentityDbContext
    {
        // Constructor that accepts DbContextOptions to configure the database connection.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets represent the tables in the database for different entities (Pokemon, Pack, etc.).
        public DbSet<Pokemon> Pokemon { get; set; } = default!;
        public DbSet<Pack> Pack { get; set; } = default!;
        public DbSet<PokemonInPack> PokemonInPacks { get; set; } = default!;
        public DbSet<Region> Region { get; set; } = default!;

        // This method is called when the model for a derived context has been initialized.
        // It configures the relationships and constraints for the models.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating to include Identity-related configuration.
            base.OnModelCreating(modelBuilder);

            // Configure a composite key for the PokemonInPack entity (PackId and PokemonId).
            modelBuilder.Entity<PokemonInPack>()
                .HasKey(pip => new { pip.PackId, pip.PokemonId }); // Composite key

            // Configure the relationship between PokemonInPack and Pokemon
            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pokemon)  // Each PokemonInPack has one Pokemon.
                .WithMany(p => p.PokemonInPacks)  // Each Pokemon can be in many packs (PokemonInPack).
                .HasForeignKey(pip => pip.PokemonId)  // Foreign key is PokemonId in PokemonInPack.
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete of Pokemon when deleting a pack.

            // Configure the relationship between PokemonInPack and Pack
            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pack)  // Each PokemonInPack has one Pack.
                .WithMany(pack => pack.PokemonInPacks)  // Each Pack can contain many Pokemon (PokemonInPack).
                .HasForeignKey(pip => pip.PackId)  // Foreign key is PackId in PokemonInPack.
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete of Pack when deleting a Pokemon.

            // Configure the relationship between Pokemon and Region
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Region)  // Each Pokemon belongs to one Region.
                .WithMany(r => r.Pokemons)  // Each Region can contain many Pokemon.
                .HasForeignKey(p => p.RegionId)  // Foreign key is RegionId in Pokemon.
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete of Region when deleting a Pokemon.

            // Configure the relationship between Pokemon and CreatedByUser (user who created the Pokemon)
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.CreatedByUser)  // Each Pokemon has a user who created it.
                .WithMany()  // No reverse navigation needed.
                .HasForeignKey(p => p.CreatedByUserId)  // Foreign key is CreatedByUserId in Pokemon.
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete of user when deleting a Pokemon.

            // Configure the relationship between Pokemon and UpdatedByUser (user who last updated the Pokemon)
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.UpdatedByUser)  // Each Pokemon has a user who last updated it.
                .WithMany()  // No reverse navigation needed.
                .HasForeignKey(p => p.UpdatedByUserId)  // Foreign key is UpdatedByUserId in Pokemon.
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete of user when deleting a Pokemon.

            // Configure the precision for the price field in the Pack entity.
            modelBuilder.Entity<Pack>()
                .Property(p => p.Price)  // Configure the Price property in Pack.
                .HasColumnType("decimal(18,2)");  // Set precision to 18 digits with 2 decimal places.
        }
    }
}

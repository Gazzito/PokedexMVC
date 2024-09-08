using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Models;

namespace PokedexMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pokemon> Pokemon { get; set; } = default!;
        public DbSet<Pack> Pack { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the foreign key between PokemonInPack and Pack
            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pack)
                .WithMany(pack => pack.PokemonInPacks)
                .HasForeignKey(pip => pip.PackId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Configure the foreign key between PokemonInPack and Pokemon
            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pokemon)
                .WithMany(pokemon => pokemon.PokemonInPacks)
                .HasForeignKey(pip => pip.PokemonId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Disable cascade delete for RegionId in Pokemon to avoid multiple cascade paths
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Region)
                .WithMany(r => r.Pokemons) // Ensure 'Pokemons' collection exists in 'Region'
                .HasForeignKey(p => p.RegionId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascading delete

            // Disable cascade delete for CreatedByUserId in Pokemon
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Disable cascade delete for UpdatedByUserId in Pokemon
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.UpdatedByUser)
                .WithMany()
                .HasForeignKey(p => p.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for Pack.Price
            modelBuilder.Entity<Pack>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
        public DbSet<PokedexMVC.Models.Region> Region { get; set; } = default!;
    }
}

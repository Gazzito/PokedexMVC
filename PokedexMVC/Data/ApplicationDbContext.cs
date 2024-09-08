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
        public DbSet<PokemonInPack> PokemonInPacks { get; set; } = default!;
        public DbSet<Region> Region { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the foreign key between PokemonInPack and Pack
            modelBuilder.Entity<PokemonInPack>()
                .HasKey(pip => new { pip.PackId, pip.PokemonId }); // Composite key

            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pokemon)
                .WithMany(p => p.PokemonInPacks)
                .HasForeignKey(pip => pip.PokemonId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            modelBuilder.Entity<PokemonInPack>()
                .HasOne(pip => pip.Pack)
                .WithMany(pack => pack.PokemonInPacks)
                .HasForeignKey(pip => pip.PackId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Configure Pokemon relationships
            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.Region)
                .WithMany(r => r.Pokemons)
                .HasForeignKey(p => p.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.CreatedByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pokemon>()
                .HasOne(p => p.UpdatedByUser)
                .WithMany()
                .HasForeignKey(p => p.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Pack price precision
            modelBuilder.Entity<Pack>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

        }
    }
}

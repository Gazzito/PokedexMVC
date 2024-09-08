using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PokedexMVC.Models
{
    public class Pack
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] // Explicitly specifying decimal precision
        public decimal Price { get; set; }

        public byte[]? Image { get; set; }

        [Required]
        public double BronzeChance { get; set; }

        [Required]
        public double SilverChance { get; set; }

        [Required]
        public double GoldChance { get; set; }

        [Required]
        public double PlatinumChance { get; set; }

        [Required]
        public double DiamondChance { get; set; }

        [Required]
        public int TotalBought { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedByUserId { get; set; }

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }

        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }

        // Add this collection for the relationship with PokemonInPack
        public virtual ICollection<PokemonInPack> PokemonInPacks { get; set; } = new List<PokemonInPack>();
    }
}

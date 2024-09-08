using System;  // For DateTime and collection functionalities.
using System.Collections.Generic;  // For ICollection.
using System.ComponentModel.DataAnnotations;  // For validation attributes.
using System.ComponentModel.DataAnnotations.Schema;  // For foreign key attributes and data type precision.
using Microsoft.AspNetCore.Identity;  // For IdentityUser, representing users.

namespace PokedexMVC.Models
{
    // Represents a "Pack" that contains Pokémon and has a price and rarity chance system.
    public class Pack
    {
        // Primary key for the Pack entity.
        [Key]
        public int Id { get; set; }

        // Name of the pack, required, with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Price of the pack, required, with precision defined as decimal(18,2).
        [Required]
        [Column(TypeName = "decimal(18,2)")]  // Explicitly specifying decimal precision for price.
        public decimal Price { get; set; }

        // Byte array to store the image of the pack, optional.
        public byte[]? Image { get; set; }

        // Probability of getting a Bronze-tier item in the pack, required.
        [Required]
        public double BronzeChance { get; set; }

        // Probability of getting a Silver-tier item in the pack, required.
        [Required]
        public double SilverChance { get; set; }

        // Probability of getting a Gold-tier item in the pack, required.
        [Required]
        public double GoldChance { get; set; }

        // Probability of getting a Platinum-tier item in the pack, required.
        [Required]
        public double PlatinumChance { get; set; }

        // Probability of getting a Diamond-tier item in the pack, required.
        [Required]
        public double DiamondChance { get; set; }

        // The total number of times this pack has been bought, required.
        [Required]
        public int TotalBought { get; set; }

        // The date and time when the pack was created, required.
        [Required]
        public DateTime CreatedOn { get; set; }

        // User ID of the user who created the pack, required.
        [Required]
        public string CreatedByUserId { get; set; }

        // The date and time when the pack was last updated, optional.
        public DateTime? UpdatedOn { get; set; }

        // User ID of the user who last updated the pack, optional.
        public string? UpdatedByUserId { get; set; }

        // Navigation properties
        // Reference to the user who created the pack.
        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }

        // Reference to the user who last updated the pack, if applicable.
        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }

        // Collection of Pokémon in this pack (many-to-many relationship via PokemonInPack).
        public virtual ICollection<PokemonInPack> PokemonInPacks { get; set; } = new List<PokemonInPack>();
    }
}

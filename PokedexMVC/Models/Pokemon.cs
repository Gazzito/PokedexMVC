using System;  // For DateTime and collection functionalities.
using System.Collections.Generic;  // For ICollection.
using System.ComponentModel.DataAnnotations;  // For validation attributes.
using System.ComponentModel.DataAnnotations.Schema;  // For foreign key attributes.
using Microsoft.AspNetCore.Identity;  // For IdentityUser, representing users.

namespace PokedexMVC.Models
{
    // Represents a Pokémon with various attributes and relationships to other entities.
    public class Pokemon
    {
        // Primary key for the Pokemon entity.
        [Key]
        public int Id { get; set; }

        // Name of the Pokémon, required, with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Foreign key to the Region entity, required.
        [Required]
        public int RegionId { get; set; }

        // Base attack points for the Pokémon, required.
        [Required]
        public int BaseAttackPoints { get; set; }

        // Base health points for the Pokémon, required.
        [Required]
        public int BaseHealthPoints { get; set; }

        // Base defense points for the Pokémon, required.
        [Required]
        public int BaseDefensePoints { get; set; }

        // Base speed points for the Pokémon, required.
        [Required]
        public int BaseSpeedPoints { get; set; }

        // The date and time when the Pokémon was created, required.
        [Required]
        public DateTime CreatedOn { get; set; }

        // User ID of the user who created the Pokémon, required.
        [Required]
        public string CreatedByUserId { get; set; }

        // The date and time when the Pokémon was last updated, optional.
        public DateTime? UpdatedOn { get; set; }

        // User ID of the user who last updated the Pokémon, optional.
        public string? UpdatedByUserId { get; set; }

        // Byte array to store the image of the Pokémon, optional.
        public byte[]? Image { get; set; }

        // Navigation properties
        // Reference to the related Region entity.
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        // Reference to the user who created the Pokémon.
        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }

        // Reference to the user who last updated the Pokémon, if applicable.
        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }

        // Collection of related Pokémon in packs (many-to-many relationship).
        public virtual ICollection<PokemonInPack> PokemonInPacks { get; set; } = new List<PokemonInPack>();
    }
}

using Microsoft.AspNetCore.Identity;  // For IdentityUser, representing users in the application.
using PokedexMVC.Models;  // For referencing other models, like Pokemon.
using System.ComponentModel.DataAnnotations;  // For validation attributes.
using System.ComponentModel.DataAnnotations.Schema;  // For foreign key attributes.
using System;  // For DateTime, a system type.

namespace PokedexMVC.Models
{
    // The Region class represents a region within the Pokedex system.
    public class Region
    {
        // Primary key for the Region entity.
        [Key]
        public int Id { get; set; }

        // Name of the region, required and with a maximum length of 100 characters.
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Date and time when the region was created, required.
        [Required]
        public DateTime CreatedOn { get; set; }

        // User ID of the user who created the region, required.
        [Required]
        public string CreatedByUserId { get; set; }

        // Date and time when the region was last updated, optional.
        public DateTime? UpdatedOn { get; set; }

        // User ID of the user who last updated the region, optional.
        public string? UpdatedByUserId { get; set; }

        // Navigation property for the user who created the region (foreign key).
        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }

        // Navigation property for the user who last updated the region (foreign key).
        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }

        // Navigation property representing a collection of Pokémon associated with this region (one-to-many relationship).
        public virtual ICollection<Pokemon> Pokemons { get; set; } = new List<Pokemon>();  // Initializes as an empty collection.
    }
}

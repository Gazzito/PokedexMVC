using System;  // For DateTime.
using System.ComponentModel.DataAnnotations;  // For validation attributes.
using System.ComponentModel.DataAnnotations.Schema;  // For foreign key attributes.

namespace PokedexMVC.Models
{
    // Represents a Pokémon in a Pack, establishing a many-to-many relationship between Pokemon and Pack.
    public class PokemonInPack
    {
        // Primary key for the PokemonInPack entity.
        [Key]
        public int Id { get; set; }

        // Foreign key to the Pack entity, required.
        [Required]
        public int PackId { get; set; }

        // Foreign key to the Pokemon entity, required.
        [Required]
        public int PokemonId { get; set; }

        // The date when the Pokémon was added to the pack, required.
        [Required]
        public DateTime CreatedOn { get; set; }

        // The date when the relationship was last updated, optional.
        public DateTime? UpdatedOn { get; set; }

        // Navigation property for the associated Pack (many-to-one relationship).
        [ForeignKey("PackId")]
        public virtual Pack Pack { get; set; }

        // Navigation property for the associated Pokemon (many-to-one relationship).
        [ForeignKey("PokemonId")]
        public virtual Pokemon Pokemon { get; set; }
    }
}

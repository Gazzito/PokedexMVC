using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokedexMVC.Models
{
    public class PokemonInPack
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackId { get; set; }

        [Required]
        public int PokemonId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        // Navigation properties
        [ForeignKey("PackId")]
        public virtual Pack Pack { get; set; }

        [ForeignKey("PokemonId")]
        public virtual Pokemon Pokemon { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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

        [Required]
        public string CreatedByUserId { get; set; }  // Reference to IdentityUser ID

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedByUserId { get; set; }  // Reference to IdentityUser ID

        [ForeignKey("PackId")]
        public virtual Pack Pack { get; set; }

        [ForeignKey("PokemonId")]
        public virtual Pokemon Pokemon { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }  // Navigation property to IdentityUser

        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }  // Navigation property to IdentityUser
    }
}

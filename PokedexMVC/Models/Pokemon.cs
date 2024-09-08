using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PokedexMVC.Models
{
    public class Pokemon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int RegionId { get; set; }

        [Required]
        public int BaseAttackPoints { get; set; }

        [Required]
        public int BaseHealthPoints { get; set; }

        [Required]
        public int BaseDefensePoints { get; set; }

        [Required]
        public int BaseSpeedPoints { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedByUserId { get; set; }

        public byte[]? Image { get; set; }

        // Navigation properties
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }

        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }

        // Add this collection for the relationship with PokemonInPack
        public virtual ICollection<PokemonInPack> PokemonInPacks { get; set; } = new List<PokemonInPack>();
    }
}

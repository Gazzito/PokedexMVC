using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Microsoft.AspNetCore.Identity; // Import Identity

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
        public string CreatedByUserId { get; set; }  // Reference to IdentityUser ID

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedByUserId { get; set; }  // Reference to IdentityUser ID

        public byte[]? Image { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }

        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }  // Navigation property to IdentityUser

        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }  // Navigation property to IdentityUser
    }
}

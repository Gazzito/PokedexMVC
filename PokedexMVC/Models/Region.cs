using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PokedexMVC.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }  // Reference to IdentityUser ID

        public DateTime? UpdatedOn { get; set; }

        public string? UpdatedByUserId { get; set; }  // Reference to IdentityUser ID

        [ForeignKey("CreatedByUserId")]
        public virtual IdentityUser CreatedByUser { get; set; }  // Navigation property to IdentityUser

        [ForeignKey("UpdatedByUserId")]
        public virtual IdentityUser UpdatedByUser { get; set; }  // Navigation property to IdentityUser
    }
}

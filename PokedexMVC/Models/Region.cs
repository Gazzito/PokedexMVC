using Microsoft.AspNetCore.Identity;
using PokedexMVC.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PokedexMVC.Models { 
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
    public string CreatedByUserId { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedByUserId { get; set; }

    // Navigation properties for IdentityUser
    [ForeignKey("CreatedByUserId")]
    public virtual IdentityUser CreatedByUser { get; set; }

    [ForeignKey("UpdatedByUserId")]
    public virtual IdentityUser UpdatedByUser { get; set; }

    // Add this collection to establish a one-to-many relationship
    public virtual ICollection<Pokemon> Pokemons { get; set; } = new List<Pokemon>();
}
}
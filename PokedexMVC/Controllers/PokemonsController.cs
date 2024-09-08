using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Data;
using PokedexMVC.Models;
using System.Security.Claims;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;


namespace PokedexMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PokemonsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PokemonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pokemons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pokemon.Include(p => p.CreatedByUser).Include(p => p.Region).Include(p => p.UpdatedByUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pokemons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokemon
                .Include(p => p.CreatedByUser)
                .Include(p => p.Region)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }

            return View(pokemon);
        }

        // GET: Pokemons/Create
        public IActionResult Create()
        {
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name");
            return View();
        }

        // POST: Pokemons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,Image")] Pokemon pokemon, IFormFile image)
        {

            // Remove validation errors for server-side properties
            ModelState.Remove(nameof(pokemon.CreatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedByUser));
            ModelState.Remove(nameof(pokemon.UpdatedByUser));
            ModelState.Remove(nameof(pokemon.Region));
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        pokemon.Image = ms.ToArray();
                    }
                }

                pokemon.CreatedOn = DateTime.Now;
                pokemon.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(pokemon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Log validation issues for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }

        // GET: Pokemons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokemon.FindAsync(id);
            if (pokemon == null)
            {
                return NotFound();
            }
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }

        // POST: Pokemons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,Image")] Pokemon pokemon, IFormFile image)
        {
            if (id != pokemon.Id)
            {
                return NotFound();
            }

            // Remove validation errors for server-side properties
            ModelState.Remove(nameof(pokemon.CreatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedByUserId));
            ModelState.Remove(nameof(pokemon.Image));
            ModelState.Remove(nameof(pokemon.Region));
            ModelState.Remove(nameof(pokemon.CreatedByUser));
            ModelState.Remove(nameof(pokemon.UpdatedByUser));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPokemon = await _context.Pokemon.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPokemon == null)
                    {
                        return NotFound();
                    }

                    // Check if a new image is uploaded; if not, retain the existing image
                    if (image != null && image.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await image.CopyToAsync(ms);
                            pokemon.Image = ms.ToArray(); // Save the new image
                        }
                    }
                    else
                    {
                        pokemon.Image = existingPokemon.Image; // Retain the existing image
                    }

                    pokemon.CreatedOn = existingPokemon.CreatedOn; // Retain the original creation date
                    pokemon.CreatedByUserId = existingPokemon.CreatedByUserId; // Retain the original creator
                    pokemon.UpdatedOn = DateTime.Now;
                    pokemon.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    _context.Update(pokemon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PokemonExists(pokemon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }


        // GET: Pokemons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pokemon = await _context.Pokemon
                .Include(p => p.CreatedByUser)
                .Include(p => p.Region)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pokemon == null)
            {
                return NotFound();
            }

            return View(pokemon);
        }

        // POST: Pokemons/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pokemon = await _context.Pokemon.FindAsync(id);
            if (pokemon != null)
            {
                _context.Pokemon.Remove(pokemon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PokemonExists(int id)
        {
            return _context.Pokemon.Any(e => e.Id == id);
        }
    }

}
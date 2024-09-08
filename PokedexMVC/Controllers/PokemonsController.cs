// Required namespaces for controller, data context, model, and authorization.
using Microsoft.AspNetCore.Mvc.Rendering;  // Provides functionalities for creating HTML select elements.
using Microsoft.AspNetCore.Mvc;  // Base classes for MVC pattern controllers and actions.
using Microsoft.EntityFrameworkCore;  // Entity Framework Core for database operations.
using PokedexMVC.Data;  // Includes application's database context.
using PokedexMVC.Models;  // Access to the model classes in PokedexMVC project.
using System.Security.Claims;  // Provides claims-based identity functionalities.
using Microsoft.AspNetCore.Authorization;  // For securing actions and controllers.

namespace PokedexMVC.Controllers
{
    // This controller is restricted to users with the "Admin" role.
    [Authorize(Roles = "Admin")]
    public class PokemonsController : Controller
    {
        // Private field to hold the reference to the database context.
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the database context.
        public PokemonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pokemons
        // Retrieves the list of Pokémon from the database, including related entities like users and regions.
        public async Task<IActionResult> Index()
        {
            // Fetch Pokémon along with the associated users (CreatedBy, UpdatedBy) and Region.
            var applicationDbContext = _context.Pokemon
                .Include(p => p.CreatedByUser)
                .Include(p => p.Region)
                .Include(p => p.UpdatedByUser);

            // Returns the list to the Index view.
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Pokemons/Details/5
        // Shows details of a specific Pokémon.
        public async Task<IActionResult> Details(int? id)
        {
            // If no ID is provided, return a NotFound result.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the Pokémon by ID, including associated entities.
            var pokemon = await _context.Pokemon
                .Include(p => p.CreatedByUser)
                .Include(p => p.Region)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            // If the Pokémon is not found, return NotFound.
            if (pokemon == null)
            {
                return NotFound();
            }

            // Pass the Pokémon details to the view.
            return View(pokemon);
        }

        // GET: Pokemons/Create
        // Displays a form for creating a new Pokémon.
        public IActionResult Create()
        {
            // Populate the Region dropdown list and pass it to the view.
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name");
            return View();
        }

        // POST: Pokemons/Create
        // Handles the form submission for creating a new Pokémon.
        [HttpPost]
        [ValidateAntiForgeryToken]  // CSRF protection.
        public async Task<IActionResult> Create([Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,Image")] Pokemon pokemon, IFormFile image)
        {
            // Remove validation for properties that are set server-side.
            ModelState.Remove(nameof(pokemon.CreatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedByUser));
            ModelState.Remove(nameof(pokemon.UpdatedByUser));
            ModelState.Remove(nameof(pokemon.Region));

            // If model state is valid, proceed with saving the new Pokémon.
            if (ModelState.IsValid)
            {
                // If an image was uploaded, save it as a byte array.
                if (image != null && image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        pokemon.Image = ms.ToArray();  // Save image data in Pokémon model.
                    }
                }

                // Set created and updated metadata.
                pokemon.CreatedOn = DateTime.Now;
                pokemon.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the ID of the logged-in user.

                // Add the new Pokémon to the database.
                _context.Add(pokemon);
                await _context.SaveChangesAsync();

                // Redirect to the Index action after successful creation.
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors for debugging purposes.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            // Repopulate the Region dropdown in case of errors.
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }

        // GET: Pokemons/Edit/5
        // Displays the form to edit an existing Pokémon.
        public async Task<IActionResult> Edit(int? id)
        {
            // If no ID is provided, return a NotFound result.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the Pokémon by its ID.
            var pokemon = await _context.Pokemon.FindAsync(id);
            if (pokemon == null)
            {
                return NotFound();
            }

            // Populate the Region dropdown and pass it to the view.
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }

        // POST: Pokemons/Edit/5
        // Handles the form submission to update an existing Pokémon.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,Image")] Pokemon pokemon, IFormFile image)
        {
            // If the Pokémon ID does not match, return a NotFound result.
            if (id != pokemon.Id)
            {
                return NotFound();
            }

            // Remove validation for properties that are set server-side.
            ModelState.Remove(nameof(pokemon.CreatedByUserId));
            ModelState.Remove(nameof(pokemon.CreatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedOn));
            ModelState.Remove(nameof(pokemon.UpdatedByUserId));
            ModelState.Remove(nameof(pokemon.Image));
            ModelState.Remove(nameof(pokemon.Region));
            ModelState.Remove(nameof(pokemon.CreatedByUser));
            ModelState.Remove(nameof(pokemon.UpdatedByUser));

            // If model state is valid, proceed with updating the Pokémon.
            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing Pokémon without tracking to retain certain fields.
                    var existingPokemon = await _context.Pokemon.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPokemon == null)
                    {
                        return NotFound();
                    }

                    // If a new image is uploaded, save it; otherwise, retain the old image.
                    if (image != null && image.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await image.CopyToAsync(ms);
                            pokemon.Image = ms.ToArray();  // Save the new image.
                        }
                    }
                    else
                    {
                        pokemon.Image = existingPokemon.Image;  // Retain the old image.
                    }

                    // Retain creation metadata and update the updated metadata.
                    pokemon.CreatedOn = existingPokemon.CreatedOn;
                    pokemon.CreatedByUserId = existingPokemon.CreatedByUserId;
                    pokemon.UpdatedOn = DateTime.Now;
                    pokemon.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Update the Pokémon in the database.
                    _context.Update(pokemon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the Pokémon doesn't exist, return NotFound.
                    if (!PokemonExists(pokemon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the Index action after successful update.
                return RedirectToAction(nameof(Index));
            }

            // Log validation errors for debugging purposes.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            // Repopulate the Region dropdown and return to the form with errors.
            ViewData["RegionId"] = new SelectList(_context.Set<PokedexMVC.Models.Region>(), "Id", "Name", pokemon.RegionId);
            return View(pokemon);
        }

        // GET: Pokemons/Delete/5
        // Displays a confirmation page for deleting a Pokémon.
        public async Task<IActionResult> Delete(int? id)
        {
            // If no ID is provided, return NotFound.
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the Pokémon by its ID, including associated entities.
            var pokemon = await _context.Pokemon
                .Include(p => p.CreatedByUser)
                .Include(p => p.Region)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            // If the Pokémon is not found, return NotFound.
            if (pokemon == null)
            {
                return NotFound();
            }

            // Pass the Pokémon details to the view.
            return View(pokemon);
        }

        // POST: Pokemons/Delete/5
        // Handles the form submission to delete a Pokémon.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the Pokémon by its ID.
            var pokemon = await _context.Pokemon.FindAsync(id);

            // If the Pokémon is found, remove it from the database.
            if (pokemon != null)
            {
                _context.Pokemon.Remove(pokemon);
            }

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Redirect to the Index action after successful deletion.
            return RedirectToAction(nameof(Index));
        }

        // Checks if a Pokémon exists by its ID.
        private bool PokemonExists(int id)
        {
            return _context.Pokemon.Any(e => e.Id == id);  // Returns true if the Pokémon exists.
        }
    }
}

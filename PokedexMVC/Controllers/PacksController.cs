// Required namespaces for the controller and other dependencies.
using Microsoft.AspNetCore.Mvc;  // Provides functionality for controllers and action methods.
using Microsoft.AspNetCore.Mvc.Rendering;  // Used for creating HTML elements, such as dropdown lists.
using Microsoft.EntityFrameworkCore;  // Provides Entity Framework Core functionality for working with the database.
using PokedexMVC.Data;  // Provides access to the application's database context.
using PokedexMVC.Models;  // Imports models defined in the PokedexMVC project.
using System.Threading.Tasks;  // Provides asynchronous programming utilities.
using System.Linq;  // Allows for LINQ queries over collections.
using System.Security.Claims;  // Provides functionality for handling user claims.
using Microsoft.AspNetCore.Authorization;  // Provides authorization functionality for securing resources.

namespace PokedexMVC.Controllers
{
    // Controller is restricted to users in the "Admin" role using the [Authorize] attribute.
    [Authorize(Roles = "Admin")]
    public class PacksController : Controller
    {
        // A reference to the application's database context.
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the database context when the controller is created.
        public PacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pack
        // Fetches the list of packs from the database and displays them in the index view.
        public async Task<IActionResult> Index()
        {
            // Include the related CreatedByUser in the query and return the result as a list.
            var packs = await _context.Pack.Include(p => p.CreatedByUser).ToListAsync();
            return View(packs);  // Passes the list of packs to the view.
        }

        // GET: Packs/Details/5
        // Displays the details of a specific pack.
        public async Task<IActionResult> Details(int? id)
        {
            // Check if the id parameter is null.
            if (id == null)
            {
                return NotFound();  // Return a 404 if no id is provided.
            }

            // Fetch the pack and include the list of associated Pokémon.
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)  // Include the Pokémon associated with the pack.
                .ThenInclude(pp => pp.Pokemon)  // Include the Pokémon entity itself.
                .FirstOrDefaultAsync(m => m.Id == id);  // Fetch the pack by its id.

            // Check if the pack is null (i.e., not found).
            if (pack == null)
            {
                return NotFound();
            }

            // Prepare the list of selected Pokémon for display.
            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                PokemonId = p.Pokemon.Id,
                Name = p.Pokemon.Name
            }).ToList();

            return View(pack);  // Pass the pack to the view.
        }

        // GET: Packs/Create
        // Displays the form for creating a new pack.
        public IActionResult Create()
        {
            // Fetch the available Pokémon to populate a dropdown list.
            var availablePokemons = _context.Pokemon.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            // Pass the list of available Pokémon to the view using ViewBag.
            ViewBag.AvailablePokemons = availablePokemons;

            return View();
        }

        // POST: Packs/Create
        // Handles form submission for creating a new pack.
        [HttpPost]
        [ValidateAntiForgeryToken]  // Protects against cross-site request forgery (CSRF) attacks.
        public async Task<IActionResult> Create(Pack pack, IFormFile image, string selectedPokemonIds)
        {
            // Remove validation for fields that are handled manually.
            ModelState.Remove(nameof(pack.CreatedByUserId));
            ModelState.Remove(nameof(pack.CreatedOn));
            ModelState.Remove(nameof(pack.UpdatedOn));
            ModelState.Remove(nameof(pack.UpdatedByUserId));
            ModelState.Remove(nameof(pack.CreatedByUser));
            ModelState.Remove(nameof(pack.UpdatedByUser));
            ModelState.Remove(nameof(pack.Image));
            ModelState.Remove(nameof(image));

            // Check if the model state is valid.
            if (ModelState.IsValid)
            {
                // Handle image upload if an image was provided.
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);  // Copy the image data to memory.
                        pack.Image = memoryStream.ToArray();  // Save the image to the pack model.
                    }
                }

                // Set additional properties for the new pack.
                pack.CreatedOn = DateTime.Now;  // Set the current date and time as the creation time.
                pack.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the user ID of the creator.

                // Add the new pack to the database and save changes.
                _context.Add(pack);
                await _context.SaveChangesAsync();

                // Handle selected Pokémon if any are provided.
                if (!string.IsNullOrEmpty(selectedPokemonIds))
                {
                    var pokemonIds = selectedPokemonIds.Split(',').Select(int.Parse).ToList();  // Convert the comma-separated list of Pokémon IDs into a list of integers.

                    // Add each selected Pokémon to the pack.
                    foreach (var pokemonId in pokemonIds)
                    {
                        _context.PokemonInPacks.Add(new PokemonInPack
                        {
                            PackId = pack.Id,
                            PokemonId = pokemonId,
                            CreatedOn = DateTime.Now
                        });
                    }

                    // Save the Pokémon associations to the database.
                    await _context.SaveChangesAsync();
                }

                // Redirect to the index view after the pack is successfully created.
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, repopulate the available Pokémon for the view.
            var availablePokemons = _context.Pokemon.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            ViewBag.AvailablePokemons = availablePokemons;

            return View(pack);  // Return to the form with validation errors and data.
        }

        // GET: Packs/Edit/5
        // Displays the form for editing an existing pack.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the pack along with its associated Pokémon.
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .ThenInclude(pip => pip.Pokemon)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Get the list of Pokémon already in the pack.
            var selectedPokemonIds = pack.PokemonInPacks.Select(p => p.PokemonId).ToList();

            // Get the list of available Pokémon excluding those already in the pack.
            var availablePokemons = _context.Pokemon
                .Where(p => !selectedPokemonIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToList();

            // Populate ViewBag with selected and available Pokémon.
            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                p.PokemonId,
                p.Pokemon.Name
            }).ToList();
            ViewBag.AvailablePokemons = new SelectList(availablePokemons, "Id", "Name");

            return View(pack);
        }

        // POST: Packs/Edit/5
        // Handles form submission for editing an existing pack.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pack pack, IFormFile image, string selectedPokemonIds)
        {
            if (id != pack.Id)
            {
                return NotFound();
            }

            // Remove validation for fields that are handled manually.
            ModelState.Remove(nameof(pack.CreatedByUserId));
            ModelState.Remove(nameof(pack.CreatedOn));
            ModelState.Remove(nameof(pack.UpdatedOn));
            ModelState.Remove(nameof(pack.UpdatedByUserId));
            ModelState.Remove(nameof(pack.CreatedByUser));
            ModelState.Remove(nameof(pack.UpdatedByUser));
            ModelState.Remove(nameof(pack.Image));
            ModelState.Remove(nameof(selectedPokemonIds));
            ModelState.Remove(nameof(image));

            // If the model state is valid, continue with the edit operation.
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing pack from the database to retain important fields.
                    var existingPack = await _context.Pack.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPack == null)
                    {
                        return NotFound();
                    }

                    // Retain original pack metadata.
                    pack.CreatedByUserId = existingPack.CreatedByUserId;
                    pack.CreatedOn = existingPack.CreatedOn;
                    pack.CreatedByUser = existingPack.CreatedByUser;

                    // Handle image upload if a new image is provided, otherwise keep the existing image.
                    if (image != null && image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            pack.Image = memoryStream.ToArray();  // Save the new image.
                        }
                    }
                    else
                    {
                        pack.Image = existingPack.Image;  // Keep the existing image.
                    }

                    // Update metadata for last update time and user.
                    pack.UpdatedOn = DateTime.Now;
                    pack.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Update the pack in the database.
                    _context.Update(pack);
                    await _context.SaveChangesAsync();

                    // Update the Pokémon associated with the pack.
                    // First, remove existing Pokémon associations.
                    var existingPokemonInPack = _context.PokemonInPacks.Where(p => p.PackId == id).ToList();
                    _context.PokemonInPacks.RemoveRange(existingPokemonInPack);

                    // Add the newly selected Pokémon if any are provided.
                    if (!string.IsNullOrEmpty(selectedPokemonIds))
                    {
                        var pokemonIds = selectedPokemonIds.Split(',').Select(int.Parse).ToList();

                        foreach (var pokemonId in pokemonIds)
                        {
                            _context.PokemonInPacks.Add(new PokemonInPack
                            {
                                PackId = pack.Id,
                                PokemonId = pokemonId,
                                CreatedOn = DateTime.Now
                            });
                        }
                    }

                    await _context.SaveChangesAsync();  // Save changes to the database.
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If there is a concurrency issue, check if the pack still exists.
                    if (!PackExists(pack.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect to the index view after a successful update.
            }

            // If validation fails, repopulate Pokémon data and show errors.
            var selectedPokemonIdsList = new List<int>();
            if (!string.IsNullOrEmpty(selectedPokemonIds))
            {
                selectedPokemonIdsList = selectedPokemonIds.Split(',').Select(int.Parse).ToList();
            }

            // Get the Pokémon already selected and those available for selection.
            var selectedPokemons = _context.Pokemon
                .Where(p => selectedPokemonIdsList.Contains(p.Id))
                .Select(p => new
                {
                    PokemonId = p.Id,
                    Name = p.Name
                })
                .ToList();

            var availablePokemons = _context.Pokemon
                .Where(p => !selectedPokemonIdsList.Contains(p.Id))
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();

            // Log validation errors for debugging purposes.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            // Repopulate ViewBag with the available and selected Pokémon.
            ViewBag.AvailablePokemons = availablePokemons;
            ViewBag.SelectedPokemons = selectedPokemons;

            return View(pack);  // Return to the form with validation errors and data.
        }

        // GET: Packs/Delete/5
        // Displays the confirmation page for deleting a pack.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the pack and its associated Pokémon (if needed for display).
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .ThenInclude(pp => pp.Pokemon)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Prepare the list of Pokémon associated with the pack for display.
            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                PokemonId = p.Pokemon.Id,
                Name = p.Pokemon.Name
            }).ToList();

            return View(pack);  // Display the confirmation view.
        }

        // POST: Packs/Delete/5
        // Handles form submission to confirm the deletion of a pack.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Fetch the pack and its associated Pokémon.
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Remove the Pokémon associations before deleting the pack.
            _context.PokemonInPacks.RemoveRange(pack.PokemonInPacks);

            // Remove the pack itself from the database.
            _context.Pack.Remove(pack);

            // Save the changes to the database.
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));  // Redirect to the index view after successful deletion.
        }

        // Check if a pack exists by its id.
        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);  // Returns true if a pack with the given id exists.
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Data;
using PokedexMVC.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace PokedexMVC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class PacksController : Controller
    {

        private readonly ApplicationDbContext _context;

        public PacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pack
        public async Task<IActionResult> Index()
        {
            var packs = await _context.Pack.Include(p => p.CreatedByUser).ToListAsync();
            return View(packs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Get the pack including the list of associated Pokémon
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .ThenInclude(pp => pp.Pokemon) // Assuming there's a navigation property to the Pokemon in your model
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Prepare the list of selected Pokémon for the ViewBag
            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                PokemonId = p.Pokemon.Id,
                Name = p.Pokemon.Name
            }).ToList();

            return View(pack);
        }

        // GET: Packs/Create
        // GET: Packs/Create
        public IActionResult Create()
        {
            // Fetch the available Pokémon and pass them to the view using ViewBag
            var availablePokemons = _context.Pokemon.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            ViewBag.AvailablePokemons = availablePokemons;

            return View();
        }

        // POST: Packs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pack pack, IFormFile image, string selectedPokemonIds)
        {
            // Remove validation for properties we handle manually
            ModelState.Remove(nameof(pack.CreatedByUserId));
            ModelState.Remove(nameof(pack.CreatedOn));
            ModelState.Remove(nameof(pack.UpdatedOn));
            ModelState.Remove(nameof(pack.UpdatedByUserId));
            ModelState.Remove(nameof(pack.CreatedByUser));
            ModelState.Remove(nameof(pack.UpdatedByUser));
            ModelState.Remove(nameof(pack.Image));
            ModelState.Remove(nameof(image));

            if (ModelState.IsValid)
            {
                // Handle Image Upload
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        pack.Image = memoryStream.ToArray();
                    }
                }

                // Set additional properties
                pack.CreatedOn = DateTime.Now;
                pack.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Save the Pack to the database
                _context.Add(pack);
                await _context.SaveChangesAsync();

                // If there are selected Pokémon, save the relations
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

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // Repopulate the available Pokémon if there's an error
            var availablePokemons = _context.Pokemon.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            ViewBag.AvailablePokemons = availablePokemons;

            return View(pack);
        }

        // GET: Packs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .ThenInclude(pip => pip.Pokemon)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Get the list of Pokémon already in the pack
            var selectedPokemonIds = pack.PokemonInPacks.Select(p => p.PokemonId).ToList();

            // Get the list of available Pokémon excluding the ones already in the pack
            var availablePokemons = _context.Pokemon
                .Where(p => !selectedPokemonIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToList();

            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                p.PokemonId,
                p.Pokemon.Name
            }).ToList();

            ViewBag.AvailablePokemons = new SelectList(availablePokemons, "Id", "Name");

            return View(pack);
        }

        // POST: Packs/Edit/5
        // POST: Packs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pack pack, IFormFile image, string selectedPokemonIds)
        {
            if (id != pack.Id)
            {
                return NotFound();
            }

            // Remove validation for properties we handle manually
            ModelState.Remove(nameof(pack.CreatedByUserId));
            ModelState.Remove(nameof(pack.CreatedOn));
            ModelState.Remove(nameof(pack.UpdatedOn));
            ModelState.Remove(nameof(pack.UpdatedByUserId));
            ModelState.Remove(nameof(pack.CreatedByUser));
            ModelState.Remove(nameof(pack.UpdatedByUser));
            ModelState.Remove(nameof(pack.Image));
            ModelState.Remove(nameof(selectedPokemonIds));
            ModelState.Remove(nameof(image));

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing pack from the database
                    var existingPack = await _context.Pack.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingPack == null)
                    {
                        return NotFound();
                    }

                    // Retain important fields
                    pack.CreatedByUserId = existingPack.CreatedByUserId;
                    pack.CreatedOn = existingPack.CreatedOn;
                    pack.CreatedByUser = existingPack.CreatedByUser;

                    // Handle Image Upload
                    if (image != null && image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            pack.Image = memoryStream.ToArray();  // Save the new image
                        }
                    }
                    else
                    {
                        // Retain the existing image
                        pack.Image = existingPack.Image;
                    }

                    // Update only the UpdatedOn and UpdatedByUserId fields
                    pack.UpdatedOn = DateTime.Now;
                    pack.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    _context.Update(pack);
                    await _context.SaveChangesAsync();

                    // Update Pokémon In Pack
                    // First, remove existing Pokémon associations
                    var existingPokemonInPack = _context.PokemonInPacks.Where(p => p.PackId == id).ToList();
                    _context.PokemonInPacks.RemoveRange(existingPokemonInPack);

                    // If there are selected Pokémon, add them
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

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackExists(pack.Id))
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

            // Repopulate Pokémon data in case of validation errors
            // Fetch the current list of Pokémon associated with the pack
            var selectedPokemonIdsList = new List<int>();
            if (!string.IsNullOrEmpty(selectedPokemonIds))
            {
                selectedPokemonIdsList = selectedPokemonIds.Split(',').Select(int.Parse).ToList();
            }

            // Get the list of Pokémon already in the pack
            var selectedPokemons = _context.Pokemon
                .Where(p => selectedPokemonIdsList.Contains(p.Id))
                .Select(p => new
                {
                    PokemonId = p.Id,
                    Name = p.Name
                })
                .ToList();

            // Get the list of available Pokémon excluding the ones already selected
            var availablePokemons = _context.Pokemon
                .Where(p => !selectedPokemonIdsList.Contains(p.Id))
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
                .ToList();
            // Log validation issues for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            ViewBag.AvailablePokemons = availablePokemons;
            ViewBag.SelectedPokemons = selectedPokemons;

            return View(pack);
        }



        // GET: Packs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the pack including associated Pokémon (if needed for display)
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks)
                .ThenInclude(pp => pp.Pokemon) // Include related Pokémon for display
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Prepare the list of selected Pokémon for display in the ViewBag (if needed)
            ViewBag.SelectedPokemons = pack.PokemonInPacks.Select(p => new
            {
                PokemonId = p.Pokemon.Id,
                Name = p.Pokemon.Name
            }).ToList();

            return View(pack);
        }

        // POST: Packs/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pack = await _context.Pack
                .Include(p => p.PokemonInPacks) // Include the Pokémon associated with the pack
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            // Remove all associated Pokémon from the pack first (if applicable)
            _context.PokemonInPacks.RemoveRange(pack.PokemonInPacks);

            // Remove the pack itself
            _context.Pack.Remove(pack);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);
        }
    }

}
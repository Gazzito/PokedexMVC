// Required namespaces for controller, data access, and user management functionalities.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;  // For securing controllers and actions.
using Microsoft.AspNetCore.Identity;  // Provides identity functionalities like UserManager.
using Microsoft.AspNetCore.Mvc;  // Base classes for controllers and action methods.
using Microsoft.EntityFrameworkCore;  // For database access using Entity Framework Core.
using PokedexMVC.Data;  // Access to the application's database context.
using PokedexMVC.Models;  // Access to models like Region.

namespace PokedexMVC.Controllers
{
    // Controller restricted to "Admin" role using the [Authorize] attribute.
    [Authorize(Roles = "Admin")]
    public class RegionsController : Controller
    {
        // Private fields for database context and user management.
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to initialize database context and user manager.
        public RegionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Regions
        // Retrieves and displays the list of regions.
        public async Task<IActionResult> Index()
        {
            // Fetch the list of regions from the database.
            var regions = await _context.Region.ToListAsync();

            // For each region, fetch the username of the creator.
            foreach (var region in regions)
            {
                var user = await _userManager.FindByIdAsync(region.CreatedByUserId);  // Fetch the user by CreatedByUserId.
                region.CreatedByUserId = user?.UserName;  // Replace CreatedByUserId with the username for display.
            }

            return View(regions);  // Return the regions list to the view.
        }

        // GET: Regions/Details/5
        // Displays the details of a specific region.
        public async Task<IActionResult> Details(int? id)
        {
            // Check if the id parameter is null.
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the region, including associated CreatedByUser and UpdatedByUser.
            var region = await _context.Region
                .Include(r => r.CreatedByUser)
                .Include(r => r.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            // If the region is not found, return NotFound.
            if (region == null)
            {
                return NotFound();
            }

            return View(region);  // Return the region details to the view.
        }

        // GET: Regions/Create
        // Displays the form for creating a new region.
        public IActionResult Create()
        {
            return View();  // Render the create view.
        }

        // POST: Regions/Create
        // Handles form submission for creating a new region.
        [HttpPost]
        [ValidateAntiForgeryToken]  // Prevent CSRF attacks.
        public async Task<IActionResult> Create([Bind("Id,Name")] Region region)
        {
            Console.WriteLine("im here");

            // Remove validation errors for fields handled by the server.
            ModelState.Remove(nameof(region.CreatedByUserId));
            ModelState.Remove(nameof(region.CreatedOn));
            ModelState.Remove(nameof(region.UpdatedOn));
            ModelState.Remove(nameof(region.UpdatedByUserId));
            ModelState.Remove(nameof(region.CreatedByUser));
            ModelState.Remove(nameof(region.UpdatedByUser));

            // If the model state is valid, continue.
            if (ModelState.IsValid)
            {
                Console.WriteLine("VALID");

                // Set server-side properties.
                region.CreatedOn = DateTime.Now;
                region.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the ID of the logged-in user.
                region.UpdatedOn = null;
                region.UpdatedByUserId = null;

                // Add the new region to the database.
                _context.Add(region);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));  // Redirect to the index page after saving.
            }

            // Log validation errors for debugging.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            return View(region);  // Return to the view with validation errors.
        }

        // GET: Regions/Edit/5
        // Displays the form to edit an existing region.
        public async Task<IActionResult> Edit(int? id)
        {
            // Check if the id parameter is null.
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the region by its ID.
            var region = await _context.Region.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            return View(region);  // Render the edit view.
        }

        // POST: Regions/Edit/5
        // Handles form submission to update an existing region.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Region region)
        {
            // Check if the region ID from the form matches the ID in the URL.
            if (id != region.Id)
            {
                return NotFound();
            }

            // Remove validation errors for fields handled by the server.
            ModelState.Remove(nameof(region.CreatedByUserId));
            ModelState.Remove(nameof(region.CreatedOn));
            ModelState.Remove(nameof(region.UpdatedOn));
            ModelState.Remove(nameof(region.UpdatedByUserId));
            ModelState.Remove(nameof(region.CreatedByUser));
            ModelState.Remove(nameof(region.UpdatedByUser));

            // If the model state is valid, continue.
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the existing region to preserve original creation data.
                    var existingRegion = await _context.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (existingRegion == null)
                    {
                        return NotFound();
                    }

                    // Preserve the CreatedByUserId and CreatedOn fields.
                    region.CreatedByUserId = existingRegion.CreatedByUserId;
                    region.CreatedOn = existingRegion.CreatedOn;

                    // Set the updated metadata.
                    region.UpdatedOn = DateTime.Now;
                    region.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);  // Get the logged-in user ID.

                    // Update the region in the database.
                    _context.Update(region);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If a concurrency issue occurs, check if the region still exists.
                    if (!RegionExists(region.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));  // Redirect to the index page after saving.
            }

            // Log validation errors for debugging.
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            return View(region);  // Return to the view with validation errors.
        }

        // GET: Regions/Delete/5
        // Displays the confirmation page for deleting a region.
        public async Task<IActionResult> Delete(int? id)
        {
            // Check if the id parameter is null.
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the region by its ID, including related CreatedByUser and UpdatedByUser.
            var region = await _context.Region
                .Include(r => r.CreatedByUser)
                .Include(r => r.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            // If the region is not found, return NotFound.
            if (region == null)
            {
                return NotFound();
            }

            return View(region);  // Return the delete confirmation view.
        }

        // POST: Regions/Delete/5
        // Handles form submission to confirm the deletion of a region.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find the region by its ID.
            var region = await _context.Region.FindAsync(id);

            // If the region exists, remove it from the database.
            if (region != null)
            {
                _context.Region.Remove(region);
            }

            await _context.SaveChangesAsync();  // Save the changes to the database.

            return RedirectToAction(nameof(Index));  // Redirect to the index page after deletion.
        }

        // Helper method to check if a region exists by its ID.
        private bool RegionExists(int id)
        {
            return _context.Region.Any(e => e.Id == id);  // Returns true if a region with the specified ID exists.
        }
    }
}

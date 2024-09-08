using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Data;
using PokedexMVC.Models;

namespace PokedexMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RegionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RegionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Regions
        public async Task<IActionResult> Index()
        {
            var regions = await _context.Region.ToListAsync();

            foreach (var region in regions)
            {
                // Fetch the username for CreatedByUserId
                var user = await _userManager.FindByIdAsync(region.CreatedByUserId);
                region.CreatedByUserId = user?.UserName; // Use UserName or Email or any other field you prefer
            }

            return View(regions);
        }
        // GET: Regions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region
                .Include(r => r.CreatedByUser)
                .Include(r => r.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        // GET: Regions/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Region region)
        {
            Console.WriteLine("im here");

         

            // Remove validation errors for server-side properties
            ModelState.Remove(nameof(region.CreatedByUserId));
            ModelState.Remove(nameof(region.CreatedOn));
            ModelState.Remove(nameof(region.UpdatedOn));
            ModelState.Remove(nameof(region.UpdatedByUserId));
            ModelState.Remove(nameof(region.CreatedByUser));
            ModelState.Remove(nameof(region.UpdatedByUser));

            // Now the ModelState will be valid if only the fields from the form are checked
            if (ModelState.IsValid)
            {
                Console.WriteLine("VALID");
                // Set server-side properties
                region.CreatedOn = DateTime.Now;
                region.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                region.UpdatedOn = null;
                region.UpdatedByUserId = null;
                _context.Add(region);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Log validation issues for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));

            return View(region);
        }



        // GET: Regions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Regions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Region region)
        {
            if (id != region.Id)
            {
                return NotFound();
            }

            // Remove validation errors for server-side properties
            ModelState.Remove(nameof(region.CreatedByUserId));
            ModelState.Remove(nameof(region.CreatedOn));
            ModelState.Remove(nameof(region.UpdatedOn));
            ModelState.Remove(nameof(region.UpdatedByUserId));
            ModelState.Remove(nameof(region.CreatedByUser));
            ModelState.Remove(nameof(region.UpdatedByUser));

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing region to keep the original CreatedByUserId and CreatedOn
                    var existingRegion = await _context.Region.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                    if (existingRegion == null)
                    {
                        return NotFound();
                    }

                    // Preserve CreatedByUserId and CreatedOn
                    region.CreatedByUserId = existingRegion.CreatedByUserId;
                    region.CreatedOn = existingRegion.CreatedOn;

                    // Set updated fields
                    region.UpdatedOn = DateTime.Now;
                    region.UpdatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    // Update the region in the database
                    _context.Update(region);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegionExists(region.Id))
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

            // Log validation issues for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.WriteLine("Validation Errors: " + string.Join(", ", errors));
            return View(region);
        }

        // GET: Regions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Region
                .Include(r => r.CreatedByUser) // Include the CreatedByUser information
                .Include(r => r.UpdatedByUser) // Include the UpdatedByUser information
                .FirstOrDefaultAsync(m => m.Id == id);

            if (region == null)
            {
                return NotFound();
            }

            return View(region);
        }

        // POST: Regions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var region = await _context.Region.FindAsync(id);
            if (region != null)
            {
                _context.Region.Remove(region);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegionExists(int id)
        {
            return _context.Region.Any(e => e.Id == id);
        }
    }
}

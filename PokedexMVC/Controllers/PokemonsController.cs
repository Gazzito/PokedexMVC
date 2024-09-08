using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PokedexMVC.Data;
using PokedexMVC.Models;

namespace PokedexMVC.Controllers
{
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
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["RegionId"] = new SelectList(_context.Set<Region>(), "Id", "CreatedByUserId");
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Pokemons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,CreatedOn,CreatedByUserId,UpdatedOn,UpdatedByUserId,Image")] Pokemon pokemon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pokemon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.CreatedByUserId);
            ViewData["RegionId"] = new SelectList(_context.Set<Region>(), "Id", "CreatedByUserId", pokemon.RegionId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.UpdatedByUserId);
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
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.CreatedByUserId);
            ViewData["RegionId"] = new SelectList(_context.Set<Region>(), "Id", "CreatedByUserId", pokemon.RegionId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.UpdatedByUserId);
            return View(pokemon);
        }

        // POST: Pokemons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RegionId,BaseAttackPoints,BaseHealthPoints,BaseDefensePoints,BaseSpeedPoints,CreatedOn,CreatedByUserId,UpdatedOn,UpdatedByUserId,Image")] Pokemon pokemon)
        {
            if (id != pokemon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.CreatedByUserId);
            ViewData["RegionId"] = new SelectList(_context.Set<Region>(), "Id", "CreatedByUserId", pokemon.RegionId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pokemon.UpdatedByUserId);
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
        [HttpPost, ActionName("Delete")]
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

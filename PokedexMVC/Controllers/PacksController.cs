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
    public class PacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Packs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Pack.Include(p => p.CreatedByUser).Include(p => p.UpdatedByUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Packs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack
                .Include(p => p.CreatedByUser)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pack == null)
            {
                return NotFound();
            }

            return View(pack);
        }

        // GET: Packs/Create
        public IActionResult Create()
        {
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Packs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Image,BronzeChance,SilverChance,GoldChance,PlatinumChance,DiamondChance,TotalBought,CreatedOn,CreatedByUserId,UpdatedOn,UpdatedByUserId")] Pack pack)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pack);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.UpdatedByUserId);
            return View(pack);
        }

        // GET: Packs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack.FindAsync(id);
            if (pack == null)
            {
                return NotFound();
            }
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.UpdatedByUserId);
            return View(pack);
        }

        // POST: Packs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Image,BronzeChance,SilverChance,GoldChance,PlatinumChance,DiamondChance,TotalBought,CreatedOn,CreatedByUserId,UpdatedOn,UpdatedByUserId")] Pack pack)
        {
            if (id != pack.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pack);
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
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "Id", pack.UpdatedByUserId);
            return View(pack);
        }

        // GET: Packs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pack = await _context.Pack
                .Include(p => p.CreatedByUser)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pack == null)
            {
                return NotFound();
            }

            return View(pack);
        }

        // POST: Packs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pack = await _context.Pack.FindAsync(id);
            if (pack != null)
            {
                _context.Pack.Remove(pack);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PackExists(int id)
        {
            return _context.Pack.Any(e => e.Id == id);
        }
    }
}

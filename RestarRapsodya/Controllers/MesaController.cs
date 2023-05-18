using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using RestarRapsodya.Data;
using RestarRapsodya.Models;

namespace RestarRapsodya.Controllers
{
    public class MesaController : Controller
    {
        private readonly RestarRapsodyaContext _context;

        public MesaController(RestarRapsodyaContext context)
        {
            _context = context;
        }

        // GET: Mesa
        public async Task<IActionResult> Consultar()
        {
            
            return _context.Mesa != null ? 
                          View(await _context.Mesa.Include(m => m.Estado).ToListAsync()) :
                          Problem("Entity set 'RestarRapsodyaContext.Mesa'  is null.");
        }

        // GET: Mesa/Details/5
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null || _context.Mesa == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesa.Include(m => m.Estado).FirstOrDefaultAsync(m => m.id_Mesa == id);

            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // GET: Mesa/Create
        public IActionResult Crear()
        {
            ViewData["Estados"] = new SelectList(_context.Estado, "id_Estado", "estado");
            return View();
        }

        // POST: Mesa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("id_Mesa,Capacidad,id_Estado")] Mesa mesa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mesa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Consultar));
            }
            ViewData["Estados"] = new SelectList(_context.Estado, "id_Estado", "estado");
            return View(mesa);
        }

        // GET: Mesa/Edit/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null || _context.Mesa == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesa.Include(m => m.Estado).FirstOrDefaultAsync(m=> m.id_Mesa == id);
            if (mesa == null)
            {
                return NotFound();
            }
            ViewData["Estados"] = new SelectList(_context.Estado, "id_Estado", "estado");
            return View(mesa);
        }

        // POST: Mesa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("id_Mesa,Capacidad,id_Estado")] Mesa mesa)
        {
            if (id != mesa.id_Mesa)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mesa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MesaExists(mesa.id_Mesa))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Consultar));
            }
            ViewData["Estados"] = new SelectList(_context.Estado, "id_Estado", "estado");
            return View(mesa);
        }

        // GET: Mesa/Delete/5
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null || _context.Mesa == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesa
                .FirstOrDefaultAsync(m => m.id_Mesa == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // POST: Mesa/Delete/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmed(int id)
        {
            if (_context.Mesa == null)
            {
                return Problem("Entity set 'RestarRapsodyaContext.Mesa'  is null.");
            }
            var mesa = await _context.Mesa.FindAsync(id);
            if (mesa != null)
            {
                _context.Mesa.Remove(mesa);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Consultar));
        }

        private bool MesaExists(int id)
        {
          return (_context.Mesa?.Any(e => e.id_Mesa == id)).GetValueOrDefault();
        }
    }
}

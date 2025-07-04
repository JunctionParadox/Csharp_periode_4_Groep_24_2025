using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Csharp_periode_4_Groep_24_2025.Models;
using Csharp_periode_4_Groep_24_2025.Data;
using Microsoft.EntityFrameworkCore;
using Csharp_periode_4_Groep_24_2025.Models.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    public class EnclosureController : Controller
    {
        private readonly DbContext24 _context;
        private readonly ILogger<EnclosureController> _logger;

        public EnclosureController(DbContext24 context, ILogger<EnclosureController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            var result = _context.Enclosure.Include(z => z.Animals);
            return View(await result.ToListAsync());
        }

        [HttpGet("/api/[controller]/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("/api/[controller]/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Create(Enclosure enclosure)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(enclosure);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
            }
            return View(enclosure);
        }

        [HttpGet("api/[controller]/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosure
                    .Include(e => e.Animals)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (enclosure == null)
            {
                return NotFound();
            }

            return View(enclosure);
        }

        [HttpPost("api/[controller]/{id:int}")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Edit(int? id, [Bind("Name", "ClimateClass", "HabitatType", "Security", "Size")] Enclosure enclosure)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Enclosure.FirstOrDefaultAsync(z => z.Id == id);
            if (await TryUpdateModelAsync<Enclosure>(target, "", z => z.Name, z => z.ClimateClass, z => z.HabitatType, z => z.Security, z => z.Size))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Failed to update");
                }
            }
            return RedirectToAction("Details", new { id });

        }

        [HttpDelete("api/[controller]/{id:int}")]
        public async Task<IActionResult> Delete (int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enclosure = await _context.Enclosure
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id);
            if (enclosure == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["Errormessage"] = "Failed to delete";
            }

            return View(enclosure);
        }
    }
}

using Csharp_periode_4_Groep_24_2025.Data;
using Csharp_periode_4_Groep_24_2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    public class AnimalController : Controller
    {
        private readonly DbContext24 _context;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(DbContext24 context, ILogger<AnimalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            var result = _context.Animal.Include(z => z.Enclosure)
                .Include(z => z.Category);
            return View(await result.ToListAsync());
        }

        [HttpGet("api/[controller]/create")]
        public IActionResult Create()
        {
            PopulateCategoryDropDownList();
            PopulateEnclosureDropDownList();
            return View();
        }

        [HttpPost("api/[controller]/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Create(Animal animal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(animal);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
            }
            return View(animal);
        }

        [HttpGet("api/[controller]/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                 .Include(s => s.Enclosure)
                 .Include(e => e.Category)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        [HttpGet("api/[controller]/{id:int}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animal
                 .Include(s => s.Enclosure)
                 .Include(e => e.Category)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null)
            {
                return NotFound();
            }
            PopulateCategoryDropDownList();
            PopulateEnclosureDropDownList();
            return View(animal);
        }

        [HttpPost("api/[controller]/{id:int}/edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Edit(int? id, [Bind("Name", "Species", "CategoryId", "SizeClass", "Diet", "Activity", "Prey", "EnclosureId", "SpaceRequirement", "SecurityRequirement")] Animal animal)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Animal.FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (await TryUpdateModelAsync<Animal>(target, "", z => z.Name, z => z.Species, z => z.CategoryId, z => z.SizeClass, z => z.Diet, z => z.Activity, z => z.Prey, z => z.EnclosureId, z => z.SpaceRequirement, z => z.SecurityRequirement))
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

        [HttpGet("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var target = await _context.Animal
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (target == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(target);
        }

        [HttpPost("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> ExecuteDeletion(int? id, bool? saveChangesError = false)
        {
            var target = await _context.Animal.FindAsync(id);
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Animal.Remove(target);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["Errormessage"] = "Failed to delete";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateCategoryDropDownList(object? selectedCategory = null)
        {
            var categoryQuery = _context.Category
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.CategoryId = new SelectList(categoryQuery, "Id", "Name", selectedCategory);
        }

        private void PopulateEnclosureDropDownList(object? selectedEnclosure = null)
        {
            var enclosureQuery = _context.Enclosure
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.EnclosureId = new SelectList(enclosureQuery, "Id", "Name", selectedEnclosure);
        }

    }
}

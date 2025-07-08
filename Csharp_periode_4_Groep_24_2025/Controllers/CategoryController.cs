using Csharp_periode_4_Groep_24_2025.Data;
using Csharp_periode_4_Groep_24_2025.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DbContext24 _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(DbContext24 context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]")]
        public async Task<IActionResult> Index()
        {
            var result = _context.Category.Include(z => z.Animals);
            return View(await result.ToListAsync());
        }

        [HttpGet("/api/[controller]/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("/api/[controller]/create")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Failed to save changes.");
            }
            return View(category);
        }

        [HttpGet("api/[controller]/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                    .Include(e => e.Animals)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpGet("api/[controller]/{id:int}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                 .AsNoTracking()
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            PopulateAnimalDropDownList();
            return View(category);
        }

        [HttpPost("api/[controller]/{id:int}/edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Edit(int? id, string? name, int? animalId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var target = await _context.Category.Include(z => z.Animals).FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (name != null)
            {
                target.Name = name;
            }
            var animal = await _context.Animal.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound("Animal not found");
            }
            if (!target.Animals.Contains(animal))
            {
                target.Animals.Add(animal);
            }
            if (await TryUpdateModelAsync(target, "", z => z.Name, z => z.Animals))
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
            var target = await _context.Category
                .AsNoTracking()
                .FirstOrDefaultAsync(z => z.Id == id);
            if (target == null)
            {
                return NotFound();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Please try again";
            }

            return View(target);
        }

        [HttpPost("api/[controller]/{id:int}/delete")]
        public async Task<IActionResult> ExecuteDeletion(int? id, bool? saveChangesError = false)
        {
            var target = await _context.Category.FindAsync(id);
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Category.Remove(target);
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

        private void PopulateAnimalDropDownList(object? selectedAnimal = null)
        {
            var animalQuery = _context.Animal
                                    .OrderBy(d => d.Name)
                                    .AsNoTracking()
                                    .ToList();

            ViewBag.AnimalId = new SelectList(animalQuery, "Id", "Name", selectedAnimal);
        }
    }
}

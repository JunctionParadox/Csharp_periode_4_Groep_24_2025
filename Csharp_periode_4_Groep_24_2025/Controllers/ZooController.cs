﻿using Csharp_periode_4_Groep_24_2025.Data;
using Csharp_periode_4_Groep_24_2025.Data.Enum;
using Csharp_periode_4_Groep_24_2025.Models;
using Csharp_periode_4_Groep_24_2025.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Csharp_periode_4_Groep_24_2025.Controllers
{
    //Het idee is dat zoo controllers globale functies kunnen aansturen
    //En dus niet gekoppeld zijn aan een object zelf
    //Zoo functies zijn ook allemaal beschikbaar vanuit home page
    public class ZooController : Controller, ICheckConstraints, IDayNightCycle, IFeedingTime
    {
        private readonly DbContext24 _context;
        private readonly ILogger<ZooController> _logger;

        public ZooController(DbContext24 context, ILogger<ZooController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("api/[controller]/sunset")]
        //De functie stelt een lijst van commentaar op voor elke animal die in de database bestaat
        public async Task<IActionResult> Sunset(int? id = null)
        {
            var result = _context.Animal.ToList();
            List<string> animalList = new List<string>();
            foreach (var animal in result)
            {
                if (animal.Activity == ActivityPattern.Diurnal)
                {
                    animalList.Add($"{animal.Name} is going to sleep.");
                }
                else if (animal.Activity == ActivityPattern.Nocturnal)
                {
                    animalList.Add($"{animal.Name} is going to awake from their slumber.");
                }
                else if (animal.Activity == ActivityPattern.Cathemeral)
                {
                    animalList.Add($"{animal.Name} is already awake.");
                }
                else
                {
                    animalList.Add($"No info could be found about the activity pattern of {animal.Name}, consider adding some in editing panel.");
                }
            }
            return View(animalList);
        }

        [HttpGet("api/[controller]/sunrise")]
        //De functie stelt een lijst van commentaar op voor elke animal die in de database bestaat
        public async Task<IActionResult> Sunrise(int? id = null)
        {
            var result = _context.Animal.ToList();
            List<string> animalList = new List<string>();
            foreach (var animal in result)
            {
                if (animal.Activity == ActivityPattern.Nocturnal)
                {
                    animalList.Add($"{animal.Name} is going to sleep.");
                }
                else if (animal.Activity == ActivityPattern.Diurnal)
                {
                    animalList.Add($"{animal.Name} is going to awake from their slumber.");
                }
                else if (animal.Activity == ActivityPattern.Cathemeral)
                {
                    animalList.Add($"{animal.Name} is already awake.");
                }
                else
                {
                    animalList.Add($"No info could be found about the activity pattern of {animal.Name}, consider adding some in editing panel.");
                }
            }
            return View(animalList);
        }

        [HttpGet("api/[controller]/feedingtime")]
        //De functie stelt een lijst van commentaar op voor elke animal die in de database bestaat
        public async Task<IActionResult> FeedingTime(int? id = null)
        {
            var result = _context.Animal.ToList();
            List<string> animalList = new List<string>();
            foreach (var animal in result)
            {
                if (animal.Diet == DietaryClass.Herbivore)
                {
                    animalList.Add($"{animal.Name} is a herbivore, they like to eat plants.");
                }
                else if (animal.Diet != DietaryClass.None && animal.Prey != null)
                {
                    animalList.Insert(0, $"{animal.Name} is a {animal.Diet}, they like to eat {animal.Prey}.");
                }
                else if (animal.Diet != DietaryClass.None)
                {
                    animalList.Insert(0, $"{animal.Name} is a {animal.Diet}, but we do not know their prey.");
                }
                else if (animal.Prey != null)
                {
                    animalList.Add($"{animal.Name} likes to eat {animal.Prey}.");
                }
                else
                {
                    animalList.Add($"{animal.Name} has no known diet or prey, please consider adding more info.");
                }
            }
            return View(animalList);
        }

        [HttpGet("api/[controller]/automatic")]
        public async Task<IActionResult> AutoAssign()
        {
            return View();
        }

        [HttpPost("api/[controller]/automatic")]
        //Checked of de gebruiker het vinkje heeft aangechecked dat alle bestaande enclosures vernietigd
        public async Task<IActionResult> AutoAssign(string reset)
        {
            if (reset == "on")
            {
                AutoAssignReset();
            }
            else
            {
                AutoAssignDefault();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("api/[controller]/check")]
        //Voor elke animal wordt de funcite CheckAnimalConstraints() invoked
        //Hierbij wordt een meegegeven lijst gepopuleerd met commentaar
        public async Task<IActionResult> CheckConstraints(int? id = null)
        {
            var result = _context.Animal.Include(z => z.Enclosure)
                                        .ToListAsync();
            List<string> commentary = new List<string>();
            foreach (var animal in await result)
            {
                if(animal.EnclosureId != null)
                {
                    animal.CheckAnimalConstraints(commentary, animal.Enclosure);
                }
                else
                {
                    animal.CheckAnimalConstraints(commentary, null);
                }
            }
            return View(commentary);
        }

        //De functie checked of een dier al een enclosure heeft
        //Zo nee, wordt een bestaande enclosure toegewezen
        //In het geval er niet genoeg enclosures zijn wordt een nieuwe aangemaakt
        private async void AutoAssignDefault()
        {
            var animals = _context.Animal.Include(z => z.Enclosure)
                                            .ToList();
            var enclosures = _context.Enclosure.ToList();
            HashSet<Enclosure> occupied = new HashSet<Enclosure>();
            int count = 0;
            foreach (var animal in  animals)
            {
                if (animal.Enclosure != null)
                {
                    occupied.Add(animal.Enclosure);
                }
                else
                {
                    foreach (var enclosure in enclosures)
                    {
                        if (!occupied.Contains(enclosure))
                        {
                            enclosure.Animals.Add(animal);
                            occupied.Add(enclosure);
                            break;
                        }
                    }
                    if (animal.Enclosure == null)
                    {
                        var replacement = new Enclosure { Name = $"Placeholder {count}", ClimateClass = Climate.None, HabitatType = HabitatTypes.None, Security = animal.SecurityRequirement, Size = animal.SpaceRequirement };
                        _context.Enclosure.Add(replacement);
                        replacement.Animals.Add(animal);
                        occupied.Add(replacement);
                        count++;

                    }
                }
            }
            try {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Could not complete task");
            }
        }

        //De functie gebruikt een queue om een voor een alle bestaande queues te vernietigen
        //Door Dequeue() onstaat er geen complicaties met een verwijderde items in een collectie hebben
        //Zoals ik bij een foreach() implementatie ervaarde
        //Daarna creerd die een nieuwe enclosure voor elke animal
        private async void AutoAssignReset()
        {
            var enclosures = _context.Enclosure.Include(z => z.Animals).ToList();
            Queue<Enclosure> oldEnclosures = new Queue<Enclosure>();
            foreach (var enclosure in enclosures)
            {
                oldEnclosures.Enqueue(enclosure);
            }
            while (!oldEnclosures.IsNullOrEmpty())
            {
                _context.Enclosure.Remove(oldEnclosures.Dequeue());
                _context.SaveChanges();
            }
            int count = 0;
            var animals = _context.Animal.ToList();
            foreach (var animal in animals)
            {
                var replacement = new Enclosure { Name = $"Placeholder {count}", ClimateClass = Climate.Arctic, HabitatType = HabitatTypes.None, Security = animal.SecurityRequirement, Size = animal.SpaceRequirement };
                _context.Enclosure.Add(replacement);
                replacement.Animals.Add(animal);
                count++;
                _context.SaveChanges();
            }
        }
    }
}

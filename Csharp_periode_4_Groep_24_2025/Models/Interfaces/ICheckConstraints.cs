﻿using Microsoft.AspNetCore.Mvc;

namespace Csharp_periode_4_Groep_24_2025.Models.Interfaces
{
    //Niet te verwaren met CheckConstraint, een ingebouwde functie
    public interface ICheckConstraints
    {
        public Task<IActionResult> CheckConstraints(int? id);
    }
}

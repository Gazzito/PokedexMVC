<<<<<<< HEAD
using Microsoft.AspNetCore.Authorization;
=======
﻿using Microsoft.AspNetCore.Authorization;
>>>>>>> fbb1070 (Added Admin Dashboard with role-based authorization)
using Microsoft.AspNetCore.Mvc;

namespace PokedexMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
<<<<<<< HEAD

=======
>>>>>>> fbb1070 (Added Admin Dashboard with role-based authorization)
}

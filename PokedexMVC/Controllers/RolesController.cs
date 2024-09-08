// Required namespaces for authorization, identity management, MVC functionalities, and database access.
using Microsoft.AspNetCore.Authorization;  // Securing the controller to allow access only to authorized users.
using Microsoft.AspNetCore.Identity;  // Provides identity management for roles and users.
using Microsoft.AspNetCore.Mvc;  // Base classes for MVC controllers and actions.
using Microsoft.EntityFrameworkCore;  // For database access and operations.
using PokedexMVC.Models;  // Access to models like AssignRoleViewModel.
using System.Linq;  // For LINQ operations on collections.
using System.Threading.Tasks;  // For asynchronous programming.

namespace PokedexMVC.Controllers
{
    // This controller is restricted to users in the "Admin" role using the [Authorize] attribute.
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        // Private fields for RoleManager and UserManager to manage roles and users.
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to initialize RoleManager and UserManager.
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: /Roles/Index
        // Displays the list of roles in the system.
        public IActionResult Index()
        {
            // Fetch all roles from the database and pass them to the view.
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: /Roles/Create
        // Displays the form to create a new role.
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        // Handles the form submission to create a new role.
        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            // Ensure the roleName is not null or empty.
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // Create a new role using the RoleManager.
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

                // If creation is successful, redirect to the Index view.
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If there are errors, add them to the ModelState for display.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View();
        }

        // GET: /Roles/Edit/{id}
        // Displays the form to edit an existing role.
        public async Task<IActionResult> Edit(string id)
        {
            // Find the role by its ID.
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: /Roles/Edit/{id}
        // Handles the form submission to update an existing role.
        [HttpPost]
        public async Task<IActionResult> Edit(string id, IdentityRole model)
        {
            // Find the role by its ID.
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // If the ModelState is valid, update the role.
            if (ModelState.IsValid)
            {
                role.Name = model.Name;  // Update the role name.
                var result = await _roleManager.UpdateAsync(role);

                // If update is successful, redirect to the Index view.
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If there are errors, add them to the ModelState for display.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        // GET: /Roles/Delete/{id}
        // Displays the confirmation page for deleting a role.
        public async Task<IActionResult> Delete(string id)
        {
            // Find the role by its ID.
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: /Roles/DeleteConfirmed
        // Handles the form submission to confirm the deletion of a role.
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Find the role by its ID.
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                // Delete the role using the RoleManager.
                var result = await _roleManager.DeleteAsync(role);

                // If deletion is successful, redirect to the Index view.
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // If there are errors, add them to the ModelState for display.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Roles/Details/{id}
        // Displays the details of a role, including users assigned to it.
        public async Task<IActionResult> Details(string id)
        {
            // Find the role by its ID.
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Get the list of users assigned to this role.
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            ViewBag.RoleName = role.Name;  // Pass the role name to the view.
            return View(usersInRole);  // Pass the list of users in the role to the view.
        }

        // GET: /Roles/AssignRole
        // Displays the form to assign a role to a user.
        public async Task<IActionResult> AssignRole(string userId)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Fetch available roles and roles already assigned to the user.
            var availableRoles = await _roleManager.Roles.ToListAsync();
            var assignedRoles = await _userManager.GetRolesAsync(user);

            // Create a view model to pass to the view.
            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                AvailableRoles = availableRoles.Select(r => r.Name).ToList(),  // List of available roles.
                AssignedRoles = assignedRoles.ToList()  // List of roles already assigned to the user.
            };

            return View(model);
        }

        // POST: /Roles/AssignRole
        // Handles the form submission to assign a role to a user.
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is already in the role.
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                ModelState.AddModelError(string.Empty, $"User is already in role {roleName}");
            }
            else
            {
                // Assign the role to the user.
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    // If there are errors, add them to the ModelState for display.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction(nameof(AssignRole), new { userId = userId });
        }

        // POST: /Roles/RemoveRole
        // Handles the form submission to remove a role from a user.
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            // Prevent the Admin role from being removed.
            if (roleName == "Admin")
            {
                ModelState.AddModelError(string.Empty, "The Admin role cannot be removed.");
                return RedirectToAction(nameof(AssignRole), new { userId = userId });
            }

            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is in the specified role.
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                // Remove the role from the user.
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(AssignRole), new { userId = userId });
                }
                else
                {
                    // If there are errors, add them to the ModelState for display.
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction(nameof(AssignRole), new { userId = userId });
        }
    }
}

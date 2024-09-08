// Required namespaces for identity, authentication, authorization, and MVC functionalities.
using Microsoft.AspNetCore.Authentication.Cookies;  // For cookie-based authentication.
using Microsoft.AspNetCore.Authentication;  // For signing in and out users.
using Microsoft.AspNetCore.Authorization;  // For securing actions based on roles or policies.
using Microsoft.AspNetCore.Identity;  // For managing users and roles in the identity system.
using Microsoft.AspNetCore.Mvc;  // For base controller functionalities and action methods.
using System.Linq;  // For LINQ operations on collections.
using System.Threading.Tasks;  // For asynchronous programming.

namespace PokedexMVC.Controllers
{
    // Controller responsible for managing users.
    public class UsersController : Controller
    {
        // Private field for UserManager, which is used to manage identity users.
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to initialize the UserManager.
        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        // GET: /Users/Index
        // Displays the list of users in the system.
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();  // Fetch all users from the identity system.
            return View(users);  // Return the users list to the view.
        }

        [Authorize(Roles = "Admin")]
        // GET: /Users/Create
        // Displays the form for creating a new user.
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: /Users/Create
        // Handles the form submission to create a new user.
        [HttpPost]
        public async Task<IActionResult> Create(IdentityUser model, string password)
        {
            // Check if the form data is valid.
            if (ModelState.IsValid)
            {
                // Create a new IdentityUser with the provided email and username.
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, password);  // Create the user with the specified password.

                // If the creation is successful, redirect to the Index view.
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
            return View(model);  // Return the model with validation errors if any.
        }

        [Authorize(Roles = "Admin")]
        // GET: /Users/Edit/{id}
        // Displays the form to edit an existing user.
        public async Task<IActionResult> Edit(string id)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();  // Return NotFound if the user doesn't exist.
            }
            return View(user);  // Pass the IdentityUser to the view for editing.
        }

        [Authorize(Roles = "Admin")]
        // POST: /Users/Edit/{id}
        // Handles the form submission to update an existing user.
        [HttpPost]
        public async Task<IActionResult> Edit(string id, IdentityUser model)
        {
            // Check if the form data is valid.
            if (ModelState.IsValid)
            {
                // Find the user by their ID.
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();  // Return NotFound if the user doesn't exist.
                }

                // Update the user's username and email.
                user.UserName = model.UserName;
                user.Email = model.Email;
                var result = await _userManager.UpdateAsync(user);  // Save the changes.

                // If the update is successful, redirect to the Index view.
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
            return View(model);  // Return the model with validation errors if any.
        }

        [Authorize(Roles = "Admin")]
        // GET: /Users/Details/{id}
        // Displays the details of a specific user.
        public async Task<IActionResult> Details(string id)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();  // Return NotFound if the user doesn't exist.
            }
            return View(user);  // Pass the IdentityUser to the view for details.
        }

        [Authorize(Roles = "Admin")]
        // GET: /Users/Delete/{id}
        // Displays the confirmation page for deleting a user.
        public async Task<IActionResult> Delete(string id)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();  // Return NotFound if the user doesn't exist.
            }

            // Prevent the logged-in user from deleting their own account.
            if (user.Id == _userManager.GetUserId(User))
            {
                ModelState.AddModelError(string.Empty, "You cannot delete your own account.");
                return RedirectToAction(nameof(Index));  // Redirect back to the index or show an error page.
            }

            return View(user);  // Pass the IdentityUser to the delete confirmation view.
        }

        [Authorize(Roles = "Admin")]
        // POST: /Users/DeleteConfirmed
        // Handles the form submission to confirm the deletion of a user.
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Find the user by their ID.
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prevent the logged-in user from deleting their own account.
                if (user.Id == _userManager.GetUserId(User))
                {
                    ModelState.AddModelError(string.Empty, "You cannot delete your own account.");
                    return RedirectToAction(nameof(Index));  // Redirect back to the index or show an error page.
                }

                // Delete the user from the system.
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));  // Redirect to the index if successful.
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
            return RedirectToAction(nameof(Index));  // Redirect to the index after deletion attempt.
        }

        [Authorize]
        // POST: /Users/Logout
        // Logs the user out of the system.
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the current user and redirect to the home page.
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index", "Home");
        }
    }
}

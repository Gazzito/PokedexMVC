// Using statements to include necessary libraries and namespaces.
using Microsoft.AspNetCore.Authorization;  // Handles authorization for the application.
using Microsoft.AspNetCore.Mvc;  // Provides the MVC framework for handling web requests.
using PokedexMVC.Models;  // Imports the models defined in the PokedexMVC project.
using System.Diagnostics;  // Used for diagnostic information, such as logging request details.

namespace PokedexMVC.Controllers  // Defines the namespace for this controller class.
{
    // Apply the [Authorize] attribute to the HomeController, which ensures that
    // only authenticated users can access its actions.
    [Authorize]
    public class HomeController : Controller  // Inherit from the base Controller class to handle web requests.
    {
        // Define a logger for the HomeController to log messages for debugging or tracing.
        private readonly ILogger<HomeController> _logger;

        // Constructor for the HomeController class, which initializes the logger.
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;  // Assign the injected logger to the private _logger field.
        }

        // Action method for the default route (Index). It returns the Index view.
        public IActionResult Index()
        {
            return View();  // Render and return the Index view to the client.
        }

        // The Error action method is decorated with the [ResponseCache] attribute to
        // disable caching of the error page.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Return the Error view with an ErrorViewModel, passing the request's unique
            // ID from either the current Activity or the HTTP context trace identifier.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

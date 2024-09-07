using Microsoft.AspNetCore.Mvc;

namespace PokedexMVC.Controllers
{
    public class PokemonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

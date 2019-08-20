using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Views
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
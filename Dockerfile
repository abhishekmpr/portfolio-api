using Microsoft.AspNetCore.Mvc;

namespace PortfolioApi
{
    public class Dockerfile : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    public class ManageController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

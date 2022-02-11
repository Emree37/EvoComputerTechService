using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace it_service_app.Areas.Admin.Controllers
{
    public class ManageController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();

        }
        public IActionResult Users() 
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminBaseController : Controller
    {
      
    }
}

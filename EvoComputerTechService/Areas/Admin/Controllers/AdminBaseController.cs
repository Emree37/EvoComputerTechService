using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace it_service_app.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminBaseController : Controller
    {
       
    }
}

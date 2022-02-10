using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles= "Admin")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users=_userManager.Users.OrderBy(x=>x.CreatedDate).ToList();

            return Ok(new JSonResponseViewModel() 
            {
                Data=users      
            });
        }
    }
}

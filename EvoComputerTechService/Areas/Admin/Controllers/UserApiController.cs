using DevExtreme.AspNet.Data;
using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyContext _dbContext;

        public UserApiController(UserManager<ApplicationUser> userManager, MyContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetUsers(DataSourceLoadOptions loadOptions)
        {
            var data = _userManager.Users.Where(x=>x.IsDeleted == false);

            return Ok(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUsers(string key, string values)
        {
            var data = _userManager.Users.FirstOrDefault(x => x.Id == key);
            if (data == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Kullanıcı Bulunamadı"
                });
            }
            JsonConvert.PopulateObject(values, data);
            if (!TryValidateModel(data))
            {
                return BadRequest(ModelState.ToFullErrorString());
            }

            var result = await _userManager.UpdateAsync(data);
            if (!result.Succeeded)
            {
                return BadRequest(new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Kullanıcı Güncellemedi"
                });
            }
            return Ok(new JSonResponseViewModel());
        }

        [HttpDelete]
        public IActionResult DeleteUser(string key)
        {
            var data = _userManager.Users.FirstOrDefault(x => x.Id == key);
            if (data == null)
            {
                return StatusCode(StatusCodes.Status409Conflict, new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Kullanıcı Bulunamadı"
                });
            }

            data.IsDeleted = true;
            var result = _dbContext.SaveChanges();
            if (result == 0)
            {
                return BadRequest(new JSonResponseViewModel()
                {
                    IsSuccess = false,
                    ErrorMessage = "Kullanıcı Güncellemedi"
                });
            }

            return Ok(new JSonResponseViewModel());

        }
    }
}

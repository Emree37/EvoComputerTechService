using AutoMapper;
using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Services;
using EvoComputerTechService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Controllers
{
    [Authorize]
    public class IssueController : Controller
    {
        private readonly MyContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public IssueController(MyContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetIssues()
        {
            var issues = _dbContext.Issues.ToList();

            return View(issues);
        }

        [HttpGet]
        public IActionResult CreateIssue()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateIssue(string lat,string lng,Issue model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());
            if(user == null)
            {

            }

            var issue = new Issue()
            {
                IssueName = model.IssueName,
                Description = model.Description,
                AddressDetail = model.AddressDetail,
                UserId = user.Id,
                IssueState = IssueStates.Beklemede,
                CreatedDate = DateTime.Now,
                CreatedUser = user.Id,
                Latitude = lat,
                Longitude = lng
            };

            _dbContext.Issues.Add(issue);
            _dbContext.SaveChanges();


            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateIssue()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateIssue(Issue model)
        {
            return View();
        }
    }
}

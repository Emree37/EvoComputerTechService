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
        public async Task<IActionResult> GetIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());
            var issues = _dbContext.Issues.Where(x=>x.UserId == user.Id).ToList();
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
        public IActionResult UpdateIssue(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            if(issue == null)
            {

            }

            return View(issue);
        }

        [HttpPost]
        public IActionResult UpdateIssue(string lat, string lng, Issue model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Issue issue = _dbContext.Issues.Find(model.Id);
            issue.IssueName = model.IssueName;
            issue.Description = model.Description;
            issue.AddressDetail = model.AddressDetail;
            issue.UpdatedDate = DateTime.Now;
            issue.Latitude = lat;
            issue.Longitude = lng;

            _dbContext.SaveChanges();
            return RedirectToAction("GetIssues");
        }

        
        public IActionResult DeleteIssue(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            if (issue == null)
            {

            }

            _dbContext.Remove(issue);
            _dbContext.SaveChanges();

            return RedirectToAction("GetIssues");
        }
    }
}

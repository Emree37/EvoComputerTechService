using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    public class TechnicianController : TechnicianBaseController
    {
        private readonly MyContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TechnicianController(MyContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var myIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id && x.IssueState == IssueStates.Atandi).ToList();

            return View(myIssues);
        }

        
        public IActionResult AcceptIssue(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            issue.IssueState = IssueStates.Islemde;
            _dbContext.SaveChanges();

            return RedirectToAction("AcceptedIssues");
        }

        [HttpGet]
        public async Task<IActionResult> AcceptedIssues()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.GetUserId());

            var acceptedIssues = _dbContext.Issues.Where(x => x.TechnicianId == user.Id &&
                x.IssueState == IssueStates.Islemde)
                .ToList();

            return View(acceptedIssues);
        }

        [HttpGet]
        public IActionResult IssueDetail(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);

            return View();
        }
    }
}

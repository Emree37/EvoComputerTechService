using EvoComputerTechService.Data;
using EvoComputerTechService.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoComputerTechService.Areas.Admin.Controllers
{
    public class OperatorController : OperatorBaseController
    {
        private readonly MyContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public OperatorController(MyContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AllIssues()
        {
            var allIssues = _dbContext.Issues.ToList();

            return View(allIssues);
        }

        [HttpGet]
        public IActionResult IssueDetail(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            if (issue == null)
            {

            }

            return View(issue);
        }
    }
}

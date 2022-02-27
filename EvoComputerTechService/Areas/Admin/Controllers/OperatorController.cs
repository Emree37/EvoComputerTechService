using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult WaitingIssues()
        {
            var waitingIssues = _dbContext.Issues.Where(x=>x.IssueState == IssueStates.Beklemede).ToList();

            return View(waitingIssues);
        }

        [HttpGet]
        public IActionResult ActiveIssues()
        {
            var activeIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Islemde || x.IssueState == IssueStates.Kuyrukta).ToList();

            return View(activeIssues);
        }


        [HttpGet]
        public IActionResult CompletedIssues()
        {
            var completedIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Tamamlandi).ToList();

            return View(completedIssues);
        }

        [HttpGet]
        public IActionResult PaidIssues()
        {
            var paidIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Odendi).ToList();

            return View(paidIssues);
        }

        [HttpGet]
        public IActionResult Issue(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);

            return View(issue);
        }

        [HttpGet]
        public IActionResult AssignTechnician(Guid id)
        {
            var issue = _dbContext.Issues.Find(id);

            var Technicians = new List<SelectListItem>();

            var x = _userManager.GetUsersInRoleAsync("Technician").Result;
            var users = x.OfType<ApplicationUser>();

            foreach (var item in users)
            {
                Technicians.Add(new SelectListItem
                {
                    Text = $"{item.Name} {item.Surname}",
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Technicians = Technicians;
            

            return View(issue);
        }

        [HttpPost]
        public IActionResult AssignTechnician(string[] Technician,Guid id)
        {
            var issue = _dbContext.Issues.Find(id);
            issue.TechnicianId = Technician[0];

            //Atama işlemi
            var technician = _dbContext.Users.Find(Technician[0]);
            var issues = _dbContext.Issues.Where(x => x.TechnicianId == technician.Id && (x.IssueState == IssueStates.Kuyrukta || x.IssueState == IssueStates.Islemde));

            if(issues.Count() > 0)
            {
                issue.IssueState = IssueStates.Kuyrukta;
            }
            else
            {
                issue.IssueState = IssueStates.Islemde;
            }

            _dbContext.SaveChanges();

            return RedirectToAction("ActiveIssues");
        }



    }
}

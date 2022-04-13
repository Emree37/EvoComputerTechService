using EvoComputerTechService.Data;
using EvoComputerTechService.Extensions;
using EvoComputerTechService.Models;
using EvoComputerTechService.Models.Entities;
using EvoComputerTechService.Models.Identity;
using EvoComputerTechService.Services;
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
        private readonly IEmailSender _emailSender;

        public OperatorController(MyContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult WaitingIssues()
        {
            var waitingIssues = _dbContext.Issues.Where(x=>x.IssueState == IssueStates.Beklemede && x.IsDeleted == false).ToList();

            return View(waitingIssues);
        }

        [HttpGet]
        public IActionResult ActiveIssues()
        {
            var activeIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Islemde || x.IssueState == IssueStates.Kuyrukta).ToList();
            var activeIssuess = activeIssues.Where(x => x.IsDeleted == false).ToList();

            return View(activeIssuess);
        }


        [HttpGet]
        public IActionResult CompletedIssues()
        {
            var completedIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Tamamlandi && x.IsDeleted == false).ToList();

            return View(completedIssues);
        }

        [HttpGet]
        public IActionResult PaidIssues()
        {
            var paidIssues = _dbContext.Issues.Include(x => x.Technician).Where(x => x.IssueState == IssueStates.Odendi && x.IsDeleted == false).ToList();

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
        public async Task<IActionResult> AssignTechnician(string[] Technician,Guid id)
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

            //Mail atılması lazım gerek....
            var emailMessage = new EmailMessage()
            {
                //Contacts = new string[] { technician.Email },
                Contacts = new string[] { "manifestationoffate@gmail.com" },
                Body = $"Merhaba {technician.Name} {technician.Surname}, <br/> {issue.IssueName} İsimli Arıza Kaydı Tarafınıza Atanmıştır.",
                Subject = "Arıza Kaydı Ataması"
            };

            await _emailSender.SendAsync(emailMessage);



            return RedirectToAction("ActiveIssues");
        }



    }
}
